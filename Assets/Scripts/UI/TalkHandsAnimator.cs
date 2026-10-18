using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Controls the hands sprite shown while talking to a fish:
// slides it up from below the screen, plays a subtle figure-eight idle motion,
// and slides it back down when the dialogue ends.
[RequireComponent(typeof(RectTransform))]
public class TalkHandsAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image fishImage; // image on the hands that shows the held fish

    [Header("Slide settings")]
    [SerializeField] private float slideDuration = 0.35f;
    [SerializeField] private float hiddenYOffset = 400f; // how far below the resting position it starts/ends
    [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Idle figure-eight motion")]
    [SerializeField] private float idleWidth = 15f;
    [SerializeField] private float idleHeight = 8f;
    [SerializeField] private float idleSpeed = 1f;

    private RectTransform _rect;
    private Vector2 _restingPosition;
    private Coroutine _activeRoutine;
    private bool _isShown;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _restingPosition = _rect.anchoredPosition;
    }

    public void Show(Sprite sprite)
    {
        if (fishImage != null)
        {
            fishImage.sprite = sprite;
            fishImage.enabled = sprite != null;
        }

        if (_isShown) return;
        _isShown = true;

        gameObject.SetActive(true);

        if (_activeRoutine != null) StopCoroutine(_activeRoutine);
        _activeRoutine = StartCoroutine(ShowRoutine());
    }

    public void Hide()
    {
        if (!_isShown)
        {
            gameObject.SetActive(false);
            return;
        }
        _isShown = false;

        if (_activeRoutine != null) StopCoroutine(_activeRoutine);
        _activeRoutine = StartCoroutine(HideRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        Vector2 start = _restingPosition + Vector2.down * hiddenYOffset;
        Vector2 end = _restingPosition;

        yield return Slide(start, end);

        _activeRoutine = StartCoroutine(IdleRoutine());
    }

    private IEnumerator HideRoutine()
    {
        Vector2 start = _rect.anchoredPosition;
        Vector2 end = _restingPosition + Vector2.down * hiddenYOffset;

        yield return Slide(start, end);

        gameObject.SetActive(false);
    }

    private IEnumerator Slide(Vector2 from, Vector2 to)
    {
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = slideCurve.Evaluate(Mathf.Clamp01(elapsed / slideDuration));
            _rect.anchoredPosition = Vector2.LerpUnclamped(from, to, t);
            yield return null;
        }

        _rect.anchoredPosition = to;
    }

    private IEnumerator IdleRoutine()
    {
        float time = 0f;

        while (true)
        {
            time += Time.unscaledDeltaTime * idleSpeed;

            // Figure-eight (Lissajous curve): x uses sin, y uses sin(2x) for the "8" shape
            float x = Mathf.Sin(time) * idleWidth;
            float y = Mathf.Sin(time * 2f) * idleHeight;

            _rect.anchoredPosition = _restingPosition + new Vector2(x, y);
            yield return null;
        }
    }
}
