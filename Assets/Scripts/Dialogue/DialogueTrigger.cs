using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [System.Serializable]
    public struct TimedDialogueEntry
    {
        [Tooltip("Dialogue is used once the elapsed run time reaches this value (in seconds).")]
        public float minElapsedTime;
        public DialogueDataSO dialogue;
    }

    [Header("Default dialogue (used at the start of the run)")]
    public DialogueDataSO defaultDialogue;

    [Header("Timed variants (checked from highest threshold down)")]
    public TimedDialogueEntry[] timedEntries;

    public void StartDialogue()
    {
        var dataToUse = SelectDialogue();
        if (dataToUse == null) return;

        DialogueManager.Instance.BeginDialogue(dataToUse);
    }

    private DialogueDataSO SelectDialogue()
    {
        float elapsed = GameTimer.Instance != null ? GameTimer.Instance.ElapsedTime : 0f;

        DialogueDataSO best = defaultDialogue;
        float bestThreshold = -1f;

        foreach (var entry in timedEntries)
        {
            if (elapsed >= entry.minElapsedTime && entry.minElapsedTime > bestThreshold)
            {
                bestThreshold = entry.minElapsedTime;
                best = entry.dialogue;
            }
        }

        return best;
    }
}