using UnityEngine;
using System.Collections;

public class Card : MonoBehaviour
{
    [SerializeField] SpriteRenderer image;
    

    private CardConfig config;
    

    private Coroutine winRoutine;
    private Vector3 defaultScale;

    public int Id;
    public int Value;

    void Awake()
    {
        defaultScale = transform.localScale;

        
    }

    public void ApplyConfig(CardConfig newConfig)
    {
        config = newConfig;

        image.sprite = config.sprite;
        Id = config.id;
        Value = config.value;

        
    }

    
    // ---------------- WIN ----------------

    public void PlayWin()
    {
        StopWin();

        

        winRoutine = StartCoroutine(WinPulse());
    }

    public void StopWin()
    {
        if (winRoutine != null)
        {
            StopCoroutine(winRoutine);
            winRoutine = null;
        }

        transform.localScale = defaultScale;
       
    }

    IEnumerator WinPulse()
    {
        float duration = 0.4f;
        float scaleAmount = 0.15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float pulse = Mathf.Sin(t * Mathf.PI);

            transform.localScale =
                defaultScale * (1f + pulse * scaleAmount);

            yield return null;
        }

        transform.localScale = defaultScale;
        winRoutine = null;
    }
}
