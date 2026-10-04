using UnityEngine;

public abstract class BulletPatternSO : ScriptableObject
{
    [Header("Common settings")]
    public float telegraphDuration = 0.5f; // warning time before the attack fires
    public GameObject warningPrefab;        // visual telegraph ("danger here")
    public GameObject attackPrefab;         // the actual projectile/wall/laser

    // Each pattern implements its own spawn logic
    public abstract System.Collections.IEnumerator Execute(BulletHellContext ctx);
}