using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Rectangle))]

public class RectEditor : Editor
{
    /*
   [MenuItem("GameObject/Create Other/Rect")]
   static void Create()
   {
       GameObject gameObject = new GameObject("Rect");
       Rect s = gameObject.AddComponent<Rect>();
       MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
       meshFilter.mesh = new Mesh();
       s.Rebuild();
   }
   */

    public override void OnInspectorGUI()
    {
        Rectangle obj;

        obj = target as Rectangle;

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
