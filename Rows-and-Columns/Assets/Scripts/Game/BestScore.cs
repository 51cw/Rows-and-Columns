using UnityEngine;
using TMPro;

public class BestScore : MonoBehaviour
{
    public TMP_Text bestScoreText;

    private void OnEnable()
    {
        GameEvents.UpdateBestScoreBar += UpdateBestScoreBar;
    }

    private void OnDisable()
    {
        GameEvents.UpdateBestScoreBar -= UpdateBestScoreBar;
    }

    private void UpdateBestScoreBar(int BestScore)
    {
        bestScoreText.text = BestScore.ToString();
    }
}
