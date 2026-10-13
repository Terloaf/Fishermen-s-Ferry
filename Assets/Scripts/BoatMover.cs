using UnityEngine;

public class BoatMover : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Vector3 moveDirection = Vector3.forward;

    [Header("Docking (slow stop when the timer ends)")]
    [SerializeField] private float dockingDeceleration = 1.5f; // units/sec^2, how quickly it slows down
    [SerializeField] private GameTimer gameTimer;

    private float _currentSpeed;
    private bool _isDocking;

    void Start()
    {
        _currentSpeed = moveSpeed;
    }

    void Update()
    {
        // Start docking once the timer runs out
        if (!_isDocking && gameTimer != null && gameTimer.isPlaying && gameTimer.RemainingTime <= 0f)
        {
            _isDocking = true;
        }

        if (_isDocking)
        {
            // Gradually slow the boat down to a full stop
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0f, dockingDeceleration * Time.deltaTime);
        }
        else
        {
            _currentSpeed = moveSpeed;
        }

        if (_currentSpeed > 0f)
        {
            transform.position += moveDirection.normalized * _currentSpeed * Time.deltaTime;
        }
    }

    // Optional: call this to manually trigger docking (e.g. if the run ends early for another reason)
    public void BeginDocking()
    {
        _isDocking = true;
    }

    // Optional: resets the boat back to full speed (e.g. starting a new run)
    public void ResetMovement()
    {
        _isDocking = false;
        _currentSpeed = moveSpeed;
    }

    public bool IsDocked => _isDocking && _currentSpeed <= 0.01f;
}
