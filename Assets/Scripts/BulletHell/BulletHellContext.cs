using UnityEngine;

public class BulletHellContext : MonoBehaviour
{
    public Transform player;
    public Rect arenaBounds;

    public Vector3 SpawnPoint(float normalizedPosition)
    {
        float x = Mathf.Lerp(arenaBounds.xMin, arenaBounds.xMax, normalizedPosition);
        return new Vector3(x, arenaBounds.yMax, 0);
    }

    public Vector3 PlayerDirection => player.position;
}