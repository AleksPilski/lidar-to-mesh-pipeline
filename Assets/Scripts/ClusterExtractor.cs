using System.Collections.Generic;
using UnityEngine;

public class ClusterExtractor : MonoBehaviour
{
    public Terrain dsmTerrain; // DSM terrain
    public Terrain dtmTerrain; // DTM terrain
    public float poiHeightThreshold = 0; // Threshold in meters for detecting POIs
    public float heighestPointY = 0; // Height of the highest detected point
    public List<Vector3> cathedralCluster; // Detected cluster of points
    public float voxelSize = 0.5f;

    public GameObject cathedral;

    public void Start()
    {
        InitData();
        GenerateVoxelsForCathedralCluster(cathedralCluster); 
        CreateTowerCuboid();
    }

    public void InitData()
    {
        // Initialize the cluster
        cathedralCluster = new List<Vector3>();

        // Access TerrainData for both DSM and DTM
        UnityEngine.TerrainData dsmData = dsmTerrain.terrainData;
        UnityEngine.TerrainData dtmData = dtmTerrain.terrainData;

        // Get heightmaps from both terrains
        float[,] dsmHeights = dsmData.GetHeights(0, 0, dsmData.heightmapResolution, dsmData.heightmapResolution);
        float[,] dtmHeights = dtmData.GetHeights(0, 0, dtmData.heightmapResolution, dtmData.heightmapResolution);

        // Find the cluster of points representing the cathedral
        cathedralCluster = DetectCathedral(dsmHeights, dtmHeights);
    }

