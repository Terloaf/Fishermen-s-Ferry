using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel; // root of the dialogue box, starts disabled
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image portraitImage;

    [Header("Typewriter settings")]
    [SerializeField] private float charactersPerSecond = 30f;

    public event System.Action OnDialogueStarted;
    public event System.Action OnDialogueEnded;

    private DialogueDataSO _currentDialogue;
    private int _currentLineIndex;
    private Coroutine _typeRoutine;
    private bool _isTyping;

    public bool IsDialogueActive { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (!IsDialogueActive) return;

        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            if (_isTyping)
            {
                // First press while typing: instantly reveal the full line instead of advancing
                SkipTyping();
            }
            else
            {
                AdvanceLine();
            }
        }
    }

    public void BeginDialogue(DialogueDataSO data)
    {
        if (IsDialogueActive) return;

        _currentDialogue = data;
        _currentLineIndex = 0;
        IsDialogueActive = true;

        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        if (GameTimer.Instance != null) GameTimer.Instance.PauseTimer();

        OnDialogueStarted?.Invoke();
        ShowCurrentLine();
    }

    private void AdvanceLine()
    {
        _currentLineIndex++;

        if (_currentLineIndex >= _currentDialogue.lines.Length)
        {
            EndDialogue();
        }
        else
        {
            ShowCurrentLine();
        }
    }

    private void ShowCurrentLine()
    {
        var line = _currentDialogue.lines[_currentLineIndex];

        if (speakerNameText != null) speakerNameText.text = line.speakerName;

        if (portraitImage != null)
        {
            portraitImage.enabled = line.portrait != null;
            portraitImage.sprite = line.portrait;
        }

        if (_typeRoutine != null) StopCoroutine(_typeRoutine);
        _typeRoutine = StartCoroutine(TypeLine(line.text));
    }

    private IEnumerator TypeLine(string fullText)
    {
        _isTyping = true;
        dialogueText.text = string.Empty;

        float delay = 1f / Mathf.Max(charactersPerSecond, 1f);

        foreach (char c in fullText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(delay);
        }

        _isTyping = false;
        _typeRoutine = null;
    }

    private void SkipTyping()
    {
        if (_typeRoutine != null) StopCoroutine(_typeRoutine);
        _typeRoutine = null;
        _isTyping = false;

        dialogueText.text = _currentDialogue.lines[_currentLineIndex].text;
    }

    private void EndDialogue()
    {
        IsDialogueActive = false;
        _currentDialogue = null;

        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        if (GameTimer.Instance != null) GameTimer.Instance.ResumeTimer();

        OnDialogueEnded?.Invoke();
    }
}