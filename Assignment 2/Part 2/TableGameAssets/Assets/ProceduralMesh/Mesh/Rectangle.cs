using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class Rectangle : MonoBehaviour 
{
    public Vector2 Center = Vector2.zero;
    public float XWidth = 1.0f;
    public float YHeight = 1.0f;
    public int XSeg = 1;
    public int YSeg = 1;

    public void Rebuild()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter not found!");
            return;
        }

        if (XWidth <= 0.0f) XWidth = 1.0f;
        if (YHeight <= 0.0f) YHeight = 1.0f;
        if (XSeg <= 0) XSeg = 1;
        if (YSeg <= 0) YSeg = 1;

        Vector3[] vertices;
        Vector3[] normals;
        Vector2[] uvs;
        int[] indices;

        vertices = new Vector3[(XSeg + 1) * (YSeg + 1)];
        normals  = new Vector3[(XSeg + 1) * (YSeg + 1)];
        uvs      = new Vector2[(XSeg + 1) * (YSeg + 1)];
        indices  = new int[6 * XSeg * YSeg];

        float dx = XWidth  / XSeg;
        float dy = YHeight / YSeg;
        float x, y;
        float du = 1.0f / XSeg;
        float dv = 1.0f / YSeg;
        float u, v;

        int vindex = 0, findex = 0;
        y = Center.y - 0.5f * YHeight;
        v = 0.0f;
        for (int yi = 0; yi <= YSeg; yi++)
        {
            x = Center.x - 0.5f * XWidth;
            u = 1.0f;
            for (int xi = 0; xi <= XSeg; xi++) // Set row of vertices
            {
                vertices[vindex] = new Vector3(x, y, 0.0f);
                normals[vindex]  = Vector3.forward;            // Normal is (0,0,1)
                uvs[vindex]      = new Vector2(u, v);

                // Set vertice indices of two faces with one vertex at vindex-th vertice (CCW order)
                if (xi < XSeg && yi < YSeg)
                {
                    indices[findex++] = vindex;
                    indices[findex++] = vindex + 1;
                    indices[findex++] = vindex + XSeg + 2;

                    indices[findex++] = vindex;
                    indices[findex++] = vindex + XSeg + 2;
                    indices[findex++] = vindex + XSeg + 1;
                }
                
                // Update x and u for next vertice
                vindex++;
                x += dx;
                u -= du;
            }

            // Update y and v for next row of vertices
            y += dy;
            v += dv;
        }

        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null)
        {
            meshFilter.mesh = new Mesh();
            mesh = meshFilter.sharedMesh;
        }
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = indices;

        // Set mesh collider
        //MeshCollider meshc = GetComponent<MeshCollider>();
        //meshc.sharedMesh = mesh;
        //meshc.convex = true;

        // Set Box collider
        //BoxCollider meshc = GetComponent<BoxCollider>();
        //meshc.center = Center;
        //meshc.size = new Vector3 (1,1,0.1f);

        print(mesh.vertices.Length);
        print(mesh.vertices[2]);
        
    }
}
