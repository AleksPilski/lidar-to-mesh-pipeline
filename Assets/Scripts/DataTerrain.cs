using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTerrain : MonoBehaviour
{
    public Terrain dtmTerrain;
    public Terrain dsmTerrain;

    private float[,] dtmHeights;
    private float[,] dsmHeights;

    public void TestData()
    {
        dtmHeights = dtmTerrain.terrainData.GetHeights(0, 0, dtmTerrain.terrainData.heightmapResolution, dtmTerrain.terrainData.heightmapResolution);
        dsmHeights = dsmTerrain.terrainData.GetHeights(0, 0, dsmTerrain.terrainData.heightmapResolution, dsmTerrain.terrainData.heightmapResolution);

        int width = dtmTerrain.terrainData.heightmapResolution;
        int height = dtmTerrain.terrainData.heightmapResolution;

        if (dsmTerrain.terrainData.heightmapResolution != width || dsmTerrain.terrainData.heightmapResolution != height)
        {
            Debug.LogError("Test Failed: Terrain resolutions do not match.");
            return;
        }

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if (dtmHeights[x, z] <= 0 && dsmHeights[x, z] <= 0)
                {
                    continue;
                }

                if (dtmHeights[x, z] == 0 || dsmHeights[x, z] == 0)
                {
                    Debug.LogError($"Test Failed: Data is missing at ({x}, {z}). DTM Height: {dtmHeights[x, z]}, DSM Height: {dsmHeights[x, z]}");
                    return;
                }
            }
        }

        Debug.Log("Test Passed: All data is present.");
        DebugPoint(1000, 1000);
        DebugPoint(FindHighestPoint(dsmHeights));
        DebugPoint(FindHighestPoint(dtmHeights));
        DebugPoint(FindHighestDifferencePoint(dsmHeights, dtmHeights));
    }

    private void DebugPoint(int checkX, int checkZ)
    {
        if (checkX < dtmTerrain.terrainData.heightmapResolution && checkZ < dtmTerrain.terrainData.heightmapResolution)
        {
            float dtmHeightAtPoint = dtmHeights[checkX, checkZ];
            float dsmHeightAtPoint = dsmHeights[checkX, checkZ];
            Debug.Log($"Original DTM Height at ({checkX}, {checkZ}): {dtmHeightAtPoint}");
            Debug.Log($"Original DSM Height at ({checkX}, {checkZ}): {dsmHeightAtPoint}");
        }
        else
        {
            Debug.LogError($"Test Failed: Specified point ({checkX}, {checkZ}) is out of bounds.");
        }
    }

    private void DebugPoint(Vector2Int point)
    {
        int checkX = point.x;
        int checkZ = point.y;

        if (checkX < dtmTerrain.terrainData.heightmapResolution && checkZ < dtmTerrain.terrainData.heightmapResolution)
        {
            float dtmHeightAtPoint = dtmHeights[checkX, checkZ];
            float dsmHeightAtPoint = dsmHeights[checkX, checkZ];
            Debug.Log($"Original DTM Height at ({checkX}, {checkZ}): {dtmHeightAtPoint}");
            Debug.Log($"Original DSM Height at ({checkX}, {checkZ}): {dsmHeightAtPoint}");
        }
        else
        {
            Debug.LogError($"Test Failed: Specified point ({checkX}, {checkZ}) is out of bounds.");
        }
    }

    public Vector2Int FindHighestPoint(float[,] heights)
    {
        int width = heights.GetLength(0);
        int height = heights.GetLength(1);

        float maxHeight = float.MinValue;
        Vector2Int highestPoint = Vector2Int.zero;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if (heights[x, z] > maxHeight)
                {
                    maxHeight = heights[x, z];
                    highestPoint = new Vector2Int(x, z);
                }
            }
        }

        return highestPoint;
    }

    public Vector2Int FindHighestDifferencePoint(float[,] dsmHeights, float[,] dtmHeights)
    {
        int width = dsmHeights.GetLength(0);
        int height = dsmHeights.GetLength(1);

        float maxDifference = float.MinValue;
        Vector2Int highestDifferencePoint = Vector2Int.zero;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float difference = dsmHeights[x, z] - dtmHeights[x, z];

                if (difference > maxDifference)
                {
                    //Debug.Log($"Updating maxDifference: Previous maxDifference = {maxDifference}, New maxDifference = {difference} at point ({x}, {z})");

                    maxDifference = difference;
                    highestDifferencePoint = new Vector2Int(x, z);
                }
            }
        }

        return highestDifferencePoint;
    }
}

