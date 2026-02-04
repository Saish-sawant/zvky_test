using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpinWheel
{
    public class GamePlay : Screens
    {
        [SerializeField] Button infoBtn;

        [SerializeField] Button spinBtn;
        [SerializeField] Button betUp, BetDown;
        const float MIN_BET = 0.1f;
        const float MAX_BET = 25f;
        const float BET_STEP = 0.1f;



        [SerializeField] TMP_Text balanceText;
        [SerializeField] TMP_Text betText;
        [SerializeField] TMP_Text winningsText;

        float balance = 1000f;
        float currentBet = 1f;

        void Start()
        {
            spinBtn.onClick.AddListener(OnSpinClicked);
            infoBtn.onClick.AddListener(OpenInfo);


            betUp.onClick.AddListener(IncreaseBet);
            BetDown.onClick.AddListener(DecreaseBet);
        }



        void OnDestroy()
        {
            if (ReelsManager.Instance == null) return;

            ReelsManager.Instance.OnSpinStarted -= OnSpinStarted;
            ReelsManager.Instance.OnSpinResult -= OnSpinResult;
        }
        void IncreaseBet()
        {
            currentBet += BET_STEP;
            currentBet = Mathf.Round(currentBet * 10f) / 10f; // fixes float issues
            currentBet = Mathf.Clamp(currentBet, MIN_BET, MAX_BET);

            RefreshUI();
        }

        void DecreaseBet()
        {
            currentBet -= BET_STEP;
            currentBet = Mathf.Round(currentBet * 10f) / 10f;
            currentBet = Mathf.Clamp(currentBet, MIN_BET, MAX_BET);

            RefreshUI();
        }

        void OnSpinClicked()
        {
            if (balance < currentBet)
                return;

            balance -= currentBet;
            winningsText.text = "0";

            ReelsManager.Instance.SetBet(currentBet);
            ReelsManager.Instance.SpinAll();

            RefreshUI();
        }

        void OnSpinStarted(float bet)
        {
            spinBtn.interactable = false;
            betUp.interactable = false;
            BetDown.interactable = false;
        }


        void OnSpinResult(float win)
        {
            balance += win;
            winningsText.text = win.ToString("0");
            spinBtn.interactable = true;

            betUp.interactable = true;
            BetDown.interactable = true;

            RefreshUI();
        }


        void RefreshUI()
        {
            balanceText.text = balance.ToString("0.0");
            betText.text = currentBet.ToString("0.0");

            BetDown.interactable = currentBet > MIN_BET;
            betUp.interactable = currentBet < MAX_BET;
        }


        void OpenInfo()
        {
            ScreenManager.Instance.SetScreen(Screens.InfoScreen);
        }

        public override IEnumerator EnterAsync(Screens previous)
        {
            // wait until ReelsManager exists
            while (ReelsManager.Instance == null)
                yield return null;

            ReelsManager.Instance.OnSpinStarted += OnSpinStarted;
            ReelsManager.Instance.OnSpinResult += OnSpinResult;

            RefreshUI();
        }


        public override IEnumerator ExitAsync(Screens next)
        {
            if (ReelsManager.Instance != null)
            {
                ReelsManager.Instance.OnSpinStarted -= OnSpinStarted;
                ReelsManager.Instance.OnSpinResult -= OnSpinResult;
            }

            yield return null;
        }


        public override void OnBack()
        {
            // handle back
        }
    }
}
