using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public int currentScore = 0;
    public int dotsCollected = 0;
    public int totalDots = 0;

    public bool isGameActive = true;

    // Singleton pattern to ensure only one instance of GameManager exists
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize game state
        totalDots = GameObject.FindGameObjectsWithTag("Dot").Length; isGameActive = true;

        UIManager.Instance.UpdateScore(currentScore);
    }

    public void AddScore(int pts)
    {
        // Add score to the player's total score
        if (!isGameActive)
            return;

        currentScore += pts;
        dotsCollected++;

        //Update the UI with the new score
        UIManager.Instance.UpdateScore(currentScore);

        // Check for win condition which is when dots are all collected
        if (dotsCollected >= totalDots)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        // Handle game over logic
        if (!isGameActive)
            return;
        //Set the game state to inactive
        isGameActive = false;
        //Show UI for game over
        UIManager.Instance.ShowEndScreenUI(false);
        //Stop the gameplay
        Time.timeScale = 0f;
    }

    public void GameWon()
    {
        // Handle game won logic
        if (!isGameActive)
            return;
        //Set the game state to inactive
        isGameActive = false;
        //Show UI for winning the game
        UIManager.Instance.ShowEndScreenUI(true);
        //Stop the gameplay
        Time.timeScale = 0f;
    }
}