using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {
    public static UIManager Instance { get; private set; }

    [Header("HUD Elements")]
    public TextMeshProUGUI scoreText; // Reference to the UI Text element for displaying score
    public Slider staminaBar; // Reference to the UI Slider element for displaying stamina

    [Header("End Game Elements")]
    public GameObject endGamePanel; // Reference to the UI Panel for end game screen
    public TextMeshProUGUI endGameMessage; // Reference to the UI Text element for displaying end game message

    [Header("Jumpscare Elements")]
    public GameObject jumpscareImage;
    public AudioSource jumpscareSound;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        // Initialize UI elements
        if (endGamePanel != null) {
            endGamePanel.SetActive(false); // Hide end game panel at the start
        }
        if (jumpscareImage != null) {
                    jumpscareImage.SetActive(false);
        }
    }

    public void UpdateScore(int score) {
        // Update the score display in the UI
        scoreText.text = "SCORE: " + score.ToString();
    }

    public void UpdateStamina(float stamina, float maxStamina) {
        // Update the stamina bar in the UI
        staminaBar.value = stamina / maxStamina;
    }

    public void ShowEndScreenUI(bool status) {
        // Show the game over UI elements
        if (endGamePanel != null && endGameMessage != null) {
            endGamePanel.SetActive(true);
            if (status) {
                        // Show game win UI
                        endGameMessage.text = "YOU SURVIVED!";
                        endGameMessage.color = Color.white;
                    } else {
                        // Show game over UI
                        StartCoroutine(GameOverSequence());
                    }
        }
    }

    private IEnumerator GameOverSequence() {
        if (jumpscareImage != null && jumpscareSound != null) {
            jumpscareImage.SetActive(true);
            jumpscareSound.Play();
        }

        yield return new WaitForSecondsRealtime(2f);

        if (jumpscareImage != null) {
            jumpscareImage.SetActive(false);
        }

        if (endGamePanel != null && endGameMessage != null) {
            endGamePanel.SetActive(true);
            endGameMessage.text = "YOU WERE CAUGHT!";
            endGameMessage.color = Color.red; // Change text color to red for game over
        }
    }
}