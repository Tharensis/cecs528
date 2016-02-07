using UnityEngine;
using System.Collections;

public class ObjectController : MonoBehaviour {

	public float speed;

	private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		rb.AddForce (new Vector2 (0, 0));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
