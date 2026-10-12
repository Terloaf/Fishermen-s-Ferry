using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Dialogue Data")]
public class DialogueDataSO : ScriptableObject
{
    [System.Serializable]
    public struct DialogueLine
    {
        public string speakerName;
        [TextArea(2, 5)] public string text;
        public Sprite portrait; // optional, can be left empty
    }

    public DialogueLine[] lines;
}