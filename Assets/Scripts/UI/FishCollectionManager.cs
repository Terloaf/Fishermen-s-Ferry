using UnityEngine;
using UnityEngine.UI;

public class FishCollectionManager : MonoBehaviour
{
    public static FishCollectionManager Instance { get; private set; }

    public GameObject fishCollectableMenu;
    [SerializeField] GameObject talkButton;
    private bool isMenuOpen;
    public FishSlot[] fishSlot;

    [Header("Blocking conditions")]
    [SerializeField] private BullethellLogic bullethellLogic;

    [Header("Canvas references")]
    [SerializeField] private GameObject gameplayCanvas; // hidden while the collection menu is open

    [System.Serializable]
    public struct FishDialogueEntry
    {
        public string fishName;
        public DialogueDataSO dialogue;
    }

    [Header("Fish dialogues")]
    [Tooltip("Maps each fish name to the dialogue that should play when talking to it.")]
    [SerializeField] private FishDialogueEntry[] fishDialogues;

    [Header("Talk hands")]
    [Tooltip("Animator controlling the hands sprite shown while talking to a fish.")]
    [SerializeField] private TalkHandsAnimator handsAnimator;

    private FIshingSystem fishingSystem;

    public int FishCount
    {
        get
        {
            int count = 0;
            foreach (var slot in fishSlot)
            {
                if (slot.isFilled)
                {
                    count++;
                }
            }
            return count;
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnded += HideHands;
        }
    }

    private void OnDisable()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnded -= HideHands;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fishingSystem = GameObject.Find("GameManager").GetComponent<FIshingSystem>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && isMenuOpen)
        {
            CloseMenu();
        }
        else if (Input.GetKeyDown(KeyCode.Q) && !isMenuOpen && CanOpenMenu())
        {
            OpenMenu();
        }
    }

    private void OpenMenu()
    {
        Time.timeScale = 0;
        fishCollectableMenu.SetActive(true);
        talkButton.SetActive(true);
        if (gameplayCanvas != null) gameplayCanvas.SetActive(false);
        fishingSystem.isFishing = true;
        isMenuOpen = true;
    }

    private void CloseMenu()
    {
        Time.timeScale = 1;
        fishCollectableMenu.SetActive(false);
        talkButton.SetActive(false);
        if (gameplayCanvas != null) gameplayCanvas.SetActive(true);
        fishingSystem.isFishing = false;
        isMenuOpen = false;
    }

    // Blocks opening the collection menu while a bullet hell challenge is active or results are being shown
    private bool CanOpenMenu()
    {
        if (bullethellLogic != null && bullethellLogic.IsChallengeActive) return false;
        if (ResultsManager.Instance != null && ResultsManager.Instance.IsResultsShown) return false;

        return true;
    }

    public void AddFish(string fishName, SpriteRenderer fishSprite)
    {
        Debug.Log("FishName = " + fishName + "Sprite = " + fishSprite);
        for (int i = 0; i < fishSlot.Length; i++)
        {
            if (fishSlot[i].isFilled == false)
            {
                fishSlot[i].AddFish(fishName, fishSprite);
                return;
            }
        }
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < fishSlot.Length; i++)
        {
            fishSlot[i].selectedSahder.SetActive(false);
            fishSlot[i].thisFishSelected = false;
        }
    }

    // Called by the "Talk" button. Finds the currently selected filled slot,
    // looks up the dialogue matching its fish, starts it, and closes the collection menu.
    public void TalkToSelectedFish()
    {
        FishSlot selectedSlot = null;

        for (int i = 0; i < fishSlot.Length; i++)
        {
            if (fishSlot[i].thisFishSelected && fishSlot[i].isFilled)
            {
                selectedSlot = fishSlot[i];
                break;
            }
        }

        if (selectedSlot == null)
        {
            Debug.Log("[FishCollectionManager] No fish slot selected, cannot talk.");
            return;
        }

        DialogueDataSO dialogueToPlay = FindDialogueForFish(selectedSlot.fishName);

        if (dialogueToPlay == null)
        {
            Debug.LogWarning($"[FishCollectionManager] No dialogue found for fish '{selectedSlot.fishName}'.");
            return;
        }

        ShowHands(selectedSlot);

        CloseMenu();

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnded -= HideHands;
            DialogueManager.Instance.OnDialogueEnded += HideHands;
            DialogueManager.Instance.BeginDialogue(dialogueToPlay);
        }
    }

    private void ShowHands(FishSlot slot)
    {
        if (handsAnimator == null) return;

        Sprite sprite = slot.image != null ? slot.image.sprite : null;
        handsAnimator.Show(sprite);
    }

    private void HideHands()
    {
        if (handsAnimator != null)
        {
            handsAnimator.Hide();
        }
    }

    private DialogueDataSO FindDialogueForFish(string fishName)
    {
        if (fishDialogues == null) return null;

        foreach (var entry in fishDialogues)
        {
            if (entry.fishName == fishName)
            {
                return entry.dialogue;
            }
        }

        return null;
    }
}
