using UnityEngine;
using UnityEngine.SceneManagement;

// Owns the rules and the flow: launch, score, lives, win/lose, restart. Listens to the
// playfield rather than being called by every brick and the ball.
public class GameManager : MonoBehaviour
{
    [SerializeField] int startingLives = 3;
    [SerializeField] BrickGrid bricks;
    [SerializeField] BallController ball;
    [SerializeField] DeathZone deathZone;
    [SerializeField] Hud hud;

    int score;
    int lives;
    bool gameOver;
    bool restartArmed;

    void OnEnable()
    {
        bricks.BrickBroken += OnBrickBroken;
        deathZone.BallEntered += OnBallLost;
    }

    void OnDisable()
    {
        bricks.BrickBroken -= OnBrickBroken;
        deathZone.BallEntered -= OnBallLost;
    }

    void Start()
    {
        Time.timeScale = 1f; // undo the previous round's EndGame freeze after a reload
        lives = startingLives;
        hud.ShowScore(score);
        hud.ShowLives(lives);
        hud.HideGameOver();
    }

    void Update()
    {
        if (!gameOver)
        {
            if (ball.IsHeld && GameInput.LaunchTriggered) ball.Launch();
            return;
        }

        // A tap only restarts if it began on the game-over screen, so a finger that was
        // still dragging the paddle when the round ended can't skip straight past it.
        if (GameInput.PointerPressed) restartArmed = true;
        if (GameInput.AnyKeyPressed || (restartArmed && GameInput.PointerReleased))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnBrickBroken(Brick brick)
    {
        score += brick.Points;
        hud.ShowScore(score);
        if (bricks.Remaining == 0) EndGame(won: true);
    }

    void OnBallLost(BallController lostBall)
    {
        lives--;
        hud.ShowLives(lives);
        lostBall.ResetToPaddle();
        if (lives <= 0) EndGame(won: false);
    }

    void EndGame(bool won)
    {
        gameOver = true;
        Time.timeScale = 0f;
        hud.ShowGameOver(won);
    }
}
