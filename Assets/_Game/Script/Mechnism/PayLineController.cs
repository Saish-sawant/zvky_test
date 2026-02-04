using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpinWheel
{
    /// <summary>
    /// Responsible for visualizing winning paylines.
    /// Plays payline drawing and card win animations sequentially.
    /// </summary>
    public class PaylineController : MonoBehaviour
    {
        [SerializeField] private PaylineRenderer paylineRenderer;
        [SerializeField] private ReelsManager reelsManager;
        [SerializeField] private float lineShowDuration = 0.5f;

        private Coroutine playRoutine;

        /// <summary>
        /// Fired when all winning paylines have finished playing.
        /// Used to unlock spin / continue flow.
        /// </summary>
        public event Action OnPaylinesFinished;

        // -----------------------------
        // PUBLIC API
        // -----------------------------

        /// <summary>
        /// Starts playing all winning paylines.
        /// If another sequence is already playing, it is stopped.
        /// </summary>
        public void PlayWinningPaylines(List<PatternMatchResult> results)
        {
            if (playRoutine != null)
                StopCoroutine(playRoutine);

            playRoutine = StartCoroutine(PlayRoutine(results));
        }

        /// <summary>
        /// Clears any currently drawn payline.
        /// </summary>
        public void ResetPaylines()
        {
            paylineRenderer.ClearLine();
        }

        // -----------------------------
        // CORE ROUTINES
        // -----------------------------

        private IEnumerator PlayRoutine(List<PatternMatchResult> results)
        {
            // Snapshot of current visible cards
            Card[,] grid = reelsManager.BuildCardGrid();

            foreach (var result in results)
            {
                yield return DrawPattern(result.pattern, grid);
            }

            ResetPaylines();
            OnPaylinesFinished?.Invoke();
        }

        /// <summary>
        /// Draws a single payline pattern and plays win animation on cards.
        /// </summary>
        private IEnumerator DrawPattern(SlotPatternSO pattern, Card[,] grid)
        {
            List<Vector3> points = new();

            for (int r = 0; r < pattern.rows; r++)
            {
                for (int c = 0; c < pattern.columns; c++)
                {
                    if (!pattern.GetCell(r, c))
                        continue;

                    Card card = grid[r, c];

                    // Add world position for payline
                    points.Add(card.transform.position);

                    // Trigger card win animation
                    card.PlayWin();

                    // Draw progressively
                    paylineRenderer.DrawLine(points);

                    yield return new WaitForSeconds(lineShowDuration);
                }
            }
        }
    }
}
