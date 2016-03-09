using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Box))]

public class Dice2Editor : Editor
{
	/*
    [MenuItem("GameObject/Create Other/Dice")]
    static void Create()
    {
        GameObject gameObject = new GameObject("Box");
        Box s = gameObject.AddComponent<Box>();
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        s.Rebuild();
    }
    */

	public override void OnInspectorGUI()
	{
		Dice2 obj;

		obj = target as Dice2;

		if (obj == null)
		{
			return;
		}

		base.DrawDefaultInspector();
		EditorGUILayout.BeginHorizontal();

		// Rebuild mesh when user click the Rebuild button
		if (GUILayout.Button("Rebuild"))
		{
			obj.Rebuild();
		}
		EditorGUILayout.EndHorizontal();
	}

}