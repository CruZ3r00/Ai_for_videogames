using UnityEngine;

public class Graph
{
    public GraphNode[,] nodes;
    public int width;
    public int height;

    public Graph(float[,] heightmap, Terrain terrain)
    {
        width = heightmap.GetLength(1);
        this.height = heightmap.GetLength(0);

        nodes = new GraphNode[this.height, width];

        float maxH = terrain.terrainData.size.y;

        for (int z = 0; z < this.height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float worldH = heightmap[z, x] * maxH;

                bool walk = worldH > 0.5f; // esempio: niente sotto acqua

                nodes[z, x] = new GraphNode(x, z, worldH, walk);
            }
        }
    }

    public GraphNode GetNode(int x, int z)
    {
        return nodes[z, x];
    }
}