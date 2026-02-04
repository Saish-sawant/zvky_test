using UnityEngine;
using UnityEngine.UI;
namespace SpinWheel
{
    public class SplashUI : MonoBehaviour
    {
        [SerializeField] Slider progressBar;
        

        void Update()
        {
            if (GameManager.Instance == null)
                return;

            float p = GameManager.Instance.LoadProgress;
            progressBar.value = p;
            
        }
    }
}