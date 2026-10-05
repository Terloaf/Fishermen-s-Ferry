using UnityEngine;

public class BulletHellContext : MonoBehaviour
{
    [Header("UI Arena")]
    public RectTransform arenaRect;  // the RectTransform that defines the play area (child of Canvas)
    public RectTransform player;     // player's RectTransform (UI-based)

    // Returns the full arena rect in local (anchored) space, centered at (0,0)
    public Rect ArenaBounds
    {
        get
        {
            float halfW = arenaRect.rect.width * 0.5f;
            float halfH = arenaRect.rect.height * 0.5f;
            return new Rect(-halfW, -halfH, arenaRect.rect.width, arenaRect.rect.height);
        }
    }

    public Vector2 SpawnPoint(float normalizedPosition)
    {
        Rect bounds = ArenaBounds;
        float x = Mathf.Lerp(bounds.xMin, bounds.xMax, normalizedPosition);
        return new Vector2(x, bounds.yMax);
    }

    public Vector2 PlayerAnchoredPosition => player.anchoredPosition;

    // Use this as the parent when instantiating UI attack objects
    public Transform SpawnParent => arenaRect;
}