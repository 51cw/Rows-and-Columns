using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public TMP_Text scoreText;
    private int currentScore;
    void Start()
    {
        currentScore = 0;
        UpdateScoreText();
    }

    private void OnEnable()
    {
        GameEvents.AddScore += AddScore;
    }

    private void OnDisable()
    {
        GameEvents.AddScore -= AddScore;
    }

    private void AddScore(int score)
    {
        currentScore += score;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = currentScore.ToString();
    }
}
