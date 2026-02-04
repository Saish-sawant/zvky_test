using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SpinWheel
{
    public class ScreenManager : MonoBehaviour
    {
        public static ScreenManager Instance { get; set; }
        public Screens CurrentScreen { get; private set; }
        private Screens _nextScreen;
        private Coroutine _switchCoroutine;
        private Screens[] _screens;
    

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            Instance = this;
            _screens = GetComponentsInChildren<Screens>();
        }

        private void Start()
        {
            HideAllScreens();
            SetScreen(Screens.WelcomeScreen);
            
        }

       

        public void SetScreen(Screens screen)
        {
            // Highlight.SetActive(DeviceDetector.IsFireTV());
            if (!CurrentScreen)
            {
                CurrentScreen = screen;
                screen.InitializeScreen();
                StartCoroutine(screen.EnterAsync(null));
                return;
            }

            // Prepare to set screen
            _nextScreen = screen;

            // Already screen switch is in progress. Try again later
            if (_switchCoroutine != null) return;

            // if (screen.Equals(CurrentScreen)) return;

            // Start switching process
            _switchCoroutine = StartCoroutine(SetScreenAsync(screen));

            // No need to cache now
            _nextScreen = null;
        }

        // Update is called once per frame
        void Update()
        {
            if (_nextScreen && _nextScreen != CurrentScreen) SetScreen(_nextScreen);

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnBack();
            }
        }

        private void OnBack()
        {
            CurrentScreen.OnBack();
        }

        private void HideAllScreens()
        {
            foreach (var screen in _screens)
            {
                screen.gameObject.SetActive(false);
            }
        }

        private IEnumerator SetScreenAsync(Screens screen)
        {
            yield return CurrentScreen.ExitAsync(screen);
            CurrentScreen.UnsetScreen();

            var prevScreen = CurrentScreen;
            CurrentScreen = screen;
            screen.InitializeScreen();
            yield return screen.EnterAsync(prevScreen);
            _switchCoroutine = null;
        }


    }
}