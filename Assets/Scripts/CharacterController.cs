using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 360f;

    // Blocks rotation input until the game has actually started (e.g. during the intro/start animation)
    public bool CanRotate { get; set; } = false;

    private Quaternion targetRotation;

    private void Start()
    {
        targetRotation = transform.rotation;
    }

    private void Update()
    {
        if (!CanRotate) return;

        if (Input.GetKeyDown(KeyCode.D))
        {
            targetRotation *= Quaternion.Euler(Vector3.up * 90);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            targetRotation *= Quaternion.Euler(Vector3.up * -90);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
