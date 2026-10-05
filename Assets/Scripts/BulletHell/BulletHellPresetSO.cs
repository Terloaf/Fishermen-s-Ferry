using UnityEngine;

[CreateAssetMenu(menuName = "BulletHell/Preset")]
public class BulletHellPresetSO : ScriptableObject
{
    [System.Serializable]
    public struct PatternEntry
    {
        public BulletPatternSO pattern;
        public float delayBeforeNext; // time ("beat") before the next entry starts

        [Header("Per-entry overrides")]
        public BulletPatternSO.PatternOverrides overrides;
    }

    public float difficultyRating = 1f;
    public PatternEntry[] sequence;
    public bool shuffleSequence;
}