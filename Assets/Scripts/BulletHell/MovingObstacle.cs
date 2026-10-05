using UnityEngine;

// Moves a UI element (RectTransform) at a fixed speed along a direction
[RequireComponent(typeof(RectTransform))]
public class MovingObstacle : MonoBehaviour
{
    private RectTransform _rect;
    private Vector2 _direction;
    private float _speed;
    private bool _initialized;
    private Rect? _destroyBounds;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void Init(float speed, Vector2 targetAnchoredPosition)
    {
        _direction = (targetAnchoredPosition - _rect.anchoredPosition).normalized;
        _speed = speed;
        _initialized = true;
    }

    // Explicit direction, used by walls moving straight across the arena
    public void InitDirectional(float speed, Vector2 direction, Rect? destroyBounds = null)
    {
        _direction = direction.normalized;
        _speed = speed;
        _destroyBounds = destroyBounds;
        _initialized = true;
    }

    void Update()
    {
        if (!_initialized) return;
        _rect.anchoredPosition += _direction * _speed * Time.deltaTime;

        if (_destroyBounds.HasValue && !_destroyBounds.Value.Contains(_rect.anchoredPosition))
        {
            Destroy(gameObject);
        }
    }

    // Optional: handled via a UI-based hit detection script instead of physics trigger,
    // since Canvas Image objects usually don't rely on Collider2D.
}