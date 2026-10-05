using UnityEngine;

public abstract class BulletPatternSO : ScriptableObject
{
    [Header("Common settings")]
    public float telegraphDuration = 0.5f;
    public GameObject warningPrefab;
    public GameObject attackPrefab;

    [System.Serializable]
    public struct PatternOverrides
    {
        public bool overridePosition;

        [Range01Vector2]
        public Vector2 position;

        public bool overrideOrientation;
        public WallPatternSO.Orientation orientation;

        public bool overrideRadius;
        public float radius;
    }

    public abstract System.Collections.IEnumerator Execute(BulletHellContext ctx, PatternOverrides overrides);
}