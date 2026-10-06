using Unity.VisualScripting;
using UnityEngine;

public class CharacterController : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            this.gameObject.transform.Rotate(Vector3.up * 90);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            this.gameObject.transform.Rotate(Vector3.up * -90);
        }
    }


}
