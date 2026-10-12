using System.Collections;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public FIshingSystem fishingSystem;
    public Animator animator;
    public UIManager uiManager;

    [Tooltip("Name of the Animator state that plays when the game starts (used to detect when it finishes).")]
    [SerializeField] private string startStateName = "Start";

    public GameTimer gameTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiManager = GetComponent<UIManager>();
        gameTimer = GetComponent<GameTimer>();

        uiManager.gameplayCanvas.enabled = false;
        fishingSystem.enabled = false;
    }

    // Update is called once per frame
    void Update() { }

    public void OnGameStart()
    {
        animator.SetBool("Start", true);
        uiManager.startCanvas.enabled = false;
       
       

        

        // Keep fishing disabled until the start animation has fully played
        StartCoroutine(EnableFishingAfterAnimation());
    }

    private IEnumerator EnableFishingAfterAnimation()
    {
        // Wait one frame so the Animator has a chance to transition into the start state
        yield return null;

        // Wait until the Animator enters the expected state
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(startStateName))
        {
            yield return null;
        }

        // Wait until that state's normalized time reaches 1 (i.e. the clip finished playing once)
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        fishingSystem.enabled = true;
        uiManager.gameplayCanvas.enabled = true;
        gameTimer.StartTimer();
    }
}
