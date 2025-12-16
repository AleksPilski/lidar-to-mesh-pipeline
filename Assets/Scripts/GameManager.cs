using UnityEngine;

public class GameManager : MonoBehaviour
{
    public DataProcessor dataProcessor;
    public ObjectClusterProcessor objectClusterProcessor;

    public void Start()
    {
        // Full process on start (optional)
        PreprocessAndProcess();
    }

    public void PreprocessAndProcess()
    {
        // Step 1: Preprocess Data
        if (dataProcessor != null)
        {
            dataProcessor.PreprocessData();
        }
        else
        {
            Debug.LogError("GameManager: DataProcessor is not assigned.");
        }

        // Step 2: Initialize Cluster Processor
        if (objectClusterProcessor != null)
        {
            objectClusterProcessor.InitData();
        }
        else
        {
            Debug.LogError("GameManager: ObjectClusterProcessor is not assigned.");
        }
    }

    public void ResetTest()
    {
        Debug.Log("Resetting test...");

        // Clear existing processed data
        if (dataProcessor != null)
        {
            dataProcessor.processedDsmData = null;
            dataProcessor.processedDtmData = null;
        }

        // Clear existing clusters and GameObjects
        if (objectClusterProcessor != null)
        {
            objectClusterProcessor.detectedCluster = null;

            if (objectClusterProcessor.processedObject != null)
            {
                DestroyImmediate(objectClusterProcessor.processedObject);
                objectClusterProcessor.processedObject = null;
            }
        }

        Debug.Log("Test reset complete.");
    }

    public float Offset
    {
        get => dataProcessor != null ? dataProcessor.offset : 0f;
        set
        {
            if (dataProcessor != null)
            {
                dataProcessor.offset = value;
                Debug.Log($"Offset set to {value}");
            }
        }
    }
}


