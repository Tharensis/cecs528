using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class Dice2 : MonoBehaviour 
{
	public Vector3 Center = Vector3.zero;
	public Vector3 Dimension = new Vector3 (1, 1, 1);
	public Texture DiceTexture;


	public void Rebuild()
	{
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		if (meshFilter == null)
		{
			Debug.LogError("MeshFilter not found!");
			return;
		}

		float w2 = Dimension.x, h2 = Dimension.y, d2 = Dimension.z;
		float cx = Center.x, cy = Center.y, cz = Center.z;
		float w = w2 / 2, h = h2 / 2, d = d2 / 2;

		Vector3[] vertices = new Vector3[24];
		Vector3[] normals = new Vector3[24]; 
		Vector2[] uvs = new Vector2[24];

		/* Front = 5 */
		float texScale = 0.33f;
		vertices[0] = new Vector3(-w + cx, -h + cy, d + cz); // bottom left corner
		normals[0]  = new Vector3(0, 0, 1);
		uvs[0]      = new Vector2(0, 0);

		vertices[1] = new Vector3(w + cx, -h + cy, d + cz); // bottom right corner
		normals[1] = new Vector3(0, 0, 1);
		uvs[1] =  new Vector2(0, texScale);

		vertices[2] = new Vector3(w + cx, h + cy, d + cz); // top right corner
		normals[2] = new Vector3(0, 0, 1);
		uvs[2] =     new Vector2(texScale, texScale);

		vertices[3] = new Vector3(-w + cx, h + cy, d + cz);  // top left corner
		normals[3] = new Vector3(0, 0, 1);
		uvs[3] =     new Vector2(0, texScale);

		/* Left = 1 */
		vertices[4] = new Vector3(w + cx, -h + cy, d + cz);
		normals[4] = new Vector3(1, 0, 0);
		uvs[4] = new Vector2(texScale, 0);

		vertices[5] = new Vector3(w + cx, -h + cy, -d + cz); 
		normals[5] = new Vector3(1, 0, 0);
		uvs[5] =     new Vector2(texScale*2, 0);

		vertices[6] = new Vector3(w + cx, h + cy, -d + cz);
		normals[6] = new Vector3(1, 0, 0);
		uvs[6] =     new Vector2(texScale*2, texScale); // 1R

		vertices[7] = new Vector3(w + cx, h + cy, d + cz); 
		normals[7] = new Vector3(1, 0, 0);
		uvs[7] =     new Vector2(texScale, texScale); // 2R

		/* Back = 2 */
		vertices[8] = new Vector3(-w + cx, h + cy, -d + cz);
		normals[8] = new Vector3(0, 0, -1);
		uvs[8] =     new Vector2(1, texScale);	// 4B

		vertices[9] = new Vector3(w + cx, h + cy, -d + cz);
		normals[9] = new Vector3(0, 0, -1);
		uvs[9] = new Vector2(texScale*2, texScale);	// 5B

		vertices[10] = new Vector3(w + cx, -h + cy, -d + cz);
		normals[10] = new Vector3(0, 0, -1);
		uvs[10] =     new Vector2(texScale*2, 0);

		vertices[11] = new Vector3(-w + cx, -h + cy, -d + cz);
		normals[11] = new Vector3(0, 0, -1);
		uvs[11] =     new Vector2(1, 0);

		/* Left = 6 */
		vertices[12] = new Vector3(-w + cx, -h + cy, -d + cz);
		normals[12] = new Vector3(-1, 0, 0);
		uvs[12] =     new Vector2(0, texScale);

		vertices[13] = new Vector3(-w + cx, -h + cy, d + cz);
		normals[13] = new Vector3(-1, 0, 0);
		uvs[13] =     new Vector2(texScale, texScale);

		vertices[14] = new Vector3(-w + cx, h + cy, d + cz);
		normals[14] = new Vector3(-1, 0, 0);
		uvs[14] =     new Vector2(texScale, texScale*2);

		vertices[15] = new Vector3(-w + cx, h + cy, -d + cz);
		normals[15] = new Vector3(-1, 0, 0);
		uvs[15] =     new Vector2(0, texScale*2);

		/* Top = 3 */
		vertices[16] = new Vector3(-w + cx, h + cy, d + cz);
		normals[16] = new Vector3(0, 1, 0);
		uvs[16] =     new Vector2(texScale, texScale);

		vertices[17] = new Vector3(w + cx, h + cy, d + cz);
		normals[17] = new Vector3(0, 1, 0);
		uvs[17] =     new Vector2(texScale*2, texScale);

		vertices[18] = new Vector3(w + cx, h + cy, -d + cz);
		normals[18] = new Vector3(0, 1, 0);
		uvs[18] =  new Vector2(texScale*2, texScale*2);

		vertices[19] = new Vector3(-w + cx, h + cy, -d + cz); 
		normals[19] = new Vector3(0, 1, 0);
		uvs[19] = new Vector2(texScale, texScale*2);

		/* Bottom = 4 */
		vertices[20] = new Vector3(-w + cx, -h + cy, -d + cz);
		normals[20] = new Vector3(0, -1, 0);
		uvs[20] =     new Vector2(texScale*2, texScale);

		vertices[21] = new Vector3(w + cx, -h + cy, -d + cz); 
		normals[21] = new Vector3(0, -1, 0);
		uvs[21] =     new Vector2(1, texScale);

		vertices[22] = new Vector3(w + cx, -h + cy, d + cz); 
		normals[22] = new Vector3(0, -1, 0);
		uvs[22] =     new Vector2(1, texScale*2);

		vertices[23] = new Vector3(-w + cx, -h + cy, d + cz);
		normals[23] = new Vector3(0, -1, 0);
		uvs[23] =     new Vector2(texScale*2, texScale*2);

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

		mesh.triangles = new int[]{
			/* Front  */ 0,1,2,      0,2,3,   
			/* Right  */ 4,5,6,      4, 6, 7,  
			/* Back   */ 8, 9, 10,   8,10,11,
			/* Left   */ 12,13,14,   12,14,15,
			/* Top    */ 16,17,18,   16,18,19,   
			/* Bottom */ 20,21,22,   20,22,23
		};

		mesh.RecalculateBounds();
		mesh.Optimize();

		// Set mesh collider
		BoxCollider meshc = GetComponent<BoxCollider>();
		meshc.center = Center;
		meshc.size = Dimension;

		// apply texture
		//		float scaleX = 2.0f;
		//		float scaleY = 2.0f;
		//		renderer.material.mainTexture =  DiceTexture;
		//		renderer.material.mainTexture.wrapMode = TextureWrapMode.Repeat;
		GetComponent<Renderer>().material.SetTexture("_MainTex", DiceTexture);
		//		renderer.material.SetTextureScale("_MainTex", new Vector2(scaleX, scaleY));
		//		renderer.material.mainTexture.wrapMode = TextureWrapMode.Repeat;

	}

}
