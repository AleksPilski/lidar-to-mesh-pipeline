using System.Collections.Generic;
using UnityEngine;

public class ObjectClusterProcessor : MonoBehaviour
{
    public DataProcessor dataProcessor; // Reference to DataProcessor
    public float poiHeightThreshold = 0; // Threshold in meters for detecting POIs
    public List<Vector3> detectedCluster; // Detected cluster of points
    public float voxelSize = 0.5f;

    public GameObject processedObject;

    // Property to expose the highest point height
    public float HighestPointY { get; private set; }

    public void InitData()
    {
        if (dataProcessor == null || dataProcessor.processedDsmData == null || dataProcessor.processedDtmData == null)
        {
            Debug.LogError("ObjectClusterProcessor: DataProcessor or its data is not ready!");
            return;
        }

        // Initialize the cluster to ensure it's always fresh
        detectedCluster = new List<Vector3>();

        // Detect the cluster using preprocessed data
        detectedCluster = DetectCluster(dataProcessor.processedDsmData, dataProcessor.processedDtmData);
    }


    private List<Vector3> DetectCluster(float[,] dsmHeights, float[,] dtmHeights)
    {
        int width = dsmHeights.GetLength(0);
        int height = dsmHeights.GetLength(1);

        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
        float highestPointHeight = 0f;
        Vector3Int highestPointPosition = new Vector3Int(-1, -1, -1);

        // Step 1: Find the highest point of interest (POI)
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float heightDifference = dsmHeights[z, x] - dtmHeights[z, x]; // Already in meters
                if (heightDifference > highestPointHeight)
                {
                    highestPointHeight = heightDifference;
                    highestPointPosition = new Vector3Int(x, 0, z);
                }
            }
        }

        if (highestPointPosition.x == -1)
        {
            Debug.Log("No high points found to represent a cluster.");
            return detectedCluster;
        }

        HighestPointY = highestPointHeight; // Store the height of the highest point

        // Add the highest point to the cluster
        detectedCluster.Add(new Vector3(highestPointPosition.x, highestPointHeight, highestPointPosition.z));
        visited.Add(highestPointPosition);

        Queue<Vector3Int> toExplore = new Queue<Vector3Int>();
        toExplore.Enqueue(highestPointPosition);

        // Step 2: Explore neighbors and expand the cluster
        while (toExplore.Count > 0)
        {
            var currentPoint = toExplore.Dequeue();
            var neighbors = GetNeighbors(currentPoint, width, height);

            foreach (var neighbor in neighbors)
            {
                if (visited.Contains(neighbor)) // Skip already visited points
                    continue;

                float neighborHeightDifference = dsmHeights[neighbor.z, neighbor.x] - dtmHeights[neighbor.z, neighbor.x]; // Already in meters

                // Safety net: Handle 0 threshold
                if ((poiHeightThreshold > 0 && neighborHeightDifference > poiHeightThreshold) ||
                    (poiHeightThreshold == 0 && neighborHeightDifference > 0)) // Stop at ground level
                {
                    detectedCluster.Add(new Vector3(neighbor.x, neighborHeightDifference, neighbor.z));
                    visited.Add(neighbor);
                    toExplore.Enqueue(neighbor);
                }
            }
        }

        Debug.Log($"Cluster detected with {detectedCluster.Count} points.");
        return detectedCluster;
    }


    private List<Vector3Int> GetNeighbors(Vector3Int point, int width, int height)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 }; // Includes diagonals
        int[] dz = { -1, -1, -1, 0, 0, 1, 1, 1 };

        for (int i = 0; i < 8; i++) // Check all 8 neighbors
        {
            int newX = point.x + dx[i];
            int newZ = point.z + dz[i];
            if (newX >= 0 && newX < width && newZ >= 0 && newZ < height)
                neighbors.Add(new Vector3Int(newX, 0, newZ));
        }

        return neighbors;
    }




    public void GenerateVoxels()
    {
        if (detectedCluster == null || detectedCluster.Count == 0)
        {
            Debug.LogError("ObjectClusterProcessor: No cluster detected to generate voxels.");
            return;
        }

        processedObject = new GameObject("VoxelizedObject");
        foreach (Vector3 point in detectedCluster)
        {
            GameObject voxel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            voxel.transform.position = new Vector3(point.x * voxelSize, point.y, point.z * voxelSize);
            voxel.transform.localScale = Vector3.one * voxelSize;
            voxel.transform.parent = processedObject.transform;
        }

        Debug.Log("Voxelized representation generated.");
    }

    public void CreateObjectCuboid()
    {
        if (detectedCluster == null || detectedCluster.Count == 0)
        {
            Debug.LogError("ObjectClusterProcessor: No cluster detected to create cuboid.");
            return;
        }

        // Find bounds of the cluster
        float minX = float.MaxValue, maxX = float.MinValue;
        float minZ = float.MaxValue, maxZ = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        foreach (Vector3 point in detectedCluster)
        {
            minX = Mathf.Min(minX, point.x);
            maxX = Mathf.Max(maxX, point.x);
            minZ = Mathf.Min(minZ, point.z);
            maxZ = Mathf.Max(maxZ, point.z);
            minY = Mathf.Min(minY, point.y);
            maxY = Mathf.Max(maxY, point.y);
        }

        // Create cuboid
        Vector3 center = new Vector3((minX + maxX) / 2 * voxelSize, (minY + maxY) / 2, (minZ + maxZ) / 2 * voxelSize);
        Vector3 size = new Vector3((maxX - minX) * voxelSize, maxY - minY, (maxZ - minZ) * voxelSize);

        GameObject cuboid = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cuboid.transform.position = center;
        cuboid.transform.localScale = size;
        cuboid.transform.parent = processedObject.transform;

        Debug.Log("Cuboid created.");
    }
}


