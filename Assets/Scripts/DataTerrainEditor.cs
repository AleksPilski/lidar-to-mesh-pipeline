using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DataTerrain))]
public class DataTerrainEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draws the default inspector

        DataTerrain script = (DataTerrain)target;

        if (GUILayout.Button("TestData"))
        {
            script.TestData();
        }
    }
}

