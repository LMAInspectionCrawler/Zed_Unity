//======= Copyright (c) Stereolabs Corporation, All rights reserved. ===============

using UnityEngine;
using UnityEditor;

/// <summary>
/// Checks if the SDK is well installed
/// </summary>
[InitializeOnLoad]
public class ZEDPluginInspector : EditorWindow
{
    /// <summary>
    /// ZED unity logo
    /// </summary>
    static Texture2D image = null;

    private static EditorWindow window;

    /// <summary>
    /// Error message to display
    /// </summary>
    private static string errorMessage = "";
    static void Init()
    {
		window = GetWindow<ZEDPluginInspector>(true);
        
        window.Show();
    }

	static ZEDPluginInspector()
    {
        EditorApplication.update += Update;
		ZEDPluginInspector.addMissingTag ();
    }

	static public void addMissingTag()
	{
		// Open tag manager
		SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
		SerializedProperty tagsProp = tagManager.FindProperty("tags");
		// Adding a Tag
		string s = "HelpObject";

		// Check if not here already
		bool found = false;
		for (int i = 0; i < tagsProp.arraySize; i++)
		{
			SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
			if (t.stringValue.Equals(s)) { found = true; break; }
		}

		// if not found, add it since we use it in GreenScreen. 
		// This tag may be used anywhere, since it tags helper object that may have a specific behavior

		if (!found)
		{
			tagsProp.InsertArrayElementAtIndex(0);
			SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
			n.stringValue = s;
		}


		// and to save the changes
		tagManager.ApplyModifiedProperties();
	}

    static void Update()
    {

        if (!sl.ZEDCamera.CheckPlugin())
        {
            errorMessage = ZEDLogMessage.Error2Str(ZEDLogMessage.ERROR.SDK_DEPENDENCIES_ISSUE);

            window = GetWindow<ZEDPluginInspector>(true);
            window.maxSize = new Vector2(400, 500);
            window.minSize = window.maxSize;
            window.Show(true);

        }
        EditorApplication.update -= Update;
    }

    void OnGUI()
    {
        if (image == null)
        {
            image = Resources.Load("Textures/logo", typeof(Texture2D)) as Texture2D;
           
        }
        var rect = GUILayoutUtility.GetRect(position.width, 150, GUI.skin.box);

        if (image)
        {
            GUI.DrawTexture(rect, image, ScaleMode.ScaleToFit);
        }
        GUIStyle myStyle = new GUIStyle(GUI.skin.label);
        myStyle.normal.textColor = Color.red;
        myStyle.fontStyle = FontStyle.Bold;

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("ZED SDK is not installed or needs to be updated", myStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        myStyle = new GUIStyle(GUI.skin.box);
        myStyle.normal.textColor = Color.red;

        GUI.Box(new Rect(0, position.height/2, position.width, 100), errorMessage, myStyle);
         


        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Close"))
        {
            this.Close();
        }

    }
}

