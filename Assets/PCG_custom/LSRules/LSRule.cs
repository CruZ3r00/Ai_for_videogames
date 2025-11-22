/*
	adapted from https://github.com/SunnyValleyStudio/procedural_town_unity
*/
using UnityEngine;

[CreateAssetMenu(menuName ="LSMapGenerator/LSRule")]
	public class LSRule : ScriptableObject
	{
		public string letter;
		[SerializeField]
		private string[] results = null;
		[SerializeField]
		private bool randomResult = false;

		public string GetResult()
		{
		// Second option to make the L-System stochastic: a random rule is picked from the list
		if (randomResult)
			{
				int randomIndex = UnityEngine.Random.Range(0, results.Length);
				return results[randomIndex];
			}
			return results[0];
		}

	}

