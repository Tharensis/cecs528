using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Bumper : MonoBehaviour 
{
	public AudioClip[] hitSounds;
	public AudioClip destroySound;
	public int maxHits;
	
	private bool isAnimating;
	private float animTime = 0.4f;
	private float animLeft;
	private int hitsLeft;
	
	private Light mLight;
	
	// Use this for initialization
	void Start () 
    {
		mLight = GetComponentInChildren<Light>();
		hitsLeft = maxHits;
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
			
			if(--hitsLeft <= 0)
			{
				GetComponent<AudioSource> ().PlayOneShot (destroySound);
				gameObject.GetComponent<MeshRenderer> ().enabled = false;
				gameObject.GetComponent<CapsuleCollider> ().enabled = false;
				info.gameObject.GetComponent<Rigidbody> ().velocity *= (float)1.2;
				PinballGame.SP.DestroyedObject ();
			} 
			else
			{
				animLeft = animTime;
				mLight.enabled = true;
				isAnimating = true;
				GetComponent<AudioSource>().PlayOneShot (hitSounds[Random.Range(0, 3)]);
				PinballGame.SP.HitBlock();
			}
		}	
	}
}
