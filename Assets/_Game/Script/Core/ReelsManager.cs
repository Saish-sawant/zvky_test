using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpinWheel
{
    /// <summary>
    /// Central controller for all reels.
    /// Responsibilities:
    /// - Create and update reels
    /// - Control spin flow & locking
    /// - Inject spin results
    /// - Build symbol grid
    /// - Evaluate paylines & trigger win animations
    /// </summary>
    public class ReelsManager : MonoBehaviour
    {
        public static ReelsManager Instance { get; private set; }

        // -----------------------------
        // DATA & CONFIG
        // -----------------------------

        [SerializeField] private PatternDatabaseSO patternDatabase;
        [SerializeField] private PayTableDatabaseSO payTable;
        [SerializeField] private float currentBet = 1f;

        private PatternManager patternManager;

        [Serializable]
        public struct ReelConfig
        {
            public int totalCards;
            public float symbolHeight;

            public float maxSpinSpeed;
            public float acceleration;

            public float minStopSpeed;
            public float stopDistance;
            public float injectDistance;
        }

        [Header("Reel Settings")]
        public ReelConfig reelConfig;
        [SerializeField] private float spinStartDelay = 0.5f;

        [Header("Reel Roots")]
        public Transform reel1;
        public Transform reel2;
        public Transform reel3;
        public Transform reel4;
        public Transform reel5;

        [Header("Pooling")]
        public Card cardPrefab;
        public Transform cardHolderParent;
        public int initialPoolSize = 30;
        public CardCollectionSO cardDatabase;

        // -----------------------------
        // RUNTIME DATA
        // -----------------------------

        private ObjectPoolManager<Card> cardPool;
        private readonly List<Reel> reels = new();

        private int stoppedReels = 0;
        private bool canSpin = true;

        // -----------------------------
        // EVENTS
        // -----------------------------

        public event Action<float> OnSpinStarted; // bet
        public event Action<float> OnSpinResult;  // total win
        public Action OnAllReelsStopped;

        [SerializeField] private PaylineController paylineController;

        // -----------------------------
        // UNITY LIFECYCLE
        // -----------------------------

        private void Awake()
        {
            Instance = this;
            InitializePool();
        }

        private void Start()
        {
            // Create all reels
            reels.Add(CreateReel(reel1));
            reels.Add(CreateReel(reel2));
            reels.Add(CreateReel(reel3));
            reels.Add(CreateReel(reel4));
            reels.Add(CreateReel(reel5));

            // Pattern evaluation system
            patternManager = new PatternManager(patternDatabase, payTable);

            // Hook events
            OnAllReelsStopped += EvaluatePatterns;
            paylineController.OnPaylinesFinished += OnWinAnimationsFinished;
        }
        /// <summary>
        /// Creates a new reel instance and initializes it.
        /// </summary>
        private Reel CreateReel(Transform parent)
        {
            Reel reel = new Reel(reelConfig);
            reel.Initialize(parent);
            return reel;
        }
        private void Update()
        {
            float dt = Time.deltaTime;

            // Manual update for all reels
            foreach (var reel in reels)
                reel.Tick(dt);
        }

        private void OnDestroy()
        {
            foreach (var reel in reels)
                reel.Dispose();
        }

        // -----------------------------
        // SPIN FLOW
        // -----------------------------

        public void SetBet(float bet)
        {
            currentBet = bet;
        }

        public void SpinAll()
        {
            if (!canSpin)
                return;

            canSpin = false; // 🔒 lock input

            paylineController.ResetPaylines();
            OnSpinStarted?.Invoke(currentBet);

            AssignRandomResults();
            StartCoroutine(SpinRoutine());

            AudioManager.Instance.Play("SpinReel");
        }

        private IEnumerator SpinRoutine()
        {
            foreach (var reel in reels)
            {
                reel.StartSpin();
                yield return new WaitForSeconds(spinStartDelay);
            }
        }

        private void OnWinAnimationsFinished()
        {
            EnableSpin();
        }

        private void EnableSpin()
        {
            canSpin = true;
        }

        // -----------------------------
        // REEL CALLBACKS
        // -----------------------------

        public void NotifyReelStopped()
        {
            stoppedReels++;

            if (stoppedReels >= reels.Count)
            {
                stoppedReels = 0;

                AudioManager.Instance.Stop("SpinReel");
                AudioManager.Instance.Play("StopReel");

                OnAllReelsStopped?.Invoke();
            }
        }

        // -----------------------------
        // RESULT & GRID BUILDING
        // -----------------------------

        private void AssignRandomResults()
        {
            // TEMP / DEBUG: forced win on top row
            reels[0].SetForcedResult(1, 2, 3);
            reels[1].SetForcedResult(1, 9, 9);
            reels[2].SetForcedResult(1, 2, 2);
            reels[3].SetForcedResult(1, 9, 9);
            reels[4].SetForcedResult(1, 9, 2);
        }

        public int[,] BuildGrid()
        {
            int[,] grid = new int[3, 5];

            for (int col = 0; col < reels.Count; col++)
            {
                int[] symbols = reels[col].GetVisibleSymbols();

                grid[0, col] = symbols[0]; // top
                grid[1, col] = symbols[1]; // center
                grid[2, col] = symbols[2]; // bottom
            }

            return grid;
        }

        public Card[,] BuildCardGrid()
        {
            Card[,] grid = new Card[3, 5];

            for (int col = 0; col < reels.Count; col++)
            {
                Card[] visible = reels[col].GetVisibleCards();

                grid[0, col] = visible[0];
                grid[1, col] = visible[1];
                grid[2, col] = visible[2];
            }

            return grid;
        }

        // -----------------------------
        // WIN EVALUATION
        // -----------------------------

        private void EvaluatePatterns()
        {
            int[,] grid = BuildGrid();

            float totalWin;
            List<PatternMatchResult> wins =
                patternManager.Evaluate(grid, currentBet, out totalWin);

            Debug.Log("TOTAL WIN: " + totalWin);

            OnSpinResult?.Invoke(totalWin);

            if (wins.Count > 0)
                paylineController.PlayWinningPaylines(wins);
        }

        // -----------------------------
        // POOLING
        // -----------------------------

        private void InitializePool()
        {
            cardPool = new ObjectPoolManager<Card>(
                cardPrefab,
                cardHolderParent,
                initialPoolSize
            );
        }

        public Card GetCard(Transform parent) => cardPool.Get(parent);
        public void ReturnCard(Card card) => cardPool.Return(card);
    }
}
