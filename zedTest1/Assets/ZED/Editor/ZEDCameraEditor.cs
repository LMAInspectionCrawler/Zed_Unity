//======= Copyright (c) Stereolabs Corporation, All rights reserved. ===============
using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom inspector :Add a button to the ZEDCameraSettingsEditor at the end of the panel ZEDManager
/// </summary>
[CustomEditor(typeof(ZEDManager)), CanEditMultipleObjects]
public class ZEDCameraEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(20);
        if (GUILayout.Button("Camera Control"))
        {
            EditorWindow.GetWindow(typeof(ZEDCameraSettingsEditor), false, "ZED Camera").Show();
        }

    }
}
