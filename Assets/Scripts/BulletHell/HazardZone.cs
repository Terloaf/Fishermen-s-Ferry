using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class HazardZone : MonoBehaviour
{
    public static readonly List<HazardZone> Active = new List<HazardZone>();

    public enum Shape { Rectangle, Circle }

    public bool IsActive { get; set; }
    public Shape ZoneShape { get; set; } = Shape.Rectangle;

    public RectTransform Rect { get; private set; }

    void Awake()
    {
        Rect = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        Active.Add(this);
    }

    void OnDisable()
    {
        Active.Remove(this);
    }

    public Rect GetWorldRect()
    {
        Vector3[] corners = new Vector3[4];
        Rect.GetWorldCorners(corners);
        return new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
    }

    // World-space center and radius, used when ZoneShape == Circle.
    // Radius is derived from the rect's world width (assumes a square rect sized radius*2).
    public Vector2 GetWorldCenter()
    {
        Vector3[] corners = new Vector3[4];
        Rect.GetWorldCorners(corners);
        return new Vector2((corners[0].x + corners[2].x) * 0.5f, (corners[0].y + corners[2].y) * 0.5f);
    }

    public float GetWorldRadius()
    {
        Vector3[] corners = new Vector3[4];
        Rect.GetWorldCorners(corners);
        float width = corners[2].x - corners[0].x;
        return width * 0.5f;
    }
}