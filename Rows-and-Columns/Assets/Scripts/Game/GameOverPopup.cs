using UnityEngine;

public class GameOverPopup : MonoBehaviour
{
    public GameObject gameOverPopup;
    public GameObject loosePopup;
    public GameObject newBestScorePopup;
    void Start()
    {
        gameOverPopup.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.GameOver += OnGameOver;
    }

    private void OnDisable()
    {
        GameEvents.GameOver -= OnGameOver;
    }

    private void OnGameOver(bool newbestscore)
    {
        gameOverPopup.SetActive(true);
        loosePopup.SetActive(false);
        newBestScorePopup.SetActive(true);
    }
}
