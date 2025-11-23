using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using Unity.VisualScripting;

[RequireComponent (typeof (Terrain))]

public class PerlinWalk : MonoBehaviour {
	public GameObject targetMarkerPrefab;
	public static event Action<float[,]> OnTerrainGenerated;

	public Texture2D baseHeightMap = null;
	public int resolution = 50;
	public float amplitude = 0f;
	public bool addHarmonics = false;
	public int RandomSeed = 0;
	public Transform agentToAdjust = null;

	private Terrain t;

	void Start () {
		if (RandomSeed == 0) RandomSeed = (int) System.DateTime.Now.Ticks;

		t = GetComponent<Terrain> ();

		StartCoroutine(GenerateLandscape());
	}

	private void OnValidate() {
		if (t)
			StartCoroutine(GenerateLandscape());
	}

	private IEnumerator GenerateLandscape() {

		yield return new WaitForEndOfFrame();

		int lx = t.terrainData.heightmapResolution;
		int ly = t.terrainData.heightmapResolution;

		UnityEngine.Random.InitState (RandomSeed);

		float [,] height = new float [ly, lx];
		
		if (baseHeightMap) {
			int tx = baseHeightMap.width;
			int ty = baseHeightMap.height;
			for (int i = 0 ; i < lx ; i += 1) {
				for (int j = 0; j < ly; j += 1) {
					int rx = (int) Mathf.Lerp(0f, (float)tx, ((float) i / lx));
					int ry = (int) Mathf.Lerp(0f, (float)ty, ((float) j / ly));
					height[j, i] = baseHeightMap.GetPixel(rx, ry).grayscale;
				}
			}
		}
		height = Amplify(height, lx, ly, 0.75f);
		
		float [,] noise = CreateNoise(lx, ly, resolution);
		noise = Amplify(noise, lx, ly, amplitude);
		for (int i = 0 ; i < lx ; i += 1) {
			for (int j = 0; j < ly; j += 1) {
				height[j, i] += noise[j, i];
			}
		}

		if (addHarmonics) {
			float [,] noise2 = Amplify(CreateNoise(lx, ly, resolution / 2), lx, ly, amplitude / 2);
			float [,] noise4 = Amplify(CreateNoise(lx, ly, resolution / 4), lx, ly, amplitude / 4);
			float [,] noise8 = Amplify(CreateNoise(lx, ly, resolution / 8), lx, ly, amplitude / 8);
			for (int i = 0 ; i < lx ; i += 1) {
				for (int j = 0; j < ly; j += 1) {
					height[j, i] += noise2[j, i] + noise4[j, i] + noise8[i, j];
				}
			}
			height = ToGround(height, lx, ly);
		}
 		t.terrainData.SetHeights (0, 0, height);
		yield return new WaitForEndOfFrame();

		// ASPETTA due frame perché terrain/collider siano aggiornati
		t.terrainData.SyncHeightmap();
		yield return null;
		yield return new WaitForFixedUpdate();
		yield return null;

		// --- GRAFO ---
		Graph graph = new Graph(height, t);

		// --- MOVEMENT ---
		AgentMovement movement = agentToAdjust.GetComponent<AgentMovement>();
		movement.Initialize(graph, t);   // solo set dei riferimenti

		// --- SPAWN ---
		GraphNode start = graph.nodes[0, 0];
		float minHeight = 7.5f;

		GraphNode spawnNode = SpawnerAStar.FindSpawnPoint(graph, start, minHeight);
		if (spawnNode == null) Debug.LogWarning("SpawnNode NULL");

		AdjustAgent(agentToAdjust, t, spawnNode);

		// --- GOAL ---
		AgentController ac = agentToAdjust.GetComponent<AgentController>();
		if (ac != null)
		{
			int res = t.terrainData.heightmapResolution - 1;
			int goalX = res - spawnNode.x;
			int goalZ = res - spawnNode.z;

			GraphNode opposite = graph.nodes[goalZ, goalX];
			GraphNode goal = SpawnerAStar.FindSpawnPoint(graph, opposite, minHeight);

			ac.SetGoal(goal, t);
		}

		// ora che GoalNode è impostato, possiamo calcolare il path
		movement.StartPathfinding();

		yield break;

	}

