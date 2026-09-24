using UnityEngine;

// Kinematic paddle. Input picks a target x in Update; the body is moved there in
// FixedUpdate with MovePosition so the physics engine sees its velocity and the ball
// bounces off it properly instead of being teleported into.
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PaddleController : MonoBehaviour
{
    [SerializeField] float keyboardSpeed = 12f;
    [SerializeField] ArenaBounds arena;

    Rigidbody2D rb;
    Camera cam;
    float baseScaleX;
    float baseHalfWidth;
    float halfWidth;
    float targetX;
    float wideEndsAt = -1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    void Start()
    {
        baseScaleX = transform.localScale.x;
        baseHalfWidth = GetComponent<BoxCollider2D>().bounds.extents.x;
        halfWidth = baseHalfWidth;
        targetX = rb.position.x;
    }

    void Update()
    {
        if (wideEndsAt >= 0f && Time.time >= wideEndsAt)
        {
            wideEndsAt = -1f;
            SetWidthScale(1f);
        }

        if (GameInput.TryGetHeldPointer(out Vector2 screenPosition))
            targetX = cam.ScreenToWorldPoint(screenPosition).x;
        else
            targetX += GameInput.MoveAxis * keyboardSpeed * Time.deltaTime;

        targetX = Mathf.Clamp(targetX, arena.Left + halfWidth, arena.Right - halfWidth);
    }

    void FixedUpdate()
    {
        rb.MovePosition(new Vector2(targetX, rb.position.y));
    }

    // Catching another one while wide restarts the timer rather than stacking.
    public void Widen(float scale, float seconds)
    {
        SetWidthScale(scale);
        wideEndsAt = Time.time + seconds;
    }

    void SetWidthScale(float scale)
    {
        Vector3 localScale = transform.localScale;
        localScale.x = baseScaleX * scale;
        transform.localScale = localScale;
        halfWidth = baseHalfWidth * scale;
    }
}
