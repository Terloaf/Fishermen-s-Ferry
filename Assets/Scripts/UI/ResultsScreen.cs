using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultsScreen : MonoBehaviour
{
    public static ResultsScreen Instance { get; private set; }

    [SerializeField] private GameObject resultsScreen;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private TextMeshProUGUI loseText;
    [SerializeField] private TextMeshProUGUI fishCountText;
    [SerializeField] public int fishThershhold = 10;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        resultsScreen.SetActive(false);
    }

    public void ShowResults(bool survived)
    {
        if (resultsScreen == null)
        {
            Debug.LogError("ResultsScreen: 'resultsScreen' reference is not assigned in the inspector.");
            return;
        }

        resultsScreen.SetActive(true);

        if (winText != null)
        {
            winText.gameObject.SetActive(survived);
        }

        if (loseText != null)
        {
            loseText.gameObject.SetActive(!survived);
        }

        if (fishCountText != null)
        {
            int fishCount = FishCollectionManager.Instance != null ? FishCollectionManager.Instance.FishCount : 0;
            fishCountText.text = $"You collected {fishCount}/{fishThershhold} fishes";
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
