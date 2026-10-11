using System.Collections;
using UnityEngine;

public class BullethellLogic : MonoBehaviour
{
    [SerializeField] private BulletHellPresetSO[] availablePresets;
    [SerializeField] private BulletHellContext context;
    [SerializeField] private BH_PlayerController playerController;

    [Header("End-of-level timing")]
    [SerializeField] private float endBuffer = 1.5f; // extra pause after the last hazard disappears, before fading out

    public event System.Action OnChallengeSurvived;
    public event System.Action OnChallengeFailed;

    private Coroutine _runningRoutine;
    private bool _challengeActive;
    private BulletHellPresetSO _preparedPreset;

    void OnEnable()
    {
        if (playerController != null)
        {
            playerController.OnPlayerDied += HandlePlayerDied;
        }
    }

    void OnDisable()
    {
        if (playerController != null)
        {
            playerController.OnPlayerDied -= HandlePlayerDied;
        }
    }

    // Call this as soon as the bullet hell canvas starts appearing (fade-in begins),
    // so the player position/health resets before they can see/move during the buffer.
    public void PrepareForChallenge()
    {
        CleanupActiveHazards();
        if (playerController != null) playerController.ResetHealth();

        _preparedPreset = PickPreset();
    }

    // Call this once the pre-challenge buffer has elapsed, to actually start the attack sequence.
    public void StartChallenge()
    {
        if (_challengeActive) return;

        // Fallback in case PrepareForChallenge wasn't called beforehand
        if (_preparedPreset == null)
        {
            PrepareForChallenge();
        }

        _challengeActive = true;
        _runningRoutine = StartCoroutine(RunPreset(_preparedPreset));
        _preparedPreset = null;
    }

    // Call this if the fishing system needs to forcibly stop (e.g. scene change)
    public void StopChallenge()
    {
        if (_runningRoutine != null) StopCoroutine(_runningRoutine);
        _challengeActive = false;
        CleanupActiveHazards();
    }

    private BulletHellPresetSO PickPreset()
    {
        return availablePresets[Random.Range(0, availablePresets.Length)];
    }

    private IEnumerator RunPreset(BulletHellPresetSO preset)
    {
        for (int i = 0; i < preset.sequence.Length; i++)
        {
            if (!_challengeActive) yield break;

            var entry = preset.sequence[i];
            bool isLast = i == preset.sequence.Length - 1;

            if (isLast)
            {
                // Wait for the final pattern to fully finish (telegraph + active + disappear)
                yield return entry.pattern.Execute(context, entry.overrides);
            }
            else
            {
                StartCoroutine(entry.pattern.Execute(context, entry.overrides));
                yield return new WaitForSeconds(entry.delayBeforeNext);
            }
        }

        if (_challengeActive)
        {
            // Give the player a breather before the canvas fades out
            yield return new WaitForSeconds(endBuffer);

            _challengeActive = false;
            CleanupActiveHazards();
            OnChallengeSurvived?.Invoke();
        }
    }

    private void HandlePlayerDied()
    {
        if (!_challengeActive) return;

        if (_runningRoutine != null) StopCoroutine(_runningRoutine);
        _challengeActive = false;
        CleanupActiveHazards();
        OnChallengeFailed?.Invoke();
    }

    // Destroys any hazard zones still active in the scene (e.g. if the challenge ends early)
    private void CleanupActiveHazards()
    {
        for (int i = HazardZone.Active.Count - 1; i >= 0; i--)
        {
            if (HazardZone.Active[i] != null)
            {
                Destroy(HazardZone.Active[i].gameObject);
            }
        }
    }

    void Update() { }
}