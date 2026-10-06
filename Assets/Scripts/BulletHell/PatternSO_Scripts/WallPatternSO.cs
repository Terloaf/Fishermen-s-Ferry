using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "BulletHell/Patterns/Wall")]
public class WallPatternSO : BulletPatternSO
{
    public enum Orientation { Horizontal, Vertical }

    [Header("Wall placement (defaults, can be overridden per preset entry)")]
    public Orientation orientation = Orientation.Horizontal;
    [Range(-1f, 1f)] public float position = 0f; // 0 = center of the arena
    public float thickness = 20f;

    [Header("Timing")]
    public float blinkInterval = 0.15f;
    public float activeDuration = 0.6f;

    [Header("Telegraph")]
    public Color telegraphColor = new Color(1f, 0f, 0f, 0.35f);
    public Color solidColor = new Color(1f, 0f, 0f, 1f);

    public override IEnumerator Execute(BulletHellContext ctx, PatternOverrides overrides)
    {
        Orientation effectiveOrientation = overrides.overrideOrientation ? overrides.orientation : orientation;
        float effectivePosition = overrides.overridePosition ? overrides.position.x : position;
        effectivePosition = Mathf.Clamp(effectivePosition, -1f, 1f); // safety clamp

        Rect bounds = ctx.ArenaBounds;

        Vector2 anchoredPos;
        Vector2 size;

        if (effectiveOrientation == Orientation.Horizontal)
        {
            // -1 -> bottom edge, 0 -> center, 1 -> top edge
            float y = effectivePosition * (bounds.height * 0.5f);
            anchoredPos = new Vector2(0f, y);
            size = new Vector2(bounds.width, thickness);
        }
        else
        {
            // -1 -> left edge, 0 -> center, 1 -> right edge
            float x = effectivePosition * (bounds.width * 0.5f);
            anchoredPos = new Vector2(x, 0f);
            size = new Vector2(thickness, bounds.height);
        }

        var obj = Object.Instantiate(warningPrefab, ctx.SpawnParent);
        SetupRect(obj, anchoredPos, size);

        var img = obj.GetComponentInChildren<Image>();

        float elapsed = 0f;
        bool visible = true;
        while (elapsed < telegraphDuration)
        {
            if (img != null) img.color = visible ? telegraphColor : new Color(telegraphColor.r, telegraphColor.g, telegraphColor.b, 0f);
            visible = !visible;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        if (img != null) img.color = solidColor;

        var hazard = obj.GetComponent<HazardZone>();
        if (hazard == null) hazard = obj.AddComponent<HazardZone>();
        hazard.IsActive = true;

        yield return new WaitForSeconds(activeDuration);

        Object.Destroy(obj);
    }

    private void SetupRect(GameObject obj, Vector2 anchoredPosition, Vector2 size)
    {
        var rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;
    }
}