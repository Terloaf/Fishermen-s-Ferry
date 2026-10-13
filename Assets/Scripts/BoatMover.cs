using UnityEngine;

public class BoatMover : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Vector3 moveDirection = Vector3.forward;

    [Header("Docking (slow stop when the timer ends)")]
    [SerializeField] private float dockingDeceleration = 1.5f;
    [SerializeField] private GameTimer gameTimer;

    private float _currentSpeed;
    private bool _isDocking;
    private bool _hasStarted; // true once the player has pressed Start

    void Update()
    {
        if (!_hasStarted) return; // boat stays still until the game actually starts

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

    // Call this from StartGame.OnGameStart() (or similar) to let the boat begin moving
    public void BeginMoving()
    {
        _hasStarted = true;
        _isDocking = false;
        _currentSpeed = moveSpeed;
    }

    // Optional: call this to manually trigger docking (e.g. if the run ends early for another reason)
    public void BeginDocking()
    {
        _isDocking = true;
    }

    // Optional: resets the boat back to full speed (e.g. starting a new run)
    public void ResetMovement()
    {
        _hasStarted = false;
        _isDocking = false;
        _currentSpeed = 0f;
    }

    public bool IsDocked => _isDocking && _currentSpeed <= 0.01f;
}
