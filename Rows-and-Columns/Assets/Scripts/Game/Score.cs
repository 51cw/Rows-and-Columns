using UnityEngine;
using TMPro;
using System.Collections;

[System.Serializable]
public class BestScoreData
{
    public int score = 0;
}
public class Score : MonoBehaviour
{
    public TMP_Text scoreText;
    private BestScoreData bestScore = new BestScoreData();
    private int currentScore;
    private string bestScoreKey = "bestscoredat";
    void Start()
    {
        currentScore = 0;
        UpdateScoreText();
    }

    private void Awake()
    {
        if (BinaryDataStream.Exist(bestScoreKey))
        {
            StartCoroutine(ReadDataFile());
        }
    }

    private IEnumerator ReadDataFile()
    {
        bestScore = BinaryDataStream.Read<BestScoreData>(bestScoreKey);
        yield return new WaitForEndOfFrame();
        GameEvents.UpdateBestScoreBar(bestScore.score);
    }

    private void OnEnable()
    {
        GameEvents.AddScore += AddScore;
        GameEvents.GameOver += SaveBestScore;
    }

    private void OnDisable()
    {
        GameEvents.AddScore -= AddScore;
        GameEvents.GameOver -= SaveBestScore;
    }

    public void SaveBestScore(bool newBestScore)
    {
        BinaryDataStream.Save<BestScoreData>(bestScore, bestScoreKey);
    }
    private void AddScore(int score)
    {
        currentScore += score;
        if(currentScore > bestScore.score)
        {
            bestScore.score = currentScore;
            SaveBestScore(true);
        }

        GameEvents.UpdateBestScoreBar(bestScore.score);
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = currentScore.ToString();
    }
}
