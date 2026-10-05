using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class BH_PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BulletHellContext context;
    [SerializeField] private PlayerHitDetector hitDetector;
    [SerializeField] private Image playerVisual; // the visible sprite/image to blink during invulnerability

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 400f;

    [Header("Health")]
    [SerializeField] private int maxHealth = 2;

    [Header("Invulnerability visual")]
    [SerializeField] private float invulnerabilityDuration = 0.5f; // should match PlayerHitDetector's value
    [SerializeField] private float blinkInterval = 0.08f;

    public event System.Action<int> OnHealthChanged;
    public event System.Action OnPlayerDied;

    private RectTransform _rect;
    private int _currentHealth;
    private Coroutine _blinkRoutine;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _currentHealth = maxHealth;
    }

    void OnEnable()
    {
        if (hitDetector != null)
        {
            hitDetector.OnPlayerHit += HandleHit;
        }
    }

    void OnDisable()
    {
        if (hitDetector != null)
        {
            hitDetector.OnPlayerHit -= HandleHit;
        }
    }

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(h, v);
        if (input.sqrMagnitude > 1f) input.Normalize();

        Vector2 nextPos = _rect.anchoredPosition + input * moveSpeed * Time.deltaTime;
        _rect.anchoredPosition = ClampToArena(nextPos);
    }

    private Vector2 ClampToArena(Vector2 desiredPosition)
    {
        Rect bounds = context.ArenaBounds;
        Vector2 halfSize = _rect.rect.size * 0.5f;

        float clampedX = Mathf.Clamp(desiredPosition.x, bounds.xMin + halfSize.x, bounds.xMax - halfSize.x);
        float clampedY = Mathf.Clamp(desiredPosition.y, bounds.yMin + halfSize.y, bounds.yMax - halfSize.y);

        return new Vector2(clampedX, clampedY);
    }

    private void HandleHit()
    {
        _currentHealth--;
        OnHealthChanged?.Invoke(_currentHealth);

        if (_blinkRoutine != null) StopCoroutine(_blinkRoutine);
        _blinkRoutine = StartCoroutine(BlinkInvulnerability());

        if (_currentHealth <= 0)
        {
            OnPlayerDied?.Invoke();
        }
    }

    // Blinks the player's visual on/off to signal temporary invulnerability after a hit
    private IEnumerator BlinkInvulnerability()
    {
        if (playerVisual == null) yield break;

        float elapsed = 0f;
        bool visible = true;

        while (elapsed < invulnerabilityDuration)
        {
            visible = !visible;
            SetVisualAlpha(visible ? 1f : 0.2f);
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        SetVisualAlpha(1f); // ensure fully visible once invulnerability ends
        _blinkRoutine = null;
    }

    private void SetVisualAlpha(float alpha)
    {
        var c = playerVisual.color;
        c.a = alpha;
        playerVisual.color = c;
    }

    public void ResetHealth()
    {
        _currentHealth = maxHealth;
        OnHealthChanged?.Invoke(_currentHealth);

        if (_blinkRoutine != null)
        {
            StopCoroutine(_blinkRoutine);
            _blinkRoutine = null;
        }
        SetVisualAlpha(1f);
    }
}