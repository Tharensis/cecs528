using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class Cylinder : MonoBehaviour 
{
    public float Radius = 1.0f;
    public float Height = 2.0f;
    public int Nsides = 32;
    public bool Top = true;
    public bool Bottom = true;
   
    public void Rebuild()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter not found!");
            return;
        }

        Vector3[] vertices;
        Vector3[] normals;
        Vector2[] uvs;
        int[] indices;

        if (Nsides <= 0) Nsides = 32;

        if (Top && Bottom)
        {
            vertices = new Vector3[2 * (Nsides + 1) + 2 * (Nsides + 1) + 2];
            normals = new Vector3[2 * (Nsides + 1) + 2 * (Nsides + 1) + 2];
            uvs = new Vector2[2 * (Nsides + 1) + 2 * (Nsides + 1) + 2];
            indices = new int[3 * 2 * Nsides * Nsides + 3 * Nsides + 3 * Nsides];
        }
        else if (Top || Bottom)
        {
            vertices = new Vector3[2 * (Nsides + 1) + (Nsides + 1) + 1];
            normals = new Vector3[2 * (Nsides + 1) + (Nsides + 1) + 1];
            uvs = new Vector2[2 * (Nsides + 1) + (Nsides + 1) + 1];
            indices = new int[3 * 2 * Nsides * Nsides + 3 * Nsides];
        }
        else
        {
            vertices = new Vector3[2 * (Nsides + 1)];
            normals = new Vector3[2 * (Nsides + 1)];
            uvs = new Vector2[2 * (Nsides + 1)];
            indices = new int[3 * 2 * Nsides * Nsides];
        }

        float inc, angle, texinc;
        float yTop, yBot;

        yTop = Height / 2.0f;
        yBot = -yTop;
        inc = 2.0f * Mathf.PI / Nsides;
        texinc = 1.0f / Nsides; // Texture coordinate increment

        // Initialize vertice data
        Vector3 p1 = Vector3.zero, p2 = Vector3.zero, n = Vector3.zero;
        Vector2 t1, t2;
        p1.y = yBot;
        p1.z = p2.z = Radius;
        p2.y = yTop;
        n.z = 1.0f;
        t1.x = 0.0f; t1.y = 1.0f;
        t2.x = 0.0f; t2.y = 0.0f;
        int vert = 0, fvert = 0, refVert;

        // Make first two vertices
        vertices[vert] = p1;
        normals[vert] = n;
        uvs[vert++] = t1;
        vertices[vert] = p2;
         normals[vert] = n;
        uvs[vert++] = t2;

        angle = inc;
        t1.x = t2.x = texinc;

        for (int i = 1; i < Nsides; ++i)
        {
            n.x = Mathf.Sin(angle);
            n.z = Mathf.Cos(angle);
            p1.x = p2.x = Radius * n.x;
            p1.z = p2.z = Radius * n.z;

            // Make side mesh
            refVert = vert - 2; // the first vertex in the next triangle faces
            vertices[vert] = p1;
            normals[vert] = n;
            uvs[vert++] = t1;

            vertices[vert] = p2;
            normals[vert] = n;
            uvs[vert++] = t2;

            indices[fvert++] = refVert;
            indices[fvert++] = refVert + 2;
            indices[fvert++] = refVert + 3;
            indices[fvert++] = refVert;
            indices[fvert++] = refVert + 3;
            indices[fvert++] = refVert + 1;

            angle += inc;
            t1.x += texinc;
            t2.x += texinc;
        }

        // Make last two vertices
        p1.x = p2.x = 0.0f;
        p1.z = p2.z = Radius;
        n.x = 0.0f;
        n.z = Radius;
        t1.x = t2.x = 1.0f;

        // Make last side mesh
        refVert = vert - 2;
        vertices[vert] = p1;
        normals[vert] = n;
        uvs[vert++] = t1;

        vertices[vert] = p2;
        normals[vert] = n;
        uvs[vert++] = t2;

        indices[fvert++] = refVert;
        indices[fvert++] = refVert + 2;
        indices[fvert++] = refVert + 3;
        indices[fvert++] = refVert;
        indices[fvert++] = refVert + 3;
        indices[fvert++] = refVert + 1;

        if (Top)
            MakeCap(Nsides, vertices, normals, uvs, indices, yTop, Radius, true, ref vert, ref fvert);
        if (Bottom)
            MakeCap(Nsides, vertices, normals, uvs, indices, yBot, Radius, false, ref vert, ref fvert);


        // Reset the mesh
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

        mesh.RecalculateBounds();
        mesh.Optimize();

        // Set mesh collider
        MeshCollider meshc = GetComponent<MeshCollider>();
        meshc.sharedMesh = mesh;
        meshc.convex = true;

    }

    private void MakeCap(int sides, Vector3[] vertices, Vector3[] normals, Vector2[] uvs, int[] indices,
            float y, float radius, bool up, ref int vert, ref int fvert)
    {
        float inc, angle, texinc;
        inc = 2.0f * Mathf.PI / sides;
        texinc = 1.0f / sides; // Texture coordinate increment

        Vector3 center = Vector3.zero, p = Vector3.zero, n = Vector3.zero;
        Vector2 t;
        center.y = p.y = y;
        p.z = radius;
        if (up)
            n.y = 1.0f;
        else
            n.y = -1.0f;
        t.x = 0.0f; t.y = 1.0f;
        int cindex;

        cindex = vert;
        vertices[vert] = center;
        normals[vert] = n;
        uvs[vert++] = new Vector2(0.5f, 0.5f);
        vertices[vert] = p;
        normals[vert] = n;
        uvs[vert++] = t;

        angle = inc;

        for (int i = 1; i < sides; ++i)
        {
            t.x = Mathf.Sin(angle);
            t.y = Mathf.Cos(angle);
            p.x = radius * t.x;
            p.z = radius * t.y;

            vertices[vert] = p;
            normals[vert] = n;
            uvs[vert++] = t;

            indices[fvert++] = cindex;
            if (up)
            {
                indices[fvert++] = vert - 2;
                indices[fvert++] = vert - 1;
            }
            else
            {
                indices[fvert++] = vert - 1;
                indices[fvert++] = vert - 2;
            }

            angle += inc;
        }

        // Make last triangle
        p.x = 0.0f;
        p.z = radius;
        t.x = 0.0f;
        t.y = 1.0f;

        vertices[vert] = p;
        normals[vert] = n;
        uvs[vert++] = t;

        indices[fvert++] = cindex;
        if (up)
        {
            indices[fvert++] = vert - 2;
            indices[fvert++] = vert - 1;
        }
        else
        {
            indices[fvert++] = vert - 1;
            indices[fvert++] = vert - 2;
        }

    }



	
}
