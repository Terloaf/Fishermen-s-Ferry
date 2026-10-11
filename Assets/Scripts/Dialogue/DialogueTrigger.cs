using UnityEngine;

// Attach this to the spirit / NPC the player can talk to
public class DialogueTrigger : MonoBehaviour
{
    public DialogueDataSO dialogueData;

    public void StartDialogue()
    {
        if (dialogueData == null) return;
        DialogueManager.Instance.BeginDialogue(dialogueData);
    }
}