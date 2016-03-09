using UnityEngine;
using System.Collections;

public class Acceleration : MonoBehaviour 
{
    public float Speed = 10.0f;
    Vector3 dir = Vector3.zero;

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), "x: " + dir.x.ToString());
        GUI.Label(new Rect(10, 30, 100, 20), "y: " + dir.y.ToString());
        GUI.Label(new Rect(10, 50, 100, 20), "z: " + dir.z.ToString());
    }

	// Update is called once per frame
	void Update () 
    {
        if (Mathf.Abs(Input.acceleration.x) > 1)
            dir.x = Input.acceleration.x;
        if (Mathf.Abs(Input.acceleration.y) > 1)
            dir.y = Input.acceleration.y;
        if (Mathf.Abs(Input.acceleration.z) > 1)
            dir.z = Input.acceleration.z;
	}
}
