using System.Collections;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public FIshingSystem fishingSystem;
    public Animator animator;
    public UIManager uiManager;
    public BoatMover boatMover;
    public CharacterController characterController; // the player's rotation controller

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

        if (characterController != null) characterController.CanRotate = false;
    }

    // Update is called once per frame
    void Update() { }

    public void OnGameStart()
    {
        animator.SetBool("Start", true);
        uiManager.startCanvas.enabled = false;

        boatMover.BeginMoving(); // start the boat moving


        // Keep fishing disabled until the start animation has fully played
        StartCoroutine(EnableFishingAfterAnimation());
    }

    private IEnumerator EnableFishingAfterAnimation()
    {
        // Wait one frame so the Animator has a chance to transition into the start state
        yield return null;

        Debug.Log("[StartGame] Waiting to enter state: " + startStateName);

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(startStateName))
            yield return null;

        Debug.Log("[StartGame] Entered start state, waiting for it to finish.");

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f < 0.99f 
               && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;

        Debug.Log("[StartGame] Start animation finished, enabling gameplay canvas.");

        fishingSystem.enabled = true;
        uiManager.gameplayCanvas.enabled = true;
        gameTimer.StartTimer();

        if (characterController != null) characterController.CanRotate = true;
    }
}
