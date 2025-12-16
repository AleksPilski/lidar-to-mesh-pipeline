using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    private float newPoiHeightThreshold = 0; // Temporary variable for the new POI height threshold

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draws the default inspector

        GameManager gameManager = (GameManager)target;

        // Display highest detected point height
        if (gameManager.objectClusterProcessor != null)
        {
            GUILayout.Space(10);
            GUILayout.Label("Cluster Data", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Highest Detected Point (Y):", gameManager.objectClusterProcessor.HighestPointY.ToString("F2"));
        }

        // Add Offset Control
        if (gameManager.dataProcessor != null)
        {
            GUILayout.Label("Offset Settings", EditorStyles.boldLabel);

            float newOffset = EditorGUILayout.FloatField("Offset", gameManager.Offset);

            if (!Mathf.Approximately(newOffset, gameManager.Offset))
            {
                gameManager.Offset = newOffset; // Update offset in the GameManager
            }
        }
        else
        {
            GUILayout.Label("DataProcessor not assigned.", EditorStyles.helpBox);
        }

        // Preprocess Data Button
        if (GUILayout.Button("Preprocess Data"))
        {
            if (gameManager.dataProcessor != null)
            {
                Debug.Log("Preprocessing data...");
                gameManager.dataProcessor.PreprocessData();
            }
            else
            {
                Debug.LogError("DataProcessor is not assigned in the GameManager.");
            }
        }

        // Initialize Object Cluster Processor Button
        if (GUILayout.Button("Initialize Object Cluster Processor"))
        {
            if (gameManager.objectClusterProcessor != null)
            {
                Debug.Log("Initializing Object Cluster Processor...");
                gameManager.objectClusterProcessor.InitData();
            }
            else
            {
                Debug.LogError("ObjectClusterProcessor is not assigned in the GameManager.");
            }
        }

        // Generate Voxels Button
        if (GUILayout.Button("Generate Voxels"))
        {
            if (gameManager.objectClusterProcessor != null)
            {
                Debug.Log("Generating voxels...");
                gameManager.objectClusterProcessor.GenerateVoxels();
            }
            else
            {
                Debug.LogError("ObjectClusterProcessor is not assigned in the GameManager.");
            }
        }

        // Create Object Cuboid Button
        if (GUILayout.Button("Create Object Cuboid"))
        {
            if (gameManager.objectClusterProcessor != null)
            {
                Debug.Log("Creating object cuboid...");
                gameManager.objectClusterProcessor.CreateObjectCuboid();
            }
            else
            {
                Debug.LogError("ObjectClusterProcessor is not assigned in the GameManager.");
            }
        }

        // Reset Test Button
        if (GUILayout.Button("Reset Test"))
        {
            Debug.Log("Resetting the test...");
            gameManager.ResetTest();
        }

        // Set POI Height Threshold Section
        GUILayout.Space(10);
        GUILayout.Label("Set POI Height Threshold", EditorStyles.boldLabel);
        newPoiHeightThreshold = EditorGUILayout.FloatField("Threshold Value", newPoiHeightThreshold);

        if (GUILayout.Button("Set POI Height Threshold"))
        {
            if (gameManager.objectClusterProcessor != null)
            {
                gameManager.objectClusterProcessor.poiHeightThreshold = newPoiHeightThreshold;
                Debug.Log($"POI Height Threshold set to {newPoiHeightThreshold}");
            }
            else
            {
                Debug.LogError("ObjectClusterProcessor is not assigned in the GameManager.");
            }
        }
    }
}







