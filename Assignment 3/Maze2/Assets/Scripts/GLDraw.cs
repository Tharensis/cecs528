using UnityEngine;
using System.Collections;

public class GLDraw : MonoBehaviour 
{
    static public void DrawDot(float x, float z, Color color, float r = 0.01f)
    {
        GL.Begin(GL.QUADS);
        GL.Color(color);
        GL.Vertex(new Vector3(x + r, 0, z + r));
        GL.Vertex(new Vector3(x + r, 0, z - r));
        GL.Vertex(new Vector3(x - r, 0, z - r));
        GL.Vertex(new Vector3(x - r, 0, z + r));
        GL.End();
    }

    // Draw the solution line on the maze solution path
    static public void DrawLine(float x1, float z1, float x2, float z2, 
        float sx, float sz, float vxmin, float vzmin)
    {
        // tansform the coordinate
        float sx1, sz1, sx2, sz2;

        //the route will be in the middle of the cell
        sx1 = x1 * sx + vxmin + sx / 2;
        sz1 = z1 * sz + vzmin + sz / 2;
        sx2 = x2 * sx + vxmin + sx / 2;
        sz2 = z2 * sz + vzmin + sz / 2;

        //GL.LineWidth (4);

        GL.Begin(GL.LINES);
        GL.Color(Color.blue);	    // GL.Color must be in between GL.Begin and GL.End.
        GL.Vertex3(sx1, 0, sz1);
        GL.Vertex3(sx2, 0, sz2);
        GL.End();
    }

    static public void DrawLine(Vector3 p1, Vector3 p2, Color color) // 1 ==> red, 2 ==> green, 3 ==> blue
    {
        GL.Begin(GL.LINES);
            GL.Color(color);	    // GL.Color must be in between GL.Begin and GL.End.
        GL.Vertex3(p1.x, p1.y, p1.z);
        GL.Vertex3(p2.x, p2.y, p2.z);
        GL.End();
    }
}
