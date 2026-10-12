using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction settings")]
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("References")]
    [SerializeField] private CharacterController characterController; // used to disable movement during dialogue

    // Prevents the same key press that closes a dialogue from immediately reopening it
    private bool _waitingForKeyRelease;

    void OnEnable()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueStarted += HandleDialogueStarted;
            DialogueManager.Instance.OnDialogueEnded += HandleDialogueEnded;
        }
    }

    void OnDisable()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueStarted -= HandleDialogueStarted;
            DialogueManager.Instance.OnDialogueEnded -= HandleDialogueEnded;
        }
    }

    void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
        {
            return;
        }

        if (_waitingForKeyRelease)
        {
            if (!Input.GetKey(interactKey))
            {
                _waitingForKeyRelease = false;
            }
            return;
        }

        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        // Cast forward from the player in the direction they're currently facing
        if (Physics.Raycast(transform.position, -transform.forward, out RaycastHit hit, interactRange, interactableLayer))
        {
            var trigger = hit.collider.GetComponent<DialogueTrigger>();
            if (trigger != null)
            {
                trigger.StartDialogue();
            }
        }
    }

    private void HandleDialogueStarted()
    {
        if (characterController != null) characterController.enabled = false;
    }

    private void HandleDialogueEnded()
    {
        if (characterController != null) characterController.enabled = true;

        // Require the key to be released before it can trigger a new interaction
        _waitingForKeyRelease = true;
    }

    // Visualizes the interaction ray in the Scene view for easier setup
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * interactRange);
    }
}