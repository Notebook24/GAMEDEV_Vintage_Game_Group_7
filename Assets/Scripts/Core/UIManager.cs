using UnityEngine;

public class UIManager : MonoBehaviour {
    public static UIManager Instance { get; private set; }

    // To be updated
    public GameObject scoreText; // Reference to the UI Text element for displaying score
    public GameObject staminaBar; // Reference to the UI element for displaying stamina

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
    }

    public void UpdateScore(int score) {
        // Update the score display in the UI
        scoreText.text = "Score: " + score.ToString();
    }

    public void UpdateStamina(float stamina, float maxStamina) {
        // Update the stamina bar in the UI
        staminaBar.value = stamina / maxStamina;
    }

    public void ShowEndScreenUI(bool status) {
        // Show the game over UI elements
        if (status) {
            // Show game win UI
        } else {
            // Show game over UI
        }
    }
}