using UnityEngine;
using System.Collections;

public class MouseInterface : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown()
	{
		transform.Rotate (0, 180, 0);
	}
}
