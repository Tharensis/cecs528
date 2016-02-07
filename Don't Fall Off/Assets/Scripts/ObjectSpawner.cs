using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] items;                // The enemy prefab to be spawned.
    public float spawnTime = 3f;            // How long between each spawn.
    public float destroyTime = 5f;
    //public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.


    void Start ()
    {
        // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
        InvokeRepeating ("Spawn", spawnTime, spawnTime);
    }


    void Spawn ()
    {
        // Find a random index between zero and one less than the number of spawn points.
        int itemIndex = Random.Range (0, items.Length);

		Object newObject = Instantiate (items [itemIndex], new Vector2 (0, 2), Quaternion.identity);
		Destroy (newObject, destroyTime);

        // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
        //Instantiate (enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
    }
}