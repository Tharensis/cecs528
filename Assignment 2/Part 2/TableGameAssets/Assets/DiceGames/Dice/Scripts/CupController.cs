using UnityEngine;
using System.Collections;

public class CupController : MonoBehaviour {

	private Animator animator;

	private Vector3 screenPoint;
	private Vector3 offset;

	private int rotateStateID;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
		{
			//StartCoroutine (PlayOneShot ("rotate"));
			animator.SetTrigger ("rotateTrigger");
		}
	}
		
	void OnMouseDown(){
		screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
	}
		
	void OnMouseDrag(){
		Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + offset;
		transform.position = cursorPosition;
	}

	public IEnumerator PlayOneShot(string paramName)
	{
		print ("Playing");
		animator.SetBool (paramName, true);
		yield return null;
		animator.SetBool (paramName, false);
		print ("Stopped");
	}
}
