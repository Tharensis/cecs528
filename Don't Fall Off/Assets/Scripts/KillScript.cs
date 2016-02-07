using UnityEngine;
using System.Collections;

public class KillScript : MonoBehaviour {

	void OnCollisionEnter2D(Collision2D other)
	{
		Destroy (other.gameObject);
	}
}
