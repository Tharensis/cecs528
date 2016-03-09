using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Bumper : MonoBehaviour 
{
	public AudioClip hitSound;
	
	private bool isAnimating;
	private float animTime = 0.4f;
	private float animLeft;
	
	private Light mLight;
    private Light newLight;
	
	// Use this for initialization
	void Start () 
    {
		mLight = GetComponentInChildren<Light>();
        newLight = GetComponentInChildren<Light>();
	}
	
	// Update is called once per frame
	void Update () 
    {	
		if(isAnimating)
		{
			mLight.intensity = animLeft / animTime;
			animLeft -= Time.deltaTime;
			
			if(animLeft < 0)
			{
				mLight.enabled = false;
				isAnimating = false;
			}
		}
	}
	
	void OnCollisionEnter(Collision info) 
    {
		if(info.gameObject.tag == "Player")
		{
			animLeft = animTime;
			mLight.enabled = true;
			isAnimating = true;
			GetComponent<AudioSource>().PlayOneShot (hitSound);
			PinballGame.SP.HitBlock();
		}	
	}
}
