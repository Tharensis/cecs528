﻿using UnityEngine;
using System.Collections;

public class NavControl : MonoBehaviour 
{

	Animator animator; 			// var to store the animator component
	public float h; 			// variable to hold user horizontal input, turns
	public float v; 					// variable to hold user vertical input, forward/backward
	public float rotVSpeed  = 90.0f; 	//rotation speed

	static int runState = Animator.StringToHash("Base Layer.Run");

	Rigidbody rb;

	public float MaxSpeed = 2.0f;

	// Use this for initialization
	void Start () 
	{
		animator = GetComponent<Animator>(); // assign the Animator component

		if(animator && animator.layerCount > 1)
			animator.SetLayerWeight(1,1.0f); // set layer 1's weight to 1

		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Get Input each frame and assign it to the variables
		h = Input.GetAxis("Horizontal");
		v = Input.GetAxis("Vertical");	

		if (animator) // if there is an animator compomonent
		{
			// if the fire button was pressed, set the Wave parameter to true
			if (Input.GetButtonDown ("Fire1"))
				animator.SetBool ("Wave", true);
			else
				animator.SetBool ("Wave", false);

			if (Input.GetKey (KeyCode.LeftShift))
				animator.SetBool ("Run", true);
			else
				animator.SetBool ("Run", false);
		}
	}

	void FixedUpdate ()
	{
		AnimatorStateInfo currentBaseState = animator.GetCurrentAnimatorStateInfo (0);

		// Set V Input and Direction Parameters to H and V axes
		animator.SetFloat ("V_Input", v);
		animator.SetFloat ("Direction", h); 
		//animator.SetFloat("Direction", Mathf.Abs(h)); // scripted single input turn

		// rotate the character according to input and rotation speed, while walking forward
		if (animator.GetFloat ("V_Input") > 0.1)
			transform.Rotate (new Vector3 (0, h * Time.deltaTime * rotVSpeed, 0));

		Vector3 dir = new Vector3 (rb.velocity.x, 0, v * MaxSpeed);

		//rb.velocity = transform.TransformDirection (dir);

		if (animator.GetFloat ("V_Input") > 0.1) {
			//rb.AddForce (transform.TransformDirection (dir));
			rb.AddForce (transform.forward);
		}

		if (currentBaseState.nameHash == runState) {
			float x = rb.velocity.x;
			float y = rb.velocity.y;
			//rb.velocity = new Vector3 (rb.velocity.x, 0, rb.velocity.z);
			//rb.velocity = transform.TransformDirection (dir) * 5;
			//rb.AddForce (transform.TransformDirection (dir) * 100);
			rb.AddForce (transform.forward * 500);

			//print ("Weeeee!" + rb.velocity);
			print (dir);
			print (transform.rotation);
		} else {
			//print (rb.velocity);
			print (dir);
		}
	}
}
