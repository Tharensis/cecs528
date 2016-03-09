using UnityEngine;
using System.Collections;


[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]


public class Sphere : MonoBehaviour 
{
    public float Radius = 1.0f;
    public int Nsides = 16;
    public int Nrings = 16;

    public void Rebuild()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter not found!");
            return;
        }


        if (Nsides <= 0) Nsides = 16;
        if (Nrings <= 0) Nrings = 16;

        Vector3[] vertices = new Vector3[Nsides * Nrings];
        Vector3[] normals = new Vector3[Nsides * Nrings];
        Vector2[] uvs = new Vector2[Nsides * Nrings];
        int[] indices = new int[6 * Nsides * Nrings];

        float theta;	// theta is the complement of the latitude (co-latitude)
        float phi;	    // phi is the longitude
        float dtheta = Mathf.PI / (Nrings - 1);
        float dphi = 2.0f * Mathf.PI / (Nsides - 1);
        Vector3 v, n;
        Vector2 t;
        float temp;
        int vert = 0;
        int fvert = 0;

        int vertCnt = 0;
        for (int x = 0; x < Nsides; x++)
        {
            phi = x * dphi;
            for (int y = 0; y < Nrings; y++)
            {
                theta = y * dtheta;
                temp = Mathf.Sin(theta);

                n.x = temp * Mathf.Cos(phi);
                n.y = temp * Mathf.Sin(phi);
                n.z = Mathf.Cos(theta);           // north pole : Z = r

                v = Radius * n;

                t.x = 1.0f - (float)x / (Nsides - 1);
                t.y = (float)y / (Nrings - 1);

                vertices[vert] = v;
                normals[vert] = n;
                uvs[vert++] = t;

                if (x < Nsides - 1 && y < Nrings - 1)
                {
                    indices[fvert++] = (vertCnt + 0);
                    indices[fvert++] = (vertCnt + 1);
                    indices[fvert++] = (vertCnt + 1 + Nrings);
                    indices[fvert++] = (vertCnt + 1 + Nrings);
                    indices[fvert++] = (vertCnt + Nrings);
                    indices[fvert++] = (vertCnt + 0);
                }
                vertCnt++;
            }


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
            computeTangent(mesh);

            mesh.RecalculateBounds();
            mesh.Optimize();

            // Set mesh collider
            SphereCollider meshc = GetComponent<SphereCollider>();
            meshc.radius = Radius;         
        }

    }

    
    void computeTangent (Mesh mesh)
    {

        int triangleCount = mesh.triangles.Length / 3;
        int vertexCount = mesh.vertices.Length;

        Vector3[] tan1 = new Vector3[vertexCount];
        Vector3[] tan2 = new Vector3[vertexCount];

        Vector4[] tangents = new Vector4[vertexCount];

        for (long a = 0; a < triangleCount; a += 3)
        {
            long i1 = mesh.triangles[a + 0];
            long i2 = mesh.triangles[a + 1];
            long i3 = mesh.triangles[a + 2];

            Vector3 v1 = mesh.vertices[i1];
            Vector3 v2 = mesh.vertices[i2];
            Vector3 v3 = mesh.vertices[i3];

            Vector2 w1 = mesh.uv[i1];
            Vector2 w2 = mesh.uv[i2];
            Vector2 w3 = mesh.uv[i3];

            float x1 = v2.x - v1.x;
            float x2 = v3.x - v1.x;
            float y1 = v2.y - v1.y;
            float y2 = v3.y - v1.y;
            float z1 = v2.z - v1.z;
            float z2 = v3.z - v1.z;

            float s1 = w2.x - w1.x;
            float s2 = w3.x - w1.x;
            float t1 = w2.y - w1.y;
            float t2 = w3.y - w1.y;

            float r = 1.0f / (s1 * t2 - s2 * t1);

            Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

            tan1[i1] += sdir;
            tan1[i2] += sdir;
            tan1[i3] += sdir;

            tan2[i1] += tdir;
            tan2[i2] += tdir;
            tan2[i3] += tdir;
        }


        for (long a = 0; a < vertexCount; ++a)
        {
            Vector3 n = mesh.normals[a];
            Vector3 t = tan1[a];

            Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
            tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);

            tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
        }

        mesh.tangents = tangents;

    }
}
