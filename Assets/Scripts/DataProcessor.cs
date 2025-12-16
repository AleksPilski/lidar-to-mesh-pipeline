using System.Collections.Generic;
using UnityEngine;

public class DataProcessor : MonoBehaviour
{
    public Terrain dsmTerrain; // Original DSM terrain
    public Terrain dtmTerrain; // Original DTM terrain

    public float voxelSize = 0.5f; // Voxel size used for alignment
    public float offset = 0;

    public float[,] processedDsmData; // Aligned DSM heightmap
    public float[,] processedDtmData; // Aligned DTM heightmap

    public void PreprocessData()
    {
        if (dsmTerrain == null || dtmTerrain == null)
        {
            Debug.LogError("DataProcessor: DSM or DTM terrain is not assigned.");
            return;
        }

        // Access TerrainData for both DSM and DTM
        TerrainData dsmData = dsmTerrain.terrainData;
        TerrainData dtmData = dtmTerrain.terrainData;

        // Get heightmaps from both terrains
        float[,] dsmHeights = dsmData.GetHeights(0, 0, dsmData.heightmapResolution, dsmData.heightmapResolution);
        float[,] dtmHeights = dtmData.GetHeights(0, 0, dtmData.heightmapResolution, dtmData.heightmapResolution);

        // Preprocess and align both heightmaps
        processedDsmData = AlignHeightmap(dsmHeights, dsmTerrain);
        processedDtmData = AlignHeightmap(dtmHeights, dtmTerrain);

        Debug.Log("DataProcessor: Data preprocessing complete.");
    }

    private float[,] AlignHeightmap(float[,] heightmap, Terrain terrain)
    {
        int width = heightmap.GetLength(0);
        int height = heightmap.GetLength(1);
        float[,] alignedHeightmap = new float[width, height];

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                // Convert the current point to world space
                float worldX = (float)x / width * terrain.terrainData.size.x;
                float worldZ = (float)z / height * terrain.terrainData.size.z;
                float heightValue = heightmap[z, x] * terrain.terrainData.size.y;

                // Align height to voxel grid
                float alignedHeight = Mathf.Floor(heightValue / voxelSize) * voxelSize;

                // Save aligned height value
                alignedHeightmap[z, x] = alignedHeight;
            }
        }

        Debug.Log("Heightmap alignment complete.");
        return alignedHeightmap;
    }
}



