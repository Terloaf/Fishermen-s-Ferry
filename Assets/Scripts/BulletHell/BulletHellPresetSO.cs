using UnityEngine;

[CreateAssetMenu(menuName = "BulletHell/Preset")]
public class BulletHellPresetSO : ScriptableObject
{
    [System.Serializable]
    public struct PatternEntry
    {
        public BulletPatternSO pattern;
        public float delayBeforeNext; // time ("beat") before the next entry starts
    }

    public float difficultyRating = 1f;
    public PatternEntry[] sequence;

    // Optional: randomize order / pull patterns from a shared pool
    public bool shuffleSequence;
}