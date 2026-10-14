using UnityEngine;

public class FishCollectionManager : MonoBehaviour
{
    public static FishCollectionManager Instance { get; private set; }

    public GameObject fishCollectableMenu;
    private bool isMenuOpen;
    public FishSlot[] fishSlot;

    [Header("Blocking conditions")]
    [SerializeField] private BullethellLogic bullethellLogic;

    [Header("Canvas references")]
    [SerializeField] private GameObject gameplayCanvas; // hidden while the collection menu is open

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isMenuOpen)
        {
            CloseMenu();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && !isMenuOpen && CanOpenMenu())
        {
            OpenMenu();
        }
    }

    private void OpenMenu()
    {
        Time.timeScale = 0;
        fishCollectableMenu.SetActive(true);
        if (gameplayCanvas != null) gameplayCanvas.SetActive(false);
        isMenuOpen = true;
    }

    private void CloseMenu()
    {
        Time.timeScale = 1;
        fishCollectableMenu.SetActive(false);
        if (gameplayCanvas != null) gameplayCanvas.SetActive(true);
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
}
