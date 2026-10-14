using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction settings")]
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("References")]
    [SerializeField] private CharacterController characterController; // used to disable movement during dialogue

    [Header("UI Hints")]
    [SerializeField] private GameObject dialogueHintUI;

    private bool _waitingForKeyRelease;
    private DialogueTrigger _currentTarget;

    void OnEnable()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueStarted += HandleDialogueStarted;
            DialogueManager.Instance.OnDialogueEnded += HandleDialogueEnded;
        }

        if (dialogueHintUI != null) dialogueHintUI.SetActive(false);
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
            SetHintVisible(false);
            return;
        }

        UpdateInteractionHint();

        if (_waitingForKeyRelease)
        {
            if (!Input.GetKey(interactKey))
            {
                _waitingForKeyRelease = false;
            }
            return;
        }

        if (Input.GetKeyDown(interactKey) && _currentTarget != null)
        {
            _currentTarget.StartDialogue();
        }
    }

    // Checks each frame whether a DialogueTrigger is currently in front of the player, and shows/hides the hint
    private void UpdateInteractionHint()
    {
        if (Physics.Raycast(transform.position, -transform.forward, out RaycastHit hit, interactRange, interactableLayer))
        {
            var trigger = hit.collider.GetComponent<DialogueTrigger>();
            if (trigger != null)
            {
                _currentTarget = trigger;
                SetHintVisible(true);
                return;
            }
        }

        _currentTarget = null;
        SetHintVisible(false);
    }

    private void SetHintVisible(bool visible)
    {
        if (dialogueHintUI != null) dialogueHintUI.SetActive(visible);
    }

    private void HandleDialogueStarted()
    {
        if (characterController != null) characterController.enabled = false;
        SetHintVisible(false);
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