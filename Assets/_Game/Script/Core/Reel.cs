using System.Collections.Generic;
using UnityEngine;

namespace SpinWheel
{
    /// <summary>
    /// Represents a single spinning reel in the slot machine.
    /// Handles:
    /// - Card pooling & positioning
    /// - Spin physics (acceleration / deceleration)
    /// - Forced result injection
    /// - Final snapping & stop notification
    /// </summary>
    public class Reel
    {
        // -----------------------------
        // DATA
        // -----------------------------

        private readonly List<Card> cards = new();
        private readonly ReelsManager.ReelConfig config;

        private float currentSpeed;
        private float remainingDistance;

        private bool isSpinning;
        private bool resultLocked;
        private bool resultInjected;

        // Forced visible result (TOP, CENTER, BOTTOM)
        private int topSymbol;
        private int centerSymbol;
        private int bottomSymbol;

        // -----------------------------
        // CONSTRUCTOR
        // -----------------------------

        public Reel(ReelsManager.ReelConfig config)
        {
            this.config = config;
        }

        // -----------------------------
        // INITIALIZATION
        // -----------------------------

        /// <summary>
        /// Creates and positions cards vertically under the given parent.
        /// </summary>
        public void Initialize(Transform parent)
        {
            int centerIndex = config.totalCards / 2;

            for (int i = 0; i < config.totalCards; i++)
            {
                Card card = ReelsManager.Instance.GetCard(parent);

                float y = (centerIndex - i) * config.symbolHeight;
                card.transform.localPosition = new Vector3(0f, y, 0f);

                AssignRandom(card);
                cards.Add(card);
            }
        }

        // -----------------------------
        // MANUAL UPDATE (called by manager)
        // -----------------------------

        /// <summary>
        /// Updates reel movement while spinning.
        /// </summary>
        public void Tick(float dt)
        {
            if (!isSpinning) return;

            UpdateSpeed(dt);
            Spin(dt);
        }

        // -----------------------------
        // PUBLIC API
        // -----------------------------

        /// <summary>
        /// Starts spinning the reel.
        /// </summary>
        public void StartSpin()
        {
            LockResult();
            CalculateStopDistance();

            resultInjected = false;
            currentSpeed = 0f;
            isSpinning = true;
        }

        /// <summary>
        /// Defines the forced result symbols for this reel.
        /// Order: TOP, CENTER, BOTTOM
        /// </summary>
        public void SetForcedResult(int top, int center, int bottom)
        {
            topSymbol = top;
            centerSymbol = center;
            bottomSymbol = bottom;
        }

        // -----------------------------
        // CORE SPIN LOGIC
        // -----------------------------

        private void UpdateSpeed(float dt)
        {
            // Acceleration phase
            if (remainingDistance > config.stopDistance)
            {
                currentSpeed = Mathf.MoveTowards(
                    currentSpeed,
                    config.maxSpinSpeed,
                    config.acceleration * dt
                );
            }
            // Deceleration phase
            else
            {
                float t = remainingDistance / config.stopDistance;
                currentSpeed = Mathf.Lerp(config.minStopSpeed, currentSpeed, t);
            }
        }

        private void Spin(float dt)
        {
            float delta = currentSpeed * dt;
            remainingDistance -= delta;

            float limit = (config.totalCards / 2) * config.symbolHeight;

            foreach (var card in cards)
            {
                Transform t = card.transform;
                t.localPosition += Vector3.down * delta;

                // Loop card back to top
                if (t.localPosition.y < -limit)
                {
                    t.localPosition += Vector3.up * config.totalCards * config.symbolHeight;

                    // Randomize only until result is injected
                    if (!resultInjected)
                        AssignRandom(card);
                }
            }

            // Inject final result near the end
            if (resultLocked && !resultInjected && remainingDistance <= config.injectDistance)
                InjectFinalCards();

            // Stop condition
            if (remainingDistance <= 0f)
                FinishStop();
        }

        // -----------------------------
        // RESULT CONTROL
        // -----------------------------

        private void InjectFinalCards()
        {
            int center = config.totalCards / 2;

            AssignForced(cards[center - 1], topSymbol);
            AssignForced(cards[center], centerSymbol);
            AssignForced(cards[center + 1], bottomSymbol);

            resultInjected = true;
        }

        private void LockResult()
        {
            resultLocked = true;
        }

        private void CalculateStopDistance()
        {
            int fullLoops = 3;
            float loopDistance = config.totalCards * config.symbolHeight;

            float centerOffset =
                Mathf.Abs(cards[config.totalCards / 2].transform.localPosition.y);

            remainingDistance = fullLoops * loopDistance + centerOffset;
        }

        // -----------------------------
        // FINISH & SNAP
        // -----------------------------

        private void FinishStop()
        {
            isSpinning = false;
            currentSpeed = 0f;
            SnapToGrid();
        }

        private void SnapToGrid()
        {
            foreach (var card in cards)
            {
                Vector3 pos = card.transform.localPosition;
                float snappedY =
                    Mathf.Round(pos.y / config.symbolHeight) * config.symbolHeight;

                card.transform.localPosition = new Vector3(0f, snappedY, 0f);
            }

            ReelsManager.Instance.NotifyReelStopped();
        }

        // -----------------------------
        // VISIBILITY HELPERS
        // -----------------------------

        /// <summary>
        /// Returns visible symbols in order: TOP, CENTER, BOTTOM
        /// </summary>
        public int[] GetVisibleSymbols()
        {
            return new int[]
            {
                topSymbol,
                centerSymbol,
                bottomSymbol
            };
        }

        /// <summary>
        /// Returns visible Card objects in order: TOP, CENTER, BOTTOM
        /// </summary>
        public Card[] GetVisibleCards()
        {
            int centerIndex = config.totalCards / 2;

            return new Card[]
            {
                cards[centerIndex - 1],
                cards[centerIndex],
                cards[centerIndex + 1]
            };
        }

        // -----------------------------
        // CARD ASSIGNMENT
        // -----------------------------

        private void AssignRandom(Card card)
        {
            CardConfig cfg =
                ReelsManager.Instance.cardDatabase.GetRandom().config;

            card.ApplyConfig(cfg);
        }

        private void AssignForced(Card card, int cardId)
        {
            CardConfig cfg =
                ReelsManager.Instance.cardDatabase.GetById(cardId).config;

            card.ApplyConfig(cfg);
        }

        // -----------------------------
        // CLEANUP
        // -----------------------------

        /// <summary>
        /// Returns all cards to pool and clears reel data.
        /// </summary>
        public void Dispose()
        {
            foreach (var card in cards)
                ReelsManager.Instance.ReturnCard(card);

            cards.Clear();
        }
    }
}
