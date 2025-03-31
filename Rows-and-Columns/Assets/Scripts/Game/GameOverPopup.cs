using UnityEngine;

public class GameOverPopup : MonoBehaviour
{
    // Reference to the game over popup GameObject
    public GameObject gameOverPopup;

    // Initialize the popup state when the game starts
    void Start()
    {
        // Ensure the popup is hidden at game start
        gameOverPopup.SetActive(false);
    }

    // Subscribe to game over event when this script is enabled
    private void OnEnable()
    {
        GameEvents.GameOver += OnGameOver;
    }

    // Unsubscribe from game over event when this script is disabled
    private void OnDisable()
    {
        GameEvents.GameOver -= OnGameOver;
    }

    // Event handler for game over state
    private void OnGameOver(bool newbestscore)
    {
        // Show the game over popup regardless of whether it's a new best score
        gameOverPopup.SetActive(true);
    }
}