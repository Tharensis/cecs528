using UnityEngine;
using System.Collections;

public enum PinballGameState {playing, won, lost};

public class PinballGame : MonoBehaviour
{
    public static PinballGame SP;

    public Transform ballPrefab;
	
	private int score;
    private PinballGameState gameState;

    void Awake()
    {
        SP = this;
        gameState = PinballGameState.playing;
        Time.timeScale = 1.0f;
        SpawnBall();
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
}
