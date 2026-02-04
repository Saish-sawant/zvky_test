using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace SpinWheel
{
    /// <summary>
    /// Central controller for the entire game flow.
    /// Responsible for:
    /// - Initializing the State Machine
    /// - Managing high-level game states
    /// - Handling global events like Spin / Play Again
    /// - Persisting across scenes
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance for global access
        /// </summary>
        public static GameManager Instance;

        /// <summary>
        /// Finite State Machine controlling game flow
        /// </summary>
        public StateMachine FSM { get; private set; }

        // -------------------- States --------------------

        public IState BootState { get; private set; }
        public IState PreGameState { get; private set; }
        public IState InGameState { get; private set; }
        public IState PostGameState { get; private set; }

        /// <summary>
        /// Used for loading UI (splash / progress bar)
        /// </summary>
        public float LoadProgress { get; private set; }

        // -------------------- Unity Lifecycle --------------------

        private void Awake()
        {
            // Enforce Singleton pattern
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize FSM
            FSM = new StateMachine();

            // Create all game states once
            BootState     = new BootState(this, this);
            PreGameState  = new PreGameState(this);
            InGameState   = new InGameState(this);
            PostGameState = new PostGameState(this);
        }

        private void Start()
        {
            // Entry point of the game flow
            FSM.ChangeState(BootState);

            // Start background music
            AudioManager.Instance.Play("BGM");
        }

        // -------------------- Shared Helpers --------------------

        /// <summary>
        /// Handles global asset loading / SDK initialization.
        /// Can later be replaced with Addressables or async loading.
        /// </summary>
        public IEnumerator LoadAssets()
        {
            // Placeholder for future loading logic
            yield return new WaitForSeconds(2f);
        }

        // -------------------- UI / Gameplay Hooks --------------------

        /// <summary>
        /// Called when Spin button is pressed
        /// </summary>
        public void OnSpinPressed()
        {
            FSM.ChangeState(InGameState);
        }

        /// <summary>
        /// Called when spin + win animations are completed
        /// </summary>
        public void OnSpinEnded()
        {
            FSM.ChangeState(PostGameState);
        }

        /// <summary>
        /// Called when player chooses to play again
        /// </summary>
        public void OnPlayAgain()
        {
            FSM.ChangeState(PreGameState);
        }

        /// <summary>
        /// Updates loading progress (0–1 range recommended)
        /// </summary>
        public void UpdateLoadProgress(float value)
        {
            LoadProgress = value;
        }
    }
}
