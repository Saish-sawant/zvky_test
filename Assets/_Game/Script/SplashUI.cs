using UnityEngine;
using UnityEngine.UI;

public class SplashUI : MonoBehaviour
{
    [SerializeField] Slider progressBar;
    [SerializeField] TMPro.TextMeshProUGUI percentText;

    void Update()
    {
        if (GameManager.Instance == null)
            return;

        float p = GameManager.Instance.LoadProgress;
        progressBar.value = p;
        percentText.text = $"{Mathf.RoundToInt(p * 100)}%";
    }
}
