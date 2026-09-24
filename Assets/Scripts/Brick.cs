using System;
using UnityEngine;

// Spawned and configured by BrickGrid; reports back when the ball breaks it.
[RequireComponent(typeof(SpriteRenderer))]
public class Brick : MonoBehaviour
{
    public int Points { get; private set; }
    public event Action<Brick> Broken;

    bool broken;

    public void Init(Color color, int points)
    {
        GetComponent<SpriteRenderer>().color = color;
        Points = points;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Destroy only lands at the end of the frame, so guard against a second hit
        // in the same frame counting the brick twice.
        if (broken || !collision.collider.TryGetComponent(out BallController _)) return;

        broken = true;
        Broken?.Invoke(this);
        Destroy(gameObject);
    }
}
