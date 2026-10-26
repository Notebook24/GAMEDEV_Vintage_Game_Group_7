using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD Elements")]
    public TextMeshProUGUI scoreText; // Reference to the UI Text element for displaying score
    public Slider staminaBar; // Reference to the UI Slider element for displaying stamina
    public Slider flashlightBar;

    [Header("Aggression UI")]
    public TextMeshProUGUI aggressionText; // Reference to UI text element for Aggression

    public void UpdateAggressionUI(string level)
    {
        if (aggressionText != null)
        {
            aggressionText.text = "THREAT LEVEL: " + level.ToUpper();

            // Optional color coding per threat level
            switch (level.ToLower())
            {
                case "passive": aggressionText.color = Color.green; break;
                case "low": aggressionText.color = Color.yellow; break;
                case "high": aggressionText.color = new Color(1f, 0.5f, 0f); break;
                case "max": aggressionText.color = Color.red; break;
            }
        }
    }

    [Header("End Game Elements")]
    public GameObject endGamePanel; // Reference to the UI Panel for end game screen
    public TextMeshProUGUI endGameMessage; // Reference to the UI Text element for displaying end game message
    public TextMeshProUGUI finalScoreText; // NEW: Reference to final score text on the end panel

    [Header("Jumpscare Elements")]
    public GameObject jumpscareImage;
    public AudioSource jumpscareSound;

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
        // Initialize UI elements
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(false); // Hide end game panel at the start
        }
        if (jumpscareImage != null)
        {
            jumpscareImage.SetActive(false);
        }
    }

    public void UpdateScore(int score)
    {
        // Update the score display in the UI
        scoreText.text = "SCORE: " + score.ToString();
    }

    public void UpdateStamina(float stamina, float maxStamina)
    {
        // Update the stamina bar in the UI
        staminaBar.value = stamina / maxStamina;
    }

    public void UpdateFlashlightBattery(float curr, float max)
    {
        if (flashlightBar != null)
        {
            flashlightBar.value = curr / max;
        }
    }

    public void ShowEndScreenUI(bool status)
    {
        // hide all UI
        if (scoreText != null) scoreText.gameObject.SetActive(false);
        if (staminaBar != null) staminaBar.gameObject.SetActive(false);
        if (flashlightBar != null) flashlightBar.gameObject.SetActive(false);
        if (aggressionText != null) aggressionText.gameObject.SetActive(false);

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject ghost in ghosts)
        {
            // Mute all audio sources immediately
            AudioSource[] audioSources = ghost.GetComponentsInChildren<AudioSource>();
            foreach (AudioSource audio in audioSources)
            {
                audio.Stop();
                audio.mute = true;
            }

            // Hide visual renderers & disable AI
            ghost.SetActive(false);
        }

        if (finalScoreText != null && GameManager.Instance != null)
        {
            finalScoreText.text = "FINAL SCORE: " + GameManager.Instance.currentScore.ToString();
            finalScoreText.gameObject.SetActive(true);
        }

        // Show the game over UI elements
        if (endGamePanel != null && endGameMessage != null)
        {
            endGamePanel.SetActive(true);
            if (status)
            {
                // Show game win UI
                endGameMessage.text = "YOU SURVIVED!";
                endGameMessage.color = Color.white;
            }
            else
            {
                // Show game over UI
                StartCoroutine(GameOverSequence());
            }
        }
    }

    private IEnumerator GameOverSequence()
    {
        if (jumpscareImage != null && jumpscareSound != null)
        {
            jumpscareImage.SetActive(true);
            jumpscareSound.Play();
        }

        yield return new WaitForSecondsRealtime(2f);

        if (jumpscareImage != null)
        {
            jumpscareImage.SetActive(false);
        }

        if (endGamePanel != null && endGameMessage != null)
        {
            endGamePanel.SetActive(true);
            endGameMessage.text = "YOU WERE CAUGHT!";
            endGameMessage.color = Color.red; // Change text color to red for game over
        }
    }
}