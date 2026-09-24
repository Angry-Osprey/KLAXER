using System;
using TMPro;
using UnityEngine;

public enum PowerUpType
{
    WidePaddle,
    MultiBall,
    SlowBall,
    ExtraLife,
}

// A falling pickup. Spawned by PowerUpDropper; reports back if the paddle catches it.
// Its collider is a trigger, so it passes through bricks and balls.
[RequireComponent(typeof(Rigidbody2D))]
public class PowerUp : MonoBehaviour
{
    [SerializeField] SpriteRenderer body;
    [SerializeField] TMP_Text label;

    public PowerUpType Type { get; private set; }
    public event Action<PowerUp> Collected;

    Rigidbody2D rb;
    float despawnY;
    bool collected;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(PowerUpType type, Color color, string text, float fallSpeed, float despawnBelowY)
    {
        Type = type;
        body.color = color;
        label.text = text;
        despawnY = despawnBelowY;
        rb.linearVelocity = Vector2.down * fallSpeed;
    }

    void FixedUpdate()
    {
        if (rb.position.y < despawnY) Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Destroy only lands at the end of the frame, so don't let a second contact count twice.
        if (collected || !other.TryGetComponent(out PaddleController _)) return;

        collected = true;
        Collected?.Invoke(this);
        Destroy(gameObject);
    }
}
