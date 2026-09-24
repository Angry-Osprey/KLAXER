using UnityEngine;

// Moves the ball and nothing else. When to launch, scoring and lives belong to GameManager.
[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour
{
    [SerializeField] float speed = 8f;
    [SerializeField] PaddleController paddle;
    [Tooltip("Where the ball rests relative to the paddle before launch.")]
    [SerializeField] Vector2 restOffset = new Vector2(0f, 0.8f);
    [Tooltip("Bounce angle from vertical when the ball hits the very edge of the paddle.")]
    [SerializeField, Range(0f, 80f)] float maxPaddleAngle = 60f;
    [Tooltip("Shallowest angle from horizontal the ball may travel at, so it can't get stuck bouncing wall to wall.")]
    [SerializeField, Range(0f, 45f)] float minAngleFromHorizontal = 15f;

    Rigidbody2D rb;
    bool launched;

    public bool IsHeld => !launched;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        ResetToPaddle();
    }

    void LateUpdate()
    {
        if (!launched) FollowPaddle();
    }

    void FixedUpdate()
    {
        // Bouncy-material collisions drift in speed and can settle into near-flat paths;
        // pin both every step.
        if (launched) rb.linearVelocity = KeepPlayable(rb.linearVelocity) * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.TryGetComponent(out PaddleController _)) return;

        // Steer by where the ball hit: -1 at the paddle's left edge, +1 at its right edge.
        Bounds paddleBounds = collision.collider.bounds;
        float hit = Mathf.Clamp((rb.position.x - paddleBounds.center.x) / paddleBounds.extents.x, -1f, 1f);
        float angle = hit * maxPaddleAngle * Mathf.Deg2Rad;
        rb.linearVelocity = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * speed;
    }

    public void ResetToPaddle()
    {
        launched = false;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        FollowPaddle();
    }

    // While held the body is out of the simulation, so track the paddle's rendered position.
    void FollowPaddle()
    {
        transform.position = paddle.transform.position + (Vector3)restOffset;
    }

    public void Launch()
    {
        launched = true;
        rb.simulated = true;
        rb.position = transform.position;
        rb.linearVelocity = new Vector2(Random.Range(-0.5f, 0.5f), 1f).normalized * speed;
    }

    Vector2 KeepPlayable(Vector2 velocity)
    {
        Vector2 direction = velocity.sqrMagnitude > 0.0001f ? velocity.normalized : Vector2.up;
        float minY = Mathf.Sin(minAngleFromHorizontal * Mathf.Deg2Rad);
        if (Mathf.Abs(direction.y) < minY)
        {
            direction.y = Mathf.Sign(direction.y) * minY;
            direction.x = Mathf.Sign(direction.x) * Mathf.Sqrt(1f - minY * minY);
        }
        return direction;
    }
}
