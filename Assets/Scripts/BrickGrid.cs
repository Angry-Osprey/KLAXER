using System;
using UnityEngine;

// Builds the wall of bricks at runtime, sized to fill the arena's width at any aspect ratio
// (up to a maximum brick width, beyond which the grid is centred). Colours and point values
// come from bands that split the rows evenly, so the row count can change freely.
public class BrickGrid : MonoBehaviour
{
    [Serializable]
    struct Band
    {
        public Color color;
        public int points;
    }

    [SerializeField] Brick brickPrefab;
    [SerializeField] ArenaBounds arena;
    [SerializeField, Min(1)] int columns = 10;
    [SerializeField, Min(1)] int rows = 20;
    [Tooltip("Colour bands from the top down; the rows are shared out evenly between them.")]
    [SerializeField] Band[] bands =
    {
        new Band { color = new Color(0.9f, 0.2f, 0.2f), points = 50 },
        new Band { color = new Color(0.95f, 0.55f, 0.15f), points = 40 },
        new Band { color = new Color(0.95f, 0.9f, 0.2f), points = 30 },
        new Band { color = new Color(0.3f, 0.85f, 0.3f), points = 20 },
        new Band { color = new Color(0.3f, 0.55f, 0.95f), points = 10 },
    };
    [SerializeField] float brickHeight = 0.3f;
    [Tooltip("Stops bricks stretching on wide screens.")]
    [SerializeField] float maxBrickWidth = 1.3f;
    [SerializeField] float gap = 0.08f;
    [Tooltip("Space between the outer bricks and the side walls.")]
    [SerializeField] float sideInset = 0.3f;
    [Tooltip("Distance from the top of the screen to the centre of the first row; leaves room for the HUD.")]
    [SerializeField] float topInset = 1.8f;

    public int Remaining { get; private set; }
    public event Action<Brick> BrickBroken;

    void Start()
    {
        float brickWidth = BrickWidth();
        for (int row = 0; row < rows; row++)
        {
            Band band = BandFor(row);
            for (int column = 0; column < columns; column++)
            {
                Brick brick = Instantiate(brickPrefab, CellCenter(row, column, brickWidth), Quaternion.identity, transform);
                brick.transform.localScale = new Vector3(brickWidth, brickHeight, 1f);
                brick.name = $"Brick_{row}_{column}";
                brick.Init(band.color, band.points);
                brick.Broken += OnBrickBroken;
            }
        }
        Remaining = rows * columns;
    }

    void OnBrickBroken(Brick brick)
    {
        Remaining--;
        BrickBroken?.Invoke(brick);
    }

    Band BandFor(int row) => bands[row * bands.Length / rows];

    float BrickWidth()
    {
        float available = arena.Right - arena.Left - 2f * sideInset;
        return Mathf.Min((available - gap * (columns - 1)) / columns, maxBrickWidth);
    }

    Vector2 CellCenter(int row, int column, float brickWidth)
    {
        float gridWidth = columns * brickWidth + (columns - 1) * gap;
        float left = (arena.Left + arena.Right - gridWidth) / 2f;
        float x = left + brickWidth / 2f + column * (brickWidth + gap);
        float y = arena.Top - topInset - row * (brickHeight + gap);
        return new Vector2(x, y);
    }

    // Bricks only exist in Play mode, so preview the layout in the Scene view.
    void OnDrawGizmos()
    {
        if (Application.isPlaying || arena == null || Camera.main == null || bands == null || bands.Length == 0) return;

        float brickWidth = BrickWidth();
        for (int row = 0; row < rows; row++)
        {
            Gizmos.color = BandFor(row).color;
            for (int column = 0; column < columns; column++)
                Gizmos.DrawWireCube(CellCenter(row, column, brickWidth), new Vector3(brickWidth, brickHeight, 0f));
        }
    }
}
