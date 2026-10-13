using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }

    public Image timerBar;
    [SerializeField] private float timerTime = 0;
    [SerializeField] private float maxTime;
    public bool isPlaying;

    // How much time has passed since the timer started (in seconds)
    public float ElapsedTime => maxTime - timerTime;

    // How much time is left (in seconds)
    public float RemainingTime => timerTime;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        
        timerTime = maxTime;
    }

    public void StartTimer()
    {
        isPlaying = true;
    }

    private bool resultsShown;

    private void Update()
    {
        if (isPlaying == true)
        {
            timerTime -= Time.deltaTime;
            timerBar.fillAmount = timerTime / maxTime;
        }

        if (timerTime <= 0 && !resultsShown)
        {
            isPlaying = false;
            resultsShown = true;

            if (ResultsScreen.Instance == null || FishCollectionManager.Instance == null)
            {
                Debug.LogError("GameTimer: Missing ResultsScreen or FishCollectionManager instance in scene.");
                return;
            }

            ResultsScreen.Instance.ShowResults(FishCollectionManager.Instance.FishCount >= ResultsScreen.Instance.fishThershhold);
        }
    }
}
