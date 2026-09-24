using TMPro;
using UnityEngine;

// Presentation only: GameManager decides what to show, this decides how it reads.
public class Hud : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text livesText;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TMP_Text gameOverText;

    public void ShowScore(int score) => scoreText.text = $"Score: {score}";

    public void ShowLives(int lives) => livesText.text = $"Lives: {lives}";

    public void ShowGameOver(bool won)
    {
        gameOverText.text = (won ? "You Win!" : "Game Over") + "\nTap to Restart";
        gameOverPanel.SetActive(true);
    }

    public void HideGameOver() => gameOverPanel.SetActive(false);
}
