using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Cylinder))]

public class CylinderEditor : Editor
{
    /*
    [MenuItem("GameObject/Create Other/Cylinder")]
    static void Create()
    {
        GameObject gameObject = new GameObject("Cylinder");
        Tetrahedron s = gameObject.AddComponent<Cylinder>();
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        s.Rebuild();
    }
    */

    public override void OnInspectorGUI()
    {
        Cylinder obj;

        obj = target as Cylinder;

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