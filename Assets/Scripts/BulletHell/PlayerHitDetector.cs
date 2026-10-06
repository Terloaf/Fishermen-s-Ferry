using UnityEngine;

public class PlayerHitDetector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform playerRect;

    [Header("Hit behaviour")]
    [SerializeField] private float invulnerabilityDuration = 0.5f;
    [SerializeField] private bool logHitsToConsole = true;

    public event System.Action OnPlayerHit;

    private float _invulnerableUntil;

    void Update()
    {
        if (playerRect == null) return;
        if (Time.time < _invulnerableUntil) return;

        Rect playerWorldRect = GetWorldRect(playerRect);

        foreach (var hazard in HazardZone.Active)
        {
            if (!hazard.IsActive) continue;

            bool overlapping = hazard.ZoneShape == HazardZone.Shape.Circle
                ? CircleIntersectsRect(hazard.GetWorldCenter(), hazard.GetWorldRadius(), playerWorldRect)
                : playerWorldRect.Overlaps(hazard.GetWorldRect());

            if (overlapping)
            {
                RegisterHit();
                break;
            }
        }
    }

    // Finds the closest point on the rect to the circle's center and compares the distance to the radius
    private bool CircleIntersectsRect(Vector2 circleCenter, float radius, Rect rect)
    {
        float closestX = Mathf.Clamp(circleCenter.x, rect.xMin, rect.xMax);
        float closestY = Mathf.Clamp(circleCenter.y, rect.yMin, rect.yMax);

        float dx = circleCenter.x - closestX;
        float dy = circleCenter.y - closestY;
        float distanceSquared = dx * dx + dy * dy;

        return distanceSquared <= radius * radius;
    }

    private void RegisterHit()
    {
        _invulnerableUntil = Time.time + invulnerabilityDuration;

        if (logHitsToConsole)
        {
            Debug.Log("Player hit by a hazard zone!");
        }

        OnPlayerHit?.Invoke();
    }

    private Rect GetWorldRect(RectTransform rect)
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);
        return new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
    }
}