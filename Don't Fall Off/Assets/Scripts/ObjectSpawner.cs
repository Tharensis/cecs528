using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] items;                // The enemy prefab to be spawned.
    public float spawnTime = 3f;            // How long between each spawn.
    public float destroyTime = 5f;
    public float initVelocity = 10f;

    private float camWidth;
    private float camHeight;

    void Start ()
    {
        // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
        InvokeRepeating ("Spawn", spawnTime, spawnTime);
    }


    void Spawn ()
	{
		float spawnX;
		float spawnY;

		// Find camera bounds
		Vector2 camPosition = (Vector2)Camera.main.GetComponent<Camera> ().transform.position;
		camWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
		camHeight = Camera.main.orthographicSize;
		//print (camPosition + " " + camWidth + " " + camHeight);

		// Determine where to spawn object
		if (Random.value > 0.5) {
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
		newObject.GetComponent<Rigidbody2D> ().velocity = new Vector2 (initVelocity, 0);

		// Determine velocity of new object
		Vector2 player = (Vector2)GameObject.FindGameObjectWithTag ("Player").transform.position;
		Vector2 objectPosition = (Vector2)newObject.transform.position;
		newObject.GetComponent<Rigidbody2D> ().velocity = player - objectPosition;
		newObject.GetComponent<Rigidbody2D> ().angularVelocity = 1000 * Random.Range(-1.0f, 1.0f);

		// Destroy object in destroyTime seconds
		Destroy (newObject, destroyTime);
    }
}