using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour 
{
    public float YRotSpeed = 20.0f;

	// Use this for initialization
	void Start () 
    {	
	}
	
	// Update is called once per frame
	void Update () 
    {
        transform.Rotate(0, YRotSpeed * Time.deltaTime, 0);
	
	}
}
