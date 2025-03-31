using UnityEngine;
using TMPro;
using System.Collections;

// Serializable class to store best score data
[System.Serializable]
public class BestScoreData
{
    public int score = 0; // The actual best score value
}

public class Score : MonoBehaviour
{
    // Reference to the UI text element displaying current score
    public TMP_Text scoreText;

    // Stores the best score data
    private BestScoreData bestScore = new BestScoreData();

    // Tracks the current gameplay score
    private int currentScore;

    // Key used for saving/loading best score from persistent storage
    private string bestScoreKey = "bestscoredat";

    // Initialize score at game start
    void Start()
    {
        currentScore = 0;
        UpdateScoreText();
    }

    // Load best score data when the script awakens
    private void Awake()
    {
        // Check if saved data exists before attempting to load
        if (BinaryDataStream.Exist(bestScoreKey))
        {
            StartCoroutine(ReadDataFile());
        }
    }

    // Coroutine to load best score data asynchronously
    private IEnumerator ReadDataFile()
    {
        bestScore = BinaryDataStream.Read<BestScoreData>(bestScoreKey);
        yield return new WaitForEndOfFrame(); // Wait one frame
        GameEvents.UpdateBestScoreBar(bestScore.score); // Update UI with loaded score
    }

    // Subscribe to relevant game events
    private void OnEnable()
    {
        GameEvents.AddScore += AddScore; // For adding points during gameplay
        GameEvents.GameOver += SaveBestScore; // For saving when game ends
    }

    // Unsubscribe from events when disabled
    private void OnDisable()
    {
        GameEvents.AddScore -= AddScore;
        GameEvents.GameOver -= SaveBestScore;
    }

    // Save the best score to persistent storage
    public void SaveBestScore(bool newBestScore)
    {
        BinaryDataStream.Save<BestScoreData>(bestScore, bestScoreKey);
    }

    // Add points to current score and check for new best score
    private void AddScore(int score)
    {
        currentScore += score;

        // Check if this is a new best score
        if (currentScore > bestScore.score)
        {
            bestScore.score = currentScore;
            SaveBestScore(true); // Immediately save new best score
        }

        // Update UI elements
        GameEvents.UpdateBestScoreBar(bestScore.score);
        UpdateScoreText();
    }

    // Update the score display text
    private void UpdateScoreText()
    {
        scoreText.text = currentScore.ToString();
    }
}