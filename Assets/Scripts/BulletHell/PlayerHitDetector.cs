using UnityEngine;

public class PlayerHitDetector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CircleCollider2D playerCollider;

    [Header("Hit behaviour")]
    [SerializeField] private float invulnerabilityDuration = 0.5f;
    [SerializeField] private bool logHitsToConsole = true;

    public event System.Action OnPlayerHit;

    private float _invulnerableUntil;

    void Update()
    {
        if (playerCollider == null) return;
        if (Time.time < _invulnerableUntil) return;

        Vector2 playerCenter = GetWorldCenter(playerCollider);
        float playerRadius = GetWorldRadius(playerCollider);

        foreach (var hazard in HazardZone.Active)
        {
            if (!hazard.IsActive) continue;

            bool overlapping = hazard.ZoneShape == HazardZone.Shape.Circle
                ? CircleIntersectsCircle(playerCenter, playerRadius, hazard.GetWorldCenter(), hazard.GetWorldRadius())
                : CircleIntersectsRect(playerCenter, playerRadius, hazard.GetWorldRect());

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

    // Standard circle-vs-circle overlap: distance between centers <= sum of radii
    private bool CircleIntersectsCircle(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB)
    {
        float dx = centerA.x - centerB.x;
        float dy = centerA.y - centerB.y;
        float distanceSquared = dx * dx + dy * dy;
        float radiusSum = radiusA + radiusB;

        return distanceSquared <= radiusSum * radiusSum;
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

    // World-space center of a CircleCollider2D (accounts for the collider's local offset)
    private Vector2 GetWorldCenter(CircleCollider2D collider)
    {
        return collider.transform.TransformPoint(collider.offset);
    }

    // World-space radius of a CircleCollider2D (accounts for non-uniform scale by using the largest axis,
    // matching Unity's own internal approach for circle colliders)
    private float GetWorldRadius(CircleCollider2D collider)
    {
        Vector3 lossyScale = collider.transform.lossyScale;
        float maxScale = Mathf.Max(Mathf.Abs(lossyScale.x), Mathf.Abs(lossyScale.y));
        return collider.radius * maxScale;
    }
}