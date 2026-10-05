using System.Collections;
using UnityEngine;

public class BullethellLogic : MonoBehaviour
{
    [SerializeField] private BulletHellPresetSO[] availablePresets;
    [SerializeField] private BulletHellContext context;
    [SerializeField] private BH_PlayerController playerController;

    private BulletHellPresetSO _currentPreset;

    void Start()
    {
        _currentPreset = PickPreset();
        StartCoroutine(RunPreset(_currentPreset));
        playerController.OnPlayerDied += HandlePlayerDied;
    }

    private BulletHellPresetSO PickPreset()
    {
        // Could be filtered by fish rarity / difficulty level
        return availablePresets[Random.Range(0, availablePresets.Length)];
    }

    private IEnumerator RunPreset(BulletHellPresetSO preset)
    {
        foreach (var entry in preset.sequence)
        {
            StartCoroutine(entry.pattern.Execute(context, entry.overrides));
            yield return new WaitForSeconds(entry.delayBeforeNext);
        }

        // Challenge complete — if the player survived, the fish is caught
        OnChallengeSurvived();
    }

    private void OnChallengeSurvived()
    {
        // Signal back to the fishing gameplay loop
    }

    void HandlePlayerDied()
    {
        StopAllCoroutines();
        // show "fish escaped" / failure state
    }

    void Update() { }
}