using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "BulletHell/Patterns/Projectile")]
public class ProjectilePatternSO : BulletPatternSO
{
    [Header("Projectile placement (defaults, can be overridden per preset entry)")]
    public SpawnSide spawnSide = SpawnSide.Top;
    [Range(-1f, 1f)] public float position = 0f; // position along the spawn edge, 0 = center (same convention as Wall/Circle)
    public float projectileSize = 50f;
    public float speed = 400f;

    [Header("Timing")]
    public float blinkInterval = 0.15f;

    [Header("Telegraph")]
    public Color telegraphColor = new Color(1f, 0f, 0f, 0.35f);
    public Color solidColor = new Color(1f, 0f, 0f, 1f);

    public override IEnumerator Execute(BulletHellContext ctx, PatternOverrides overrides)
    {
        SpawnSide effectiveSide = overrides.overrideSide ? overrides.side : spawnSide;
        float effectivePosition = overrides.overridePosition ? overrides.position.x : position;
        effectivePosition = Mathf.Clamp(effectivePosition, -1f, 1f);

        float effectiveSize = overrides.overrideSize ? overrides.size : projectileSize;
        float effectiveSpeed = overrides.overrideSpeed ? overrides.speed : speed;

        Rect bounds = ctx.ArenaBounds;

        Vector2 spawnPos = GetSpawnPosition(bounds, effectiveSide, effectivePosition);
        Vector2 moveDirection = GetOppositeDirection(effectiveSide);

        // 1. Telegraph: a transparent preview blinking at the spawn point
        var telegraph = Object.Instantiate(warningPrefab, ctx.SpawnParent);
        SetupRect(telegraph, spawnPos, effectiveSize);

        var telegraphImg = telegraph.GetComponentInChildren<Image>();

        float elapsed = 0f;
        bool visible = true;
        while (elapsed < telegraphDuration)
        {
            if (telegraphImg != null) telegraphImg.color = visible ? telegraphColor : new Color(telegraphColor.r, telegraphColor.g, telegraphColor.b, 0f);
            visible = !visible;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        Object.Destroy(telegraph);

        // 2. Spawn the real projectile and send it flying toward the opposite side
        var projectile = Object.Instantiate(attackPrefab, ctx.SpawnParent);
        SetupRect(projectile, spawnPos, effectiveSize);

        var img = projectile.GetComponentInChildren<Image>();
        if (img != null) img.color = solidColor;

        var hazard = projectile.GetComponent<HazardZone>();
        if (hazard == null) hazard = projectile.AddComponent<HazardZone>();
        hazard.IsActive = true;

        var obstacle = projectile.GetComponent<MovingObstacle>();
        if (obstacle == null) obstacle = projectile.AddComponent<MovingObstacle>();

        Rect expandedBounds = new Rect(
            bounds.xMin - 200f, bounds.yMin - 200f,
            bounds.width + 400f, bounds.height + 400f);

        obstacle.InitDirectional(effectiveSpeed, moveDirection, expandedBounds);

        // Wait until the projectile actually leaves the arena and destroys itself,
        // so the pattern's Execute() coroutine only finishes once it's truly gone.
        while (projectile != null)
        {
            yield return null;
        }
    }

    private Vector2 GetSpawnPosition(Rect bounds, SpawnSide side, float position)
    {
        // position: -1 -> one end of the edge, 0 -> center, 1 -> other end of the edge
        switch (side)
        {
            case SpawnSide.Top:
                return new Vector2(position * (bounds.width * 0.5f), bounds.yMax);
            case SpawnSide.Bottom:
                return new Vector2(position * (bounds.width * 0.5f), bounds.yMin);
            case SpawnSide.Left:
                return new Vector2(bounds.xMin, position * (bounds.height * 0.5f));
            case SpawnSide.Right:
                return new Vector2(bounds.xMax, position * (bounds.height * 0.5f));
            default:
                return Vector2.zero;
        }
    }

    private Vector2 GetOppositeDirection(SpawnSide side)
    {
        switch (side)
        {
            case SpawnSide.Top: return Vector2.down;
            case SpawnSide.Bottom: return Vector2.up;
            case SpawnSide.Left: return Vector2.right;
            case SpawnSide.Right: return Vector2.left;
            default: return Vector2.zero;
        }
    }

    private void SetupRect(GameObject obj, Vector2 anchoredPosition, float size)
    {
        var rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(size, size);
    }
}
