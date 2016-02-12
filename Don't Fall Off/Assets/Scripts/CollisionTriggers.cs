using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CollisionTriggers : MonoBehaviour {

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.CompareTag("Object")) 
		{
			Destroy (other.gameObject);
		} else if(other.gameObject.CompareTag("Player"))
		{
			other.gameObject.GetComponent<AudioSource> ().Play ();
			GameObject.Find ("GameManager").GetComponent<GameManager> ().endGame ();
		} else {
			print (other.gameObject.tag);
		}
	}
}
