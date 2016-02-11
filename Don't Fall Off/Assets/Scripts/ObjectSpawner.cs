using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] items;                // The enemy prefab to be spawned.
    public float spawnTime = 3f;            // How long between each spawn.
    public float destroyTime = 5f;
    public float velocity = 1f;
    public bool spawning = true;

    private float camWidth;
    private float camHeight;

    void Start ()
    {
        // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
    }


    void Spawn ()
	{
		if(!spawning)
		{
			return;
		}

		float spawnX;
		float spawnY;

		// Find camera bounds
		Vector2 camPosition = (Vector2)Camera.main.GetComponent<Camera> ().transform.position;
		camWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
		camHeight = Camera.main.orthographicSize;
		//print (camPosition + " " + camWidth + " " + camHeight);

		// Determine where to spawn object
		if (Random.value > 0.7) {
			// Spawn on right or left
			if(Random.value > 0.5) {
				// Spawn on right
				spawnX = camPosition.x + camWidth;
				spawnY = Random.Range (camPosition.y, camPosition.y + camHeight);
			} else {
				// Spawn on left
				spawnX = camPosition.x - camWidth;
				spawnY = Random.Range (camPosition.y, camPosition.y + camHeight);
			}
		} else {
			spawnX = Random.Range (camPosition.x - camWidth, camPosition.y + camWidth);
			spawnY = camPosition.y + camHeight;
		}

		// Spawn object and set velocity
		int itemIndex = Random.Range (0, items.Length);
		GameObject newObject = (GameObject)Instantiate (items [itemIndex], new Vector2 (spawnX, spawnY), Quaternion.identity);
		newObject.transform.parent = GameObject.Find ("ObjectHolder").transform;

		// Determine velocity of new object
		Vector2 player = (Vector2)GameObject.FindGameObjectWithTag ("Player").transform.position;
		Vector2 objectPosition = (Vector2)newObject.transform.position;
		newObject.GetComponent<Rigidbody2D> ().velocity = (player - objectPosition) * velocity;
		newObject.GetComponent<Rigidbody2D> ().angularVelocity = 1000 * Random.Range(-1.0f, 1.0f);

		newObject.GetComponent<AudioSource> ().Play ();

		// Destroy object in destroyTime seconds
		Destroy (newObject, destroyTime);
    }

    public void beginSpawning ()
	{
		print (spawnTime);
		InvokeRepeating ("Spawn", spawnTime, spawnTime);
	}

	public void stopSpawning()
	{
		CancelInvoke ();
	}
}