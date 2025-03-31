using UnityEngine;
using TMPro;

public class BestScore : MonoBehaviour
{
    // Reference to the TextMeshPro UI element that displays the best score
    public TMP_Text bestScoreText;

    // Subscribe to the UpdateBestScoreBar event when this script is enabled
    private void OnEnable()
    {
        GameEvents.UpdateBestScoreBar += UpdateBestScoreBar;
    }

    // Unsubscribe from the event when this script is disabled
    private void OnDisable()
    {
        GameEvents.UpdateBestScoreBar -= UpdateBestScoreBar;
    }

    // Event handler that updates the best score display
    private void UpdateBestScoreBar(int BestScore)
    {
        // Convert the best score integer to a string and update the UI text
        bestScoreText.text = BestScore.ToString();
    }
}