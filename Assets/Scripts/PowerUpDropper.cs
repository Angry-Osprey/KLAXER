using System;
using UnityEngine;
using Random = UnityEngine.Random;

// Rolls for a power-up each time a brick breaks and drops it from where the brick was.
// What each power-up does is GameManager's call.
public class PowerUpDropper : MonoBehaviour
{
    [Serializable]
    struct Option
    {
        public PowerUpType type;
        public string label;
        public Color color;
        [Min(0f)] public float weight;
    }

    [SerializeField] BrickGrid bricks;
    [SerializeField] ArenaBounds arena;
    [SerializeField] PowerUp prefab;
    [SerializeField, Range(0f, 100f)] float dropChancePercent = 12f;
    [SerializeField] float fallSpeed = 3f;
    [Tooltip("Relative odds of each power-up when one drops.")]
    [SerializeField] Option[] options =
    {
        new Option { type = PowerUpType.WidePaddle, label = "W", color = new Color(0.2f, 0.8f, 1f), weight = 3f },
        new Option { type = PowerUpType.MultiBall, label = "M", color = new Color(1f, 1f, 1f), weight = 3f },
        new Option { type = PowerUpType.SlowBall, label = "S", color = new Color(0.55f, 1f, 0.75f), weight = 2f },
        new Option { type = PowerUpType.ExtraLife, label = "+", color = new Color(1f, 0.45f, 0.6f), weight = 1f },
    };

    public event Action<PowerUpType> Collected;

    void OnEnable()
    {
        bricks.BrickBroken += OnBrickBroken;
    }

    void OnDisable()
    {
        bricks.BrickBroken -= OnBrickBroken;
    }

    void OnBrickBroken(Brick brick)
    {
        if (Random.Range(0f, 100f) >= dropChancePercent) return;

        Option option = PickOption();
        PowerUp powerUp = Instantiate(prefab, brick.transform.position, Quaternion.identity, transform);
        powerUp.Init(option.type, option.color, option.label, fallSpeed, arena.Bottom - 1f);
        powerUp.Collected += OnPowerUpCollected;
    }

    void OnPowerUpCollected(PowerUp powerUp)
    {
        Collected?.Invoke(powerUp.Type);
    }

    Option PickOption()
    {
        float total = 0f;
        foreach (Option option in options) total += option.weight;

        float roll = Random.Range(0f, total);
        foreach (Option option in options)
        {
            roll -= option.weight;
            if (roll < 0f) return option;
        }
        return options[options.Length - 1];
    }
}
