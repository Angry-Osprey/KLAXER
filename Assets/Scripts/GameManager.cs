using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Owns the rules and the flow: launch, score, lives, power-ups, win/lose, restart. Listens
// to the playfield rather than being called by every brick and the ball.
public class GameManager : MonoBehaviour
{
    [SerializeField] int startingLives = 3;
    [SerializeField] BrickGrid bricks;
    [SerializeField] BallController ball;
    [SerializeField] PaddleController paddle;
    [SerializeField] DeathZone deathZone;
    [SerializeField] PowerUpDropper powerUps;
    [SerializeField] Hud hud;

    [Header("Power-ups")]
    [SerializeField] float widePaddleScale = 1.5f;
    [SerializeField] float widePaddleSeconds = 10f;
    [SerializeField] float slowBallScale = 0.65f;
    [SerializeField] float slowBallSeconds = 10f;
    [SerializeField, Min(1)] int multiBallExtraBalls = 2;
    [Tooltip("Multi-ball never takes the number of balls in play above this.")]
    [SerializeField, Min(1)] int maxBalls = 8;

    readonly List<BallController> balls = new List<BallController>();
    int score;
    int lives;
    bool gameOver;
    bool restartArmed;
    float ballSpeedScale = 1f;
    float slowEndsAt = -1f;

    void Awake()
    {
        balls.Add(ball);
    }

    void OnEnable()
    {
        bricks.BrickBroken += OnBrickBroken;
        deathZone.BallEntered += OnBallLost;
        powerUps.Collected += OnPowerUpCollected;
    }

    void OnDisable()
    {
        bricks.BrickBroken -= OnBrickBroken;
        deathZone.BallEntered -= OnBallLost;
        powerUps.Collected -= OnPowerUpCollected;
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
            if (GameInput.LaunchTriggered)
                foreach (BallController b in balls)
                    if (b.IsHeld) b.Launch();

            if (slowEndsAt >= 0f && Time.time >= slowEndsAt)
            {
                slowEndsAt = -1f;
                SetBallSpeedScale(1f);
            }
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
        // Extra balls from multi-ball just disappear; only the last ball costs a life.
        if (balls.Count > 1)
        {
            balls.Remove(lostBall);
            Destroy(lostBall.gameObject);
            return;
        }

        lives--;
        hud.ShowLives(lives);
        lostBall.ResetToPaddle();
        if (lives <= 0) EndGame(won: false);
    }

    void OnPowerUpCollected(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.WidePaddle:
                paddle.Widen(widePaddleScale, widePaddleSeconds);
                break;
            case PowerUpType.MultiBall:
                SpawnExtraBalls();
                break;
            case PowerUpType.SlowBall:
                SetBallSpeedScale(slowBallScale);
                slowEndsAt = Time.time + slowBallSeconds;
                break;
            case PowerUpType.ExtraLife:
                lives++;
                hud.ShowLives(lives);
                break;
        }
    }

    // Splits new balls off one already in flight (or straight up off the paddle if none is),
    // fanned out either side of its direction.
    void SpawnExtraBalls()
    {
        BallController source = balls.Find(b => !b.IsHeld) ?? balls[0];
        Vector2 baseDirection = source.IsHeld ? Vector2.up : source.Direction;
        Vector2 position = source.transform.position;

        for (int i = 0; i < multiBallExtraBalls && balls.Count < maxBalls; i++)
        {
            float angle = (i % 2 == 0 ? 25f : -25f) * (i / 2 + 1);
            BallController extra = Instantiate(source, position, Quaternion.identity);
            extra.SpeedScale = ballSpeedScale;
            extra.LaunchFrom(position, Quaternion.Euler(0f, 0f, angle) * baseDirection);
            foreach (BallController other in balls)
                Physics2D.IgnoreCollision(extra.Collider, other.Collider); // balls pass through each other
            balls.Add(extra);
        }
    }

    void SetBallSpeedScale(float scale)
    {
        ballSpeedScale = scale;
        foreach (BallController b in balls) b.SpeedScale = scale;
    }

    void EndGame(bool won)
    {
        gameOver = true;
        Time.timeScale = 0f;
        hud.ShowGameOver(won);
    }
}
