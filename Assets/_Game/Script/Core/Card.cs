using UnityEngine;
using System.Collections;

public class Card : MonoBehaviour
{
    [SerializeField] public SpriteRenderer image;

    private CardConfig config;
    private Coroutine winRoutine;
    private Vector3 defaultScale;

    public int Id;
    public int Value;
    [SerializeField] Animation anim;
    private AnimatorOverrideController overrideController;


    void Awake()
    {
        defaultScale = transform.localScale;
        Animator animator = GetComponent<Animator>();
    }

    public void ApplyConfig(CardConfig newConfig)
    {
        config = newConfig;
        image.sprite = config.sprite;
        Id = config.id;
        Value = config.value;
        SetupAnimation();
    }
    void SetupAnimation()
    {
        if (config.winClip == null || anim == null)
            return;

        // Remove previous clips (pool-safe)
        anim.RemoveClip(config.winClip.name);

        // Add as legacy
        config.winClip.legacy = true;
        anim.AddClip(config.winClip, config.winClip.name);
    }
    // ---------------- WIN ANIMATION ----------------

    public void PlayWin()
    {
        StopWin();
        winRoutine = StartCoroutine(WinPulse());
        if (anim == null || config.winClip == null)
            return;

        anim.Play(config.winClip.name);
    }


    public void StopWin()
    {
        if (winRoutine != null)
        {
            StopCoroutine(winRoutine);
            winRoutine = null;
        }

        transform.localScale = defaultScale;
        if (anim == null)
            return;

        anim.Stop();
    }


    IEnumerator WinPulse()
    {
        float duration = 0.4f;      // total pulse time
        float elapsed = 0f;
        float scaleAmount = 0.15f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // 0 → 1 → 0 curve
            float t = elapsed / duration;
            float pulse = Mathf.Sin(t * Mathf.PI);

            transform.localScale = defaultScale * (1f + pulse * scaleAmount);
            yield return null;
        }

        // safety reset
        transform.localScale = defaultScale;
        winRoutine = null;
    }

}
