using System;
using UnityEngine;

// Trigger below the paddle. Reports a lost ball; what that costs is GameManager's call.
[RequireComponent(typeof(Collider2D))]
public class DeathZone : MonoBehaviour
{
    public event Action<BallController> BallEntered;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out BallController ball)) BallEntered?.Invoke(ball);
    }
}
