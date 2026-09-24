using UnityEngine;

// The playfield is whatever the camera sees. Fits the invisible walls and the death zone
// to the view on load so the game works at any aspect ratio, and exposes the edges to
// anything else that needs them (paddle clamping, brick layout).
public class ArenaBounds : MonoBehaviour
{
    [SerializeField] BoxCollider2D leftWall;
    [SerializeField] BoxCollider2D rightWall;
    [SerializeField] BoxCollider2D topWall;
    [SerializeField] BoxCollider2D deathZone;

    Camera cam;

    Camera Cam
    {
        get
        {
            if (cam == null) cam = Camera.main;
            return cam;
        }
    }

    public float Left => Cam.transform.position.x - Cam.orthographicSize * Cam.aspect;
    public float Right => Cam.transform.position.x + Cam.orthographicSize * Cam.aspect;
    public float Top => Cam.transform.position.y + Cam.orthographicSize;
    public float Bottom => Cam.transform.position.y - Cam.orthographicSize;

    void Awake()
    {
        float centerX = (Left + Right) / 2f;
        float centerY = (Top + Bottom) / 2f;
        float width = Right - Left;
        float height = Top - Bottom;

        // Each collider keeps its authored thickness and sits just outside the view, so its
        // inner face lines up with the screen edge. Lengths overshoot to seal the corners.
        float side = leftWall.size.x;
        float top = topWall.size.y;
        float death = deathZone.size.y;

        Fit(leftWall, new Vector2(Left - side / 2f, centerY), new Vector2(side, height + 2f * top));
        Fit(rightWall, new Vector2(Right + side / 2f, centerY), new Vector2(side, height + 2f * top));
        Fit(topWall, new Vector2(centerX, Top + top / 2f), new Vector2(width + 2f * side, top));
        Fit(deathZone, new Vector2(centerX, Bottom - death / 2f), new Vector2(width + 2f * side, death));
    }

    static void Fit(BoxCollider2D box, Vector2 center, Vector2 size)
    {
        box.transform.position = center;
        box.offset = Vector2.zero;
        box.size = size;
    }
}
