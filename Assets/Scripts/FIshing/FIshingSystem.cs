using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FIshingSystem : MonoBehaviour
{
    private bool isFishing;
    public AnimationClip castingLine;
    public Animator animator;

    [Header("Fish selection")]
    public List<GameObject> fishTypes = new List<GameObject>();
    public GameObject selectedFish;

    [Header("Bullet hell integration")]
    [SerializeField] private BullethellLogic bullethellLogic;
    [SerializeField] private CanvasGroup bulletHellCanvasGroup;
    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private float preChallengeBuffer = 1.5f; // pause after fade-in, before attacks start

    [Header("Main character control")]
    [SerializeField] private CharacterController mainCharacterController; // disabled while the bullet hell challenge is active

    private bool _challengeResultReceived;
    private bool _challengeSurvived;

    void Start()
    {
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
        if (Input.GetMouseButtonDown(0))
        {
            if (!isFishing)
            {
                bool isCasting = animator.GetBool("isCasting");
                animator.SetBool("isCasting", !isCasting);

                isFishing = true;
                StartCoroutine(FishingSequence());
            }
        }
    }

    private IEnumerator FishingSequence()
    {
        // Wait for a bite
        yield return StartCoroutine(WaitForBite());

        selectedFish = fishTypes[UnityEngine.Random.Range(0, fishTypes.Count)];

        // Run the bullet hell challenge before reeling the fish in
        yield return StartCoroutine(RunBulletHellChallenge());

        if (_challengeSurvived)
        {
            ReelInFish();
        }
        else
        {
            FishEscaped(); // the fish breaks free from the hook, nothing else happens
        }

        // Reset the rod/animation state so the player can cast again
        animator.SetBool("isCasting", false);
        isFishing = false;
    }

    private IEnumerator WaitForBite()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(5f, 10f));
    }

    private IEnumerator RunBulletHellChallenge()
    {
        _challengeResultReceived = false;

        // Disable the main character controller so the player can't move the boat/character mid-challenge
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
}
