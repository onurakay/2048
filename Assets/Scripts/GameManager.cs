using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] GameBoard gameBoard;
    [SerializeField] CanvasGroup gameOver;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI highScoreText;

    public int score { get; private set; } = 0;

    void Start()
    {
        NewGame();
    }

    public void NewGame()
    {
        SetScore(0);
        highScoreText.text = LoadHighScore().ToString();
        gameOver.alpha = 0f;
        gameOver.interactable = false;

        gameBoard.ClearBoard();
        gameBoard.GenerateTile();
        gameBoard.GenerateTile();
        gameBoard.enabled = true;
    }

    public void GameOver()
    {
        gameBoard.enabled = false;
        gameOver.interactable = true;

        StartCoroutine(Fade(gameOver, 1f, 1f));
    }

    public void IncreaseScore(int points)
    {
        SetScore(score + points);
    }

    #region Score Management

    void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString();

        SaveHighScore();
    }

    void SaveHighScore()
    {
        int hiscore = LoadHighScore();

        if (score > hiscore) {
            PlayerPrefs.SetInt("hiscore", score);
        }
    }

    int LoadHighScore()
    {
        return PlayerPrefs.GetInt("hiscore", 0);
    }

    #endregion

    IEnumerator Fade(CanvasGroup canvasGroup, float targetAlpha, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float duration = 0.5f;
        float startAlpha = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}
