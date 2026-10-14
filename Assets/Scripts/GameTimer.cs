using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }

    public Image timerBar;
    [SerializeField] private float timerTime = 0;
    [SerializeField] private float maxTime;
    public bool isPlaying;

    public event System.Action OnTimerEnded;
    private bool _hasEnded;

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

    // Pauses the countdown without resetting it (e.g. during dialogue)
    public void PauseTimer()
    {
        isPlaying = false;
    }

    public void ResumeTimer()
    {
        if (!_hasEnded)
        {
            isPlaying = true;
        }
    }

    private void Update()
    {
        if (isPlaying == true)
        {
            timerTime -= Time.deltaTime;
            timerBar.fillAmount = timerTime / maxTime;

            if (timerTime <= 0f && !_hasEnded)
            {
                timerTime = 0f;
                isPlaying = false;
                _hasEnded = true;
                OnTimerEnded?.Invoke();
            }
        }
    }
}
