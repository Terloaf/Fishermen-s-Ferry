using UnityEngine;

// Simple obstacle that moves toward a target direction at a fixed speed
public class MovingObstacle : MonoBehaviour
{
    private Vector3 _direction;
    private float _speed;
    private bool _initialized;

    public void Init(float speed, Vector3 targetPosition)
    {
        _direction = (targetPosition - transform.position).normalized;
        _speed = speed;
        _initialized = true;
    }

    void Update()
    {
        if (!_initialized) return;
        transform.position += _direction * _speed * Time.deltaTime;
    }

    // Optional: destroy self when colliding with the player
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerHitbox"))
        {
            // Hook into your health/damage system here
        }
    }
}