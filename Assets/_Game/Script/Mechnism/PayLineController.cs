using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaylineController : MonoBehaviour
{
    [SerializeField] PaylineRenderer paylineRenderer;
    [SerializeField] float lineShowDuration = 0.5f;
    [SerializeField] ReelsManager reelsManager;

    Coroutine playRoutine;

    public void PlayWinningPaylines(List<PatternMatchResult> results)
    {
        if (playRoutine != null)
            StopCoroutine(playRoutine);

        playRoutine = StartCoroutine(PlayRoutine(results));
    }

    IEnumerator PlayRoutine(List<PatternMatchResult> results)
    {
        Card[,] grid = reelsManager.BuildCardGrid();

        foreach (var result in results)
        {
            yield return DrawPattern(result.pattern, grid);
        }
    }

    IEnumerator DrawPattern(SlotPatternSO pattern, Card[,] grid)
    {
        List<Vector3> points = new();

        for (int r = 0; r < pattern.rows; r++)
        {
            for (int c = 0; c < pattern.columns; c++)
            {
                if (!pattern.GetCell(r, c))
                    continue;

                Card card = grid[r, c];

                // position for payline
                points.Add(card.transform.position);

                // OPTIONAL: visual feedback
                card.PlayWin();

                paylineRenderer.DrawLine(points);
                yield return new WaitForSeconds(lineShowDuration);
            }
        }

        // Optional cleanup
        // foreach (var card in highlightedCards)
        //     card.StopWin();
    }

}