	private float[,] CreateNoise(int x, int y, int resolution) {

		int xOut = 1 + resolution * (1 + (int)Mathf.Ceil (x / resolution));
		int yOut = 1 + resolution * (1 + (int)Mathf.Ceil (y / resolution));

		float [,] h = new float [y, x];
		Vector2 [,] slopes = new Vector2 [yOut, xOut];

		// first, set up the slopes and height at lattice points
		for (int i = 0; i < xOut; i += resolution) {
			for (int j = 0; j < yOut; j += resolution) {
				slopes [j, i] = UnityEngine.Random.insideUnitCircle;
			//	h [j, i] = 0;
			}
		}
		for (int i = 0; i < x; i += 1) {
			for (int j = 0; j < y; j += 1) {

				// find the neighbouring lattice points
				int floorI = resolution * ((int) Mathf.Floor ((float) i / resolution));
				int floorJ = resolution * ((int) Mathf.Floor ((float) j / resolution));
				int ceilI  = resolution * ((int) Mathf.Ceil  ((float) i / resolution));
				int ceilJ  = resolution * ((int) Mathf.Ceil  ((float) j / resolution));

				// calculate the four contribution to height 
				float h1 = Vector2.Dot (slopes [floorJ, floorI], new Vector2 (i, j) - new Vector2 (floorI, floorJ));
				float h2 = Vector2.Dot (slopes [floorJ, ceilI], new Vector2 (i, j) - new Vector2 (ceilI, floorJ));
				float h3 = Vector2.Dot (slopes [ceilJ, floorI], new Vector2 (i, j) - new Vector2 (floorI, ceilJ));
				float h4 = Vector2.Dot (slopes [ceilJ, ceilI], new Vector2 (i, j) - new Vector2 (ceilI, ceilJ));

				// calculate relative position inside the square
				float u = ((float) i - floorI) / resolution;
				float v = ((float) j - floorJ) / resolution;

				// interpolate by lerping first horizontally (for each couple) and then vertically
				float l1 = Mathf.Lerp (h1, h2, slope(u));
				float l2 = Mathf.Lerp (h3, h4, slope(u));
				float finalH = Mathf.Lerp (l1, l2, slope(v));

				h [j, i] = finalH;
			}
		}
		return Normalize(h, x, y);
	}

	private static float slope (float x) {
		return -2f * Mathf.Pow (x, 3) + 3f * Mathf.Pow (x, 2);
	}

	private float [,] Normalize (float [,] m, int x, int y) {
		float max, min;
		max = float.MinValue;
		min = float.MaxValue;
		for (int i = 0; i < x; i += 1) {
			for (int j = 0; j < y; j += 1) {
				if (m [j, i] < min) min = m [j, i];
				if (m [j, i] > max) max = m [j, i];
			}
		}
		for (int i = 0; i < x; i += 1) {
			for (int j = 0; j < y; j += 1) {
				m [j, i] = (m [j, i] - min) / (max - min);
			}
		}
		return m;
	}

	private float [,] Amplify (float [,] m, int x, int y, float amplitude) {
		for (int i = 0; i < x; i += 1) {
			for (int j = 0; j < y; j += 1) {
				m [j, i] *= amplitude;
			}
		}
		return m;
	}

	private float [,] ToGround (float [,] m, int x, int y) {
		float min = float.MaxValue;
		for (int i = 0; i < x; i += 1) {
			for (int j = 0; j < y; j += 1) {
				if (m [j, i] < min) min = m [j, i];
			}
		}
		for (int i = 0; i < x; i += 1) {
			for (int j = 0; j < y; j += 1) {
				m [j, i] -= min;
			}
		}
		return m;
	}

	private void AdjustAgent(Transform a, Terrain t, GraphNode n) 
	{
		// Terrain size in world units
		float terrainWidth  = t.terrainData.size.x;
		float terrainLength = t.terrainData.size.z;

		// Convert node.x, node.z from grid coords → world coords
		float worldX = ((n.x +0.5f ) / (float)(t.terrainData.heightmapResolution - 1)) * terrainWidth;
		float worldZ = ((n.z  +0.5f )/ (float)(t.terrainData.heightmapResolution - 1)) * terrainLength;

		// Altezza reale del nodo
		float worldY = n.height + 01f;
		// Sposta l'agente
		a.position = new Vector3(worldX, worldY, worldZ);
	}
}