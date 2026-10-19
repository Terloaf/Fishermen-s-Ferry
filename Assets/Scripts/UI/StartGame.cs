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

    [Header("Start sprite fade (world-space)")]
    [SerializeField] private SpriteRenderer fadeOutSprite; // sprite that disappears when the game starts
    [SerializeField] private SpriteRenderer fadeInSprite;  // sprite that appears when the game starts
    [SerializeField] private SpriteRenderer titleScreen;
    [SerializeField] private float fadeOutSpeed = 1f;      // alpha units per second
    [SerializeField] private float fadeInSpeed = 1f;

    public GameTimer gameTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiManager = GetComponent<UIManager>();
        gameTimer = GetComponent<GameTimer>();

        uiManager.gameplayCanvas.enabled = false;
        fishingSystem.enabled = false;

        if (characterController != null) characterController.CanRotate = false;

        // Make sure the sprites start in their expected visible/invisible state
        SetSpriteAlpha(fadeOutSprite, 1f);
        SetSpriteAlpha(fadeInSprite, 0f);

        AudioManager.Instance.Play("BoatAmbience");
        AudioManager.Instance.PlayMusic("WaterAmbience");
    }

    // Update is called once per frame
    void Update() { }

    public void OnGameStart()
    {
        AudioManager.Instance.Play("ButtonClick");
        animator.SetBool("Start", true);
        uiManager.startCanvas.enabled = false;
        PauseMenu.Instance.canPause = true; // allow pausing now that the game has started

        boatMover.BeginMoving(); // start the boat moving

        // Fade sprites in/out independently from the fishing/animation wait
        if (fadeOutSprite != null) StartCoroutine(FadeSprite(fadeOutSprite, 0f, fadeOutSpeed));
        if (titleScreen != null) StartCoroutine(FadeSprite(titleScreen, 0f, fadeOutSpeed));
        if (fadeInSprite != null) StartCoroutine(FadeSprite(fadeInSprite, 1f, fadeInSpeed));

        // Keep fishing disabled until the start animation has fully played
        StartCoroutine(EnableFishingAfterAnimation());
    }

    public void QuitGame()
    {
        Debug.Log("[StartGame] Quitting game.");
        Application.Quit();
    }

    private static void SetSpriteAlpha(SpriteRenderer sprite, float alpha)
    {
        if (sprite == null) return;
        Color c = sprite.color;
        c.a = alpha;
        sprite.color = c;
    }

    // Smoothly moves a SpriteRenderer's alpha towards targetAlpha at the given speed (alpha units per second)
    private IEnumerator FadeSprite(SpriteRenderer sprite, float targetAlpha, float speed)
    {
        while (!Mathf.Approximately(sprite.color.a, targetAlpha))
        {
            Color c = sprite.color;
            c.a = Mathf.MoveTowards(c.a, targetAlpha, speed * Time.deltaTime);
            sprite.color = c;
            yield return null;
        }

        SetSpriteAlpha(sprite, targetAlpha);
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

        fadeInSprite.gameObject.SetActive(false); // hide the fade-in sprite now that the game has started

        if (characterController != null) characterController.CanRotate = true;
    }
}
