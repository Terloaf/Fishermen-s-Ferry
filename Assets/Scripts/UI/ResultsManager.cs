using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultsManager : MonoBehaviour
{
    public static ResultsManager Instance { get; private set; }

    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private BullethellLogic bullethellLogic;
    [SerializeField] private GameObject gameplayCanvas;

    [Header("Results UI")]
    [SerializeField] private FishCollectionManager fishCollectionManager;
    [SerializeField] private TMP_Text fishCountText;
    [SerializeField] private TMP_Text resultMessageText;
    [SerializeField] private int requiredFishQuota = 10; // how many fish needed to "win"
    [SerializeField] private string successMessage = "Time's up! Great haul!";
    [SerializeField] private string failMessage = "The run ended early...";

    public bool IsResultsShown { get; private set; }

    private bool _timerEnded;
    private bool _subscribed;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (resultsPanel != null) resultsPanel.SetActive(false);
    }

    void Start()
    {
        // Subscribing in Start() guarantees GameTimer.Awake() (which sets GameTimer.Instance) has already run
        TrySubscribe();
    }

    void OnDisable()
    {
        Unsubscribe();
    }

    private void TrySubscribe()
    {
        if (_subscribed) return;

        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.OnTimerEnded += HandleTimerEnded;
        }

        if (bullethellLogic != null)
        {
            bullethellLogic.OnChallengeSurvived += HandleChallengeSurvived;
            bullethellLogic.OnChallengeFailed += HandleChallengeFailed;
        }

        _subscribed = true;
    }

    private void Unsubscribe()
    {
        if (!_subscribed) return;

        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.OnTimerEnded -= HandleTimerEnded;
        }

        if (bullethellLogic != null)
        {
            bullethellLogic.OnChallengeSurvived -= HandleChallengeSurvived;
            bullethellLogic.OnChallengeFailed -= HandleChallengeFailed;
        }

        _subscribed = false;
    }

    private void HandleTimerEnded()
    {
        Debug.Log("[ResultsManager] Timer ended received.");
        _timerEnded = true;

        // If a bullet hell challenge is currently running, wait for it to finish first
        if (bullethellLogic != null && bullethellLogic.IsChallengeActive)
        {
            return;
        }

        ShowResults();
    }

    private void HandleChallengeSurvived()
    {
        if (_timerEnded) ShowResults();
    }

    private void HandleChallengeFailed()
    {
        if (_timerEnded) ShowResults();
    }

    private void ShowResults()
    {
        IsResultsShown = true;

        if (resultsPanel != null)
        {
            resultsPanel.SetActive(true);
        }

        if (gameplayCanvas != null)
        {
            gameplayCanvas.SetActive(false);
        }

        int caughtFish = fishCollectionManager != null ? fishCollectionManager.FishCount : 0;

        if (fishCountText != null)
        {
            fishCountText.text = $"Fish caught: {caughtFish}/{requiredFishQuota}";
        }

        bool quotaMet = caughtFish >= requiredFishQuota;

        if (resultMessageText != null)
        {
            resultMessageText.text = quotaMet ? successMessage : failMessage;
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}