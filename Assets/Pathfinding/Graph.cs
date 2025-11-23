using UnityEngine;
/*
    struttura che contiene tutti i nodi del grafo, organizzati in una matrice 2d
    istanziato in PerlinWalk
    usato in PathfinderAStar, SpawnerAStar e AgentMovement
*/
public class Graph
{
    //matrice dei nodi
    public GraphNode[,] nodes;

    //dimensioni totali del grafo
    public int width;
    public int height;

    //ricevo heightmap normalizzata e terrain per sapere l'altezza reale
    public Graph(float[,] heightmap, Terrain terrain)
    {

        width = heightmap.GetLength(1);
        this.height = heightmap.GetLength(0);

        //allocazione della matrice dei nodi
        nodes = new GraphNode[this.height, width];

        //altezza massima del mondo
        float maxH = terrain.terrainData.size.y;

        //doppio ciclo su ogni cella
        for (int z = 0; z < this.height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                //conversione dell'altezza normalizzata in altezza del mondo
                float worldH = heightmap[z, x] * maxH;

                //evita posti in cui c'è sempre l'acqua
                bool walk = worldH > 0.5f; 

                //creazione del graph node con le informazioni corrette
                nodes[z, x] = new GraphNode(x, z, worldH, walk);
            }
        }
    }

    //restituisce il nodo alle coordinate x,z
    public GraphNode GetNode(int x, int z)
    {
        return nodes[z, x];
    }
}