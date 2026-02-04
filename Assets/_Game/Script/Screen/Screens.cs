using System.Collections;
using UnityEngine;
namespace SpinWheel
{
    public abstract class Screens : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created


        public static GamePlay GamePlay { get; private set; }
        public static WelcomeScreen WelcomeScreen { get; private set; }
        public static InfoScreen InfoScreen { get; private set; }


        private void OnEnable()
        {
            switch (this)
            {
                case GamePlay gp:
                    GamePlay = gp;
                    break;
                case WelcomeScreen ws:
                    WelcomeScreen = ws;
                    break;
                case InfoScreen ins:
                    InfoScreen = ins;
                    break;

            }
        }

        public void InitializeScreen() => gameObject.SetActive(true);
        public void UnsetScreen() => gameObject.SetActive(false);

        public abstract IEnumerator EnterAsync(Screens previous);
        public abstract IEnumerator ExitAsync(Screens next);
        public abstract void OnBack();
    }
}