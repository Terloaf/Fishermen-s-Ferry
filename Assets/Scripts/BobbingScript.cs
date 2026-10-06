using UnityEngine;

public class BobbingScript : MonoBehaviour
{
    private Vector3 startPos;
    public float bobSpeed;
    public float bobDistance;

   

    private void Start()
    {
        startPos = transform.localPosition;
    }
    void Update()
    {
        float offset = Mathf.Sin(Time.time * bobSpeed) * bobDistance;

        transform.localPosition = startPos + Vector3.up.normalized * offset;
    }
}
