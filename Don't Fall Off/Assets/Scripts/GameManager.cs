using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public int level = 1;
	public float maxLevelTime = 20f;
	public float levelStartTime = 2f;

	private float timeLeft;
	private bool gamePlaying = false;
	private GameObject levelText;
	private Text timeText;
	ObjectSpawner spawner;

	// Use this for initialization
	void Start () 
	{
		spawner = GameObject.Find ("Object Spawner").GetComponent<ObjectSpawner>();
		spawner.stopSpawning ();
		timeText = GameObject.Find("TimeText").GetComponent<Text>();
		levelText = GameObject.Find ("LevelText");

		beginLevel ();
	}

	void beginLevel()
	{
		// Destroy all thrown objects if they exist
		foreach (Transform child in GameObject.Find("ObjectHolder").transform)
		{
			Destroy (child.gameObject);
		}

		// Reset time left
		timeLeft = maxLevelTime;

		// Display level text
		levelText.GetComponent<Text> ().text = "Level: " + level;
		levelText.SetActive (true);

		// Set init time text
		timeText.text = "Time Left: " + Mathf.Round(timeLeft);

		// Activate object thrower after delay
		Invoke ("activateSpawner", levelStartTime);

	}

	// Update is called once per frame
	void Update () {
		if (gamePlaying)
		{
			updateTime();
			if(timeLeft <= 0)
			{
				gamePlaying = false;
				finishLevel ();
			}
		}
	}

	void finishLevel ()
	{
		spawner.stopSpawning ();
		level++;
		beginLevel ();
	}

	private void updateTime()
	{
		timeLeft -= Time.deltaTime;

		// Display new time text
		timeText.text = "Time Left: " + timeLeft.ToString("F2");
	}

	// Update rate of objects thrown based on level
	public void activateSpawner()
	{
		levelText.GetComponent<Text> ().text = "GO!";
		Invoke ("disableLevelText", 2);
		gamePlaying = true;
		spawner.spawnTime = (float)2 / level;
		spawner.beginSpawning ();
		// Should start timer as well
	}

	public void disableLevelText()
	{
		levelText.SetActive (false);
	}

	public void endGame()
	{
		//GameObject.FindGameObjectWithTag ("Player").transform.position = new Vector2 (0, 0);
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		return;
	}
}
