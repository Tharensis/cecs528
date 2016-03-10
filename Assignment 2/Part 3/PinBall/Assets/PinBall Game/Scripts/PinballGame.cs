using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum PinballGameState {playing, won, lost};

public class PinballGame : MonoBehaviour
{
    public static PinballGame SP;

    public Transform ballPrefab;
	
	private int score;
    private PinballGameState gameState;
    private bool paused = false;
    private GameObject pauseMenu;

    private int numObjects;

    void Awake()
    {
    	// Save pause menu object and hide
		pauseMenu = GameObject.Find ("Pause Panel");
		pauseMenu.SetActive(false);

		numObjects = GameObject.FindGameObjectsWithTag ("Bumper").Length;

        SP = this;
        gameState = PinballGameState.playing;
        Time.timeScale = 1.0f;
        SpawnBall();
    }

    void Update()
    {
    	if(Input.GetKeyDown(KeyCode.Escape))
    	{
			PauseToggle ();
    	}
    }

    void SpawnBall()
    {
        Instantiate(ballPrefab, new Vector3(0f, 1.0f , 4.75f), Quaternion.identity);
    }

    void OnGUI()
    {
    
        GUILayout.Space(10);
        GUILayout.Label("  Score: " + score.ToString());

        if (gameState == PinballGameState.lost)
        {
            GUILayout.Label("You Lost!");
            if (GUILayout.Button("Try again"))
            {
                Application.LoadLevel(Application.loadedLevel);
            }
        }
        else if (gameState == PinballGameState.won)
        {
            GUILayout.Label("You won!");
            if (GUILayout.Button("Play again"))
            {
                Application.LoadLevel(Application.loadedLevel);
            }
        }
    }

    public void HitBlock()
    {
		score += 10;
    }

    public void WonGame()
    {
        Time.timeScale = 0.0f; //Pause game
        gameState = PinballGameState.won;
    }

    public void LostBall()
    {
        int ballsLeft = GameObject.FindGameObjectsWithTag("Player").Length;
        if(ballsLeft<=1)
        {
            //Was the last ball..
            SetGameOver();
        }
    }

    public void SetGameOver()
    {
        Time.timeScale = 0.0f; //Pause game
        gameState = PinballGameState.lost;
    }

    public void PauseToggle ()
	{
		if (!paused) {
				Time.timeScale = 0.0f;
				paused = !paused;
						pauseMenu.SetActive (true);
		} else {
				Time.timeScale = 1.0f;
				paused = !paused;
						pauseMenu.SetActive (false);
		}
    }

    public void DestroyedObject()
    {
		numObjects--;
		score += 100;
		if(numObjects <= 0)
		{
			WonGame ();
		}
    }
}
