using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "BulletHell/Patterns/Circle")]
public class CirclePatternSO : BulletPatternSO
{
    [Header("Circle placement (defaults, can be overridden per preset entry)")]
    [Range(-1f, 1f)] public float centerX = 0f; // 0 = center of the arena
    [Range(-1f, 1f)] public float centerY = 0f;
    public float radius = 80f;

    [Header("Timing")]
    public float blinkInterval = 0.15f;
    public float activeDuration = 0.6f;

    [Header("Telegraph")]
    public Color telegraphColor = new Color(1f, 0f, 0f, 0.35f);
    public Color solidColor = new Color(1f, 0f, 0f, 1f);

    public override IEnumerator Execute(BulletHellContext ctx, PatternOverrides overrides)
    {
        Vector2 effectiveCenter = overrides.overridePosition ? overrides.position : new Vector2(centerX, centerY);
        effectiveCenter.x = Mathf.Clamp(effectiveCenter.x, -1f, 1f);
        effectiveCenter.y = Mathf.Clamp(effectiveCenter.y, -1f, 1f);

        float effectiveRadius = overrides.overrideRadius ? overrides.radius : radius;

        Rect bounds = ctx.ArenaBounds;

        // -1 -> min edge, 0 -> center, 1 -> max edge
        float x = effectiveCenter.x * (bounds.width * 0.5f);
        float y = effectiveCenter.y * (bounds.height * 0.5f);
        Vector2 anchoredPos = new Vector2(x, y);
        Vector2 size = new Vector2(effectiveRadius * 2f, effectiveRadius * 2f);

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
        hazard.ZoneShape = HazardZone.Shape.Circle;
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