using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FIshingSystem : MonoBehaviour
{
    private bool isFishing;
    public AnimationClip castingLine;
    public Animator animator;

    [Header("Fish selection")]
    public List<GameObject> fishTypes = new List<GameObject>();
    public GameObject selectedFish;
    [SerializeField]
    private FishCollectionManager fishCollectionManager;

    [Header("Bullet hell integration")]
    [SerializeField] private BullethellLogic bullethellLogic;
    [SerializeField] private CanvasGroup bulletHellCanvasGroup;
    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private float preChallengeBuffer = 1.5f; // pause after fade-in, before attacks start

    [Header("Main character control")]
    [SerializeField] private CharacterController mainCharacterController; // disabled while the bullet hell challenge is active

    [Header("Facing restriction")]
    [Tooltip("Fishing is only allowed when the character's Y rotation is at this angle (degrees).")]
    [SerializeField] private float requiredFacingAngle = 0f;
    [SerializeField] private float facingAngleTolerance = 1f;

    [Header("Bobber")]
    [SerializeField] private Transform bobberTransform;
    private Vector3 _bobberStartLocalPosition;
    private Quaternion _bobberStartLocalRotation;

    [Header("UI Hints")]
    [SerializeField] private GameObject fishingHintUI;
    [SerializeField] private TMP_Text fishingHintText;
    [SerializeField] private string fishingHintMessage = "Press LMB to catch fish";

    private bool _challengeResultReceived;
    private bool _challengeSurvived;

    void Start()
    {
        fishCollectionManager = GameObject.Find("CollectionCanvas").GetComponent<FishCollectionManager>();

        if (bobberTransform != null)
        {
            _bobberStartLocalPosition = bobberTransform.localPosition;
            _bobberStartLocalRotation = bobberTransform.localRotation;
        }

        // Ensure the bullet hell canvas starts fully hidden and non-interactive
        if (bulletHellCanvasGroup != null)
        {
            bulletHellCanvasGroup.alpha = 0f;
            bulletHellCanvasGroup.gameObject.SetActive(false);
            bulletHellCanvasGroup.interactable = false;
            bulletHellCanvasGroup.blocksRaycasts = false;
        }
    }

    void OnEnable()
    {
        if (bullethellLogic != null)
        {
            bullethellLogic.OnChallengeSurvived += HandleChallengeSurvived;
            bullethellLogic.OnChallengeFailed += HandleChallengeFailed;
        }
    }

    void OnDisable()
    {
        if (bullethellLogic != null)
        {
            bullethellLogic.OnChallengeSurvived -= HandleChallengeSurvived;
            bullethellLogic.OnChallengeFailed -= HandleChallengeFailed;
        }
    }

    void Update()
    {
        UpdateFishingHint();

        if (Input.GetMouseButtonDown(0))
        {
            if (!isFishing && IsFacingCorrectDirection() && !IsResultsShown())
            {
                bool isCasting = animator.GetBool("isCasting");
                animator.SetBool("isCasting", !isCasting);

                isFishing = true;
                StartCoroutine(FishingSequence());
            }
        }
    }

    // Checks whether the character is currently facing the side where fishing is allowed
    private bool IsFacingCorrectDirection()
    {
        if (mainCharacterController == null) return true; // fallback: no restriction if not assigned

        float currentAngle = mainCharacterController.transform.eulerAngles.y;
        float difference = Mathf.Abs(Mathf.DeltaAngle(currentAngle, requiredFacingAngle));

        return difference <= facingAngleTolerance;
    }

    private IEnumerator FishingSequence()
    {
        // Prevent turning while the player is waiting for a bite
        if (mainCharacterController != null) mainCharacterController.enabled = false;

        // Wait for a bite
        yield return StartCoroutine(WaitForBite());

        if (mainCharacterController != null) mainCharacterController.enabled = true;

        // If every fish type has already been caught, skip the bullet hell challenge entirely
        if (fishTypes.Count == 0)
        {
            Debug.Log("All fish types have already been caught!");
            animator.SetBool("isCasting", false);
            isFishing = false;
            yield break;
        }

        selectedFish = fishTypes[UnityEngine.Random.Range(0, fishTypes.Count)];

        // Run the bullet hell challenge before reeling the fish in
        yield return StartCoroutine(RunBulletHellChallenge());

        if (_challengeSurvived)
        {
            ReelInFish();
            fishCollectionManager.AddFish(selectedFish.ToString(), selectedFish.GetComponent<SpriteRenderer>());
            fishTypes.Remove(selectedFish);
        }
        else
        {
            FishEscaped();
        }

        animator.SetBool("isCasting", false);
        animator.Play("Idle", 0, 0f); // force-snap Animator to the idle state's first frame
        ResetBobberPosition();
        isFishing = false;
    }

    private IEnumerator WaitForBite()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(5f, 10f));
    }

    private IEnumerator RunBulletHellChallenge()
    {
        _challengeResultReceived = false;

        // Already disabled during the bite wait, but keep disabled through the challenge too
        if (mainCharacterController != null)
        {
            mainCharacterController.enabled = false;
        }

        if (bulletHellCanvasGroup != null)
        {
            bulletHellCanvasGroup.gameObject.SetActive(true);
            bulletHellCanvasGroup.interactable = true;
            bulletHellCanvasGroup.blocksRaycasts = true;
            bulletHellCanvasGroup.alpha = 0f; // ensure it starts invisible even though it's already active
        }

        // Reset player position/health right as the canvas starts becoming visible,
        // so any pre-buffer movement doesn't get overwritten by a late reset/teleport.
        bullethellLogic.PrepareForChallenge();

        if (bulletHellCanvasGroup != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(bulletHellCanvasGroup, 0f, 1f));
        }

        // Give the player a moment to get oriented before the attacks start
        yield return new WaitForSeconds(preChallengeBuffer);

        bullethellLogic.StartChallenge();

        // Wait until the bullet hell logic reports a result via events
        while (!_challengeResultReceived)
        {
            yield return null;
        }

        // Fade the bullet hell canvas out
        if (bulletHellCanvasGroup != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(bulletHellCanvasGroup, 1f, 0f));
            bulletHellCanvasGroup.interactable = false;
            bulletHellCanvasGroup.blocksRaycasts = false;
            bulletHellCanvasGroup.gameObject.SetActive(false);
        }

        // Re-enable the main character controller now that the challenge is over
        if (mainCharacterController != null)
        {
            mainCharacterController.enabled = true;
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to)
    {
        float elapsed = 0f;
        group.alpha = from;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        group.alpha = to;
    }

    private void HandleChallengeSurvived()
    {
        _challengeSurvived = true;
        _challengeResultReceived = true;
    }

    private void HandleChallengeFailed()
    {
        _challengeSurvived = false;
        _challengeResultReceived = true;
    }

    private void ReelInFish()
    {
        // TODO: instantiate/display selectedFish, add to inventory, play reel-in animation, etc.
        Debug.Log($"Caught: {selectedFish.name}");
    }

    private void FishEscaped()
    {
        // The fish breaks free from the hook; intentionally no reward/feedback beyond the rod resetting
        Debug.Log("The fish got away!");
    }

    private void ResetBobberPosition()
    {
        if (bobberTransform == null) return;

        bobberTransform.localPosition = _bobberStartLocalPosition;
        bobberTransform.localRotation = _bobberStartLocalRotation;
    }

    private bool IsResultsShown()
    {
        return ResultsManager.Instance != null && ResultsManager.Instance.IsResultsShown;
    }

    private void UpdateFishingHint()
    {
        if (fishingHintUI == null) return;

        bool shouldShow = !isFishing && IsFacingCorrectDirection() && !IsResultsShown();
        fishingHintUI.SetActive(shouldShow);

        if (shouldShow && fishingHintText != null)
        {
            fishingHintText.text = fishingHintMessage;
        }
    }
}
