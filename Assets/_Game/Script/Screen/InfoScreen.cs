using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SpinWheel
{
    public class InfoScreen : Screens
    {
        [SerializeField] Button nextSlideBtn;
        [SerializeField] Button previousSlideBtn;
        [SerializeField] Button closeBtn;
        [SerializeField] GameObject[] slides;
        [SerializeField] Image[] highLights;          // 👈 Image is better than GameObject
        [SerializeField] Sprite[] highLightsSprite;   // [0] = inactive, [1] = active

        int currentSlide = 0;

        public override IEnumerator EnterAsync(Screens previous)
        {
            ShowSlide(0);
            yield return null;
        }

        public override IEnumerator ExitAsync(Screens next)
        {
            yield return null;
        }

        public override void OnBack()
        {
            // example
            // ScreenManager.Instance.ShowPrevious();
        }

        void Start()
        {
            nextSlideBtn.onClick.AddListener(NextSlide);
            previousSlideBtn.onClick.AddListener(PreviousSlide);
            closeBtn.onClick.AddListener(ClosePanel);
            ShowSlide(currentSlide);
        }

        void NextSlide()
        {
            if (currentSlide >= slides.Length - 1)
                return;

            currentSlide++;
            ShowSlide(currentSlide);
        }

        void PreviousSlide()
        {
            if (currentSlide <= 0)
                return;

            currentSlide--;
            ShowSlide(currentSlide);
        }

        void ShowSlide(int index)
        {
            for (int i = 0; i < slides.Length; i++)
            {
                slides[i].SetActive(i == index);
            }

            for (int i = 0; i < highLights.Length; i++)
            {
                highLights[i].sprite =
                    (i == index) ? highLightsSprite[1] : highLightsSprite[0];
            }

            // Optional: disable buttons at edges
            previousSlideBtn.interactable = index > 0;
            nextSlideBtn.interactable = index < slides.Length - 1;
        }
        void ClosePanel()
        {
            ScreenManager.Instance.SetScreen(Screens.GamePlay);
        }

    }
}
