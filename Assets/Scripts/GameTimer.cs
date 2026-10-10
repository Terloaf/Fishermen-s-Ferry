using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public Image timerBar;
    [SerializeField]private float timerTime = 0;
    [SerializeField]private float maxTime;
    public bool isPlaying;

    void Start()
    {
        timerTime = maxTime;
    }
    public void StartTimer()
    {
        isPlaying = true;
    }

    private void Update()
    {
        if(isPlaying == true)
        {
            timerTime -= Time.deltaTime;
            timerBar.fillAmount = timerTime / maxTime;
        }
    }
}
