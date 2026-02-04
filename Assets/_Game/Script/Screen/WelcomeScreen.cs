using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace SpinWheel
{
    public class WelcomeScreen : Screens
    {

        [SerializeField] Button continueBtn;

        void Start()
        {
            continueBtn.onClick.AddListener(() =>
            {
                ScreenManager.Instance.SetScreen(Screens.GamePlay);
            });
        }
        public override IEnumerator EnterAsync(Screens previous)
        {
            yield return null;
            // throw new System.NotImplementedException();
        }

        public override IEnumerator ExitAsync(Screens next)
        {

            yield return null;
            //  throw new System.NotImplementedException();
        }

        public override void OnBack()
        {
            throw new System.NotImplementedException();
        }


    }
}