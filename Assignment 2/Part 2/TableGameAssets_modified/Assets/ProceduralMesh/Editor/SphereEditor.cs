using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Sphere))]

public class SphereEditor : Editor
{
    /*
    [MenuItem("GameObject/Create Other/Sphere")]
    static void Create()
    {
        GameObject gameObject = new GameObject("Sphere");
        Tetrahedron s = gameObject.AddComponent<Sphere>();
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        s.Rebuild();
    }
    */

    public override void OnInspectorGUI()
    {
        Sphere obj;

        obj = target as Sphere;

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
