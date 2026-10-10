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
    [SerializeField] private CanvasGroup bulletHellCanvasGroup; // on the bullet hell canvas prefab, starts disabled
    [SerializeField] private float fadeDuration = 0.4f;

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
            FishEscaped();
        }

        isFishing = false;
    }

    private IEnumerator WaitForBite()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(5f, 10f));
    }

    private IEnumerator RunBulletHellChallenge()
    {
        _challengeResultReceived = false;

        // Fade the bullet hell canvas in
        if (bulletHellCanvasGroup != null)
        {
            bulletHellCanvasGroup.gameObject.SetActive(true);
            bulletHellCanvasGroup.interactable = true;
            bulletHellCanvasGroup.blocksRaycasts = true;
            yield return StartCoroutine(FadeCanvasGroup(bulletHellCanvasGroup, 0f, 1f));
        }

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
        // TODO: play "fish got away" feedback
        Debug.Log("The fish got away!");
    }
}
