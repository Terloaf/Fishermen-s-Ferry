using UnityEngine;

public abstract class BulletPatternSO : ScriptableObject
{
    [Header("Common settings")]
    public float telegraphDuration = 0.5f;
    public GameObject warningPrefab;
    public GameObject attackPrefab;

    public enum SpawnSide { Top, Bottom, Left, Right }

    [System.Serializable]
    public struct PatternOverrides
    {
        // Used by Wall (.x only) and Circle (.x and .y as center).
        // For Projectile, .x is used as the normalized offset along the spawn edge.
        public bool overridePosition;

        [Range01Vector2]
        public Vector2 position;

        public bool overrideOrientation;
        public WallPatternSO.Orientation orientation;

        public bool overrideRadius;
        public float radius;

        [Header("Projectile-specific overrides")]
        public bool overrideSize;
        public float size;

        public bool overrideSpeed;
        public float speed;

        public bool overrideSide;
        public SpawnSide side;
    }

    public abstract System.Collections.IEnumerator Execute(BulletHellContext ctx, PatternOverrides overrides);
}