    private List<Vector3> DetectCathedral(float[,] dsmHeights, float[,] dtmHeights)
    {
        int width = dsmHeights.GetLength(0);
        int height = dsmHeights.GetLength(1);

        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        float highestPOIHeight = 0f;
        Vector3Int highestPOIPosition = new Vector3Int(-1, -1, -1);

        // Step 1: Find the highest point of interest (POI)
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float heightDifference = dsmHeights[z, x] - dtmHeights[z, x];
                float heightDifferenceInMeters = heightDifference * dsmTerrain.terrainData.size.y;

                if (heightDifferenceInMeters > highestPOIHeight)
                {
                    highestPOIHeight = heightDifferenceInMeters;
                    highestPOIPosition = new Vector3Int(x, 0, z);
                }
            }
        }

        if (highestPOIPosition.x == -1)
        {
            Debug.Log("No high points found to represent a cathedral.");
            return cathedralCluster;
        }

        heighestPointY = highestPOIHeight; // Store the height of the highest point

        // Add the highest point to the cluster
        cathedralCluster.Add(highestPOIPosition);
        visited.Add(highestPOIPosition);

        Queue<Vector3Int> toExplore = new Queue<Vector3Int>();
        toExplore.Enqueue(highestPOIPosition);

        // Step 2: Explore neighbors and expand the cluster
        while (toExplore.Count > 0)
        {
            var currentPoint = toExplore.Dequeue();
            var neighbors = GetNeighbors(currentPoint, width, height);

            foreach (var neighbor in neighbors)
            {
                if (visited.Contains(neighbor)) // Skip already visited points
                    continue;

                float neighborHeightDifference = dsmHeights[neighbor.z, neighbor.x] - dtmHeights[neighbor.z, neighbor.x];
                float neighborHeightDifferenceInMeters = neighborHeightDifference * dsmTerrain.terrainData.size.y;

                // Safety net: Handle 0 threshold
                if ((poiHeightThreshold > 0 && neighborHeightDifferenceInMeters > poiHeightThreshold) ||
                    (poiHeightThreshold == 0 && neighborHeightDifferenceInMeters > 0)) // Stop at ground level
                {
                    cathedralCluster.Add(new Vector3(neighbor.x, neighborHeightDifferenceInMeters, neighbor.z));
                    visited.Add(neighbor);
                    toExplore.Enqueue(neighbor);
                }
            }
        }

        Debug.Log($"Cathedral cluster detected with {cathedralCluster.Count} POIs.");
        return cathedralCluster;
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

    // Helper method to get the height of the terrain at a given position
    public float GetTerrainHeightAtPosition(Vector3 position, Terrain terrain)
    {
        // Convert world position to terrain local position
        Vector3 terrainLocalPos = position - terrain.gameObject.transform.position;

        // Normalize position with respect to the terrain size
        Vector2 normalizedPos = new Vector2(terrainLocalPos.x / terrain.terrainData.size.x, terrainLocalPos.z / terrain.terrainData.size.z);

        // Get the height at the normalized position
        float height = terrain.terrainData.GetHeight(Mathf.RoundToInt(normalizedPos.x * terrain.terrainData.heightmapResolution), Mathf.RoundToInt(normalizedPos.y * terrain.terrainData.heightmapResolution));

        // Convert height to world space
        height += terrain.transform.position.y;

        return height;
    }

    public Vector3 AlignToWorldSpace(Vector3 clusterPoint)
    {
        // Get terrain data for size and resolution
        float terrainWidth = dsmTerrain.terrainData.size.x;
        float terrainDepth = dsmTerrain.terrainData.size.z;
        int heightmapWidth = dsmTerrain.terrainData.heightmapResolution;
        int heightmapHeight = dsmTerrain.terrainData.heightmapResolution;

        // Convert cluster point (heightmap space) to world space
        float worldPosX = clusterPoint.x / heightmapWidth * terrainWidth;
        float worldPosZ = clusterPoint.z / heightmapHeight * terrainDepth;

        // Determine height at this point from DSM terrain
        Vector3 terrainLocalPos = new Vector3(worldPosX, 0, worldPosZ);
        float baseHeightDSM = GetTerrainHeightAtPosition(terrainLocalPos, dsmTerrain);

        // Align to voxel grid
        float alignedX = Mathf.Floor(worldPosX / voxelSize) * voxelSize + voxelSize / 2;
        float alignedY = Mathf.Floor(baseHeightDSM / voxelSize) * voxelSize + voxelSize / 2;
        float alignedZ = Mathf.Floor(worldPosZ / voxelSize) * voxelSize + voxelSize / 2;

        return new Vector3(alignedX, alignedY, alignedZ);
    }

    public void GenerateVoxelsForCathedralCluster(List<Vector3> clusterPoints)
    {
        // Create a parent GameObject for the voxelized representation
        cathedral = new GameObject("VoxelizedCathedral");
        cathedral.transform.parent = this.transform;

        // Create a separate folder GameObject for the voxels
        GameObject voxelFolder = new GameObject("Voxels");
        voxelFolder.transform.parent = cathedral.transform;

        // Iterate through the cluster points
        foreach (Vector3 point in clusterPoints)
        {
            // Align cluster point to world space
            Vector3 alignedPosition = AlignToWorldSpace(point);

            // Create the voxel as a cube
            GameObject voxel = GameObject.CreatePrimitive(PrimitiveType.Cube);

            // Set position and scale
            voxel.transform.position = alignedPosition;
            voxel.transform.localScale = new Vector3(voxelSize, voxelSize, voxelSize);

            // Set the voxel as a child of the voxel folder GameObject
            voxel.transform.parent = voxelFolder.transform;
        }

        Debug.Log($"Voxelized representation generated with {clusterPoints.Count} voxels, organized under the 'Voxels' folder.");
    }


    public void CreateTowerCuboid()
    {
        // Find corners of the cluster
        Vector3 west = cathedralCluster[0];
        Vector3 east = cathedralCluster[0];
        Vector3 north = cathedralCluster[0];
        Vector3 south = cathedralCluster[0];

        foreach (var point in cathedralCluster)
        {
            if (point.x < west.x) west = point;
            if (point.x > east.x) east = point;
            if (point.z < south.z) south = point;
            if (point.z > north.z) north = point;
        }

        Debug.Log($"West: {west}");
        Debug.Log($"North: {north}");
        Debug.Log($"East: {east}");
        Debug.Log($"South: {south}");

        // Calculate the rotation angle
        Vector3 edgeVector = north - west; // Vector along one edge of the cluster
        Vector2 edgeVector2D = new Vector2(edgeVector.x, edgeVector.z); // Project to XZ plane
        float angle = Mathf.Atan2(edgeVector2D.y, edgeVector2D.x) * Mathf.Rad2Deg;

        Debug.Log($"Angle to world: {angle}");


        // Average height
        float averageHeight = (west.y + east.y + north.y + south.y) / 4;

        // Align corners to world space
        Vector3 alignedWest = AlignToWorldSpace(west); 
        Vector3 alignedEast = AlignToWorldSpace(east);
        Vector3 alignedNorth = AlignToWorldSpace(north);
        Vector3 alignedSouth = AlignToWorldSpace(south);

        Debug.Log($"Aligned West: {alignedWest}");
        Debug.Log($"Aligned North: {alignedNorth}");
        Debug.Log($"Aligned East: {alignedEast}");
        Debug.Log($"Aligned South: {alignedSouth}");

        // Compute cuboid center and size
        Vector3 cuboidCenter = new Vector3(
            (alignedWest.x + alignedEast.x) / 2,
            averageHeight / 2,
            (alignedNorth.z + alignedSouth.z) / 2
        );

        Vector3 cuboidSize = new Vector3(
            Mathf.Abs(alignedEast.x - alignedWest.x),
            averageHeight,
            Mathf.Abs(alignedNorth.z - alignedSouth.z)
        );

        // Create cuboid for the tower body
        GameObject towerBody = GameObject.CreatePrimitive(PrimitiveType.Cube);
        towerBody.transform.position = cuboidCenter;
        towerBody.transform.localScale = cuboidSize;

        // Apply rotation to align with the cluster
        towerBody.transform.rotation = Quaternion.Euler(0, angle, 0);

        towerBody.name = "Cathedral Tower Body";

        // Set the cuboid as a child of the parent GameObject
        towerBody.transform.parent = cathedral.transform;
    }
}



