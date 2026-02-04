using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Evaluates slot patterns against a symbol grid
/// and calculates wins using a paytable.
/// Pure logic class (no MonoBehaviour).
/// </summary>
public class PatternManager
{
    private readonly PatternDatabaseSO patternDatabase;
    private readonly PayTableDatabaseSO payTable;

    // Special symbol ID used as wildcard
    private const int WILD_ID = 0;

    public PatternManager(
        PatternDatabaseSO patternDatabase,
        PayTableDatabaseSO payTable)
    {
        this.patternDatabase = patternDatabase;
        this.payTable = payTable;
    }

    /// <summary>
    /// Evaluates all patterns against the grid and returns win results.
    /// </summary>
    /// <param name="grid">Final visible symbol grid [rows, columns]</param>
    /// <param name="bet">Current bet amount</param>
    /// <param name="totalWin">Total win amount (output)</param>
    public List<PatternMatchResult> Evaluate(
        int[,] grid,
        float bet,
        out float totalWin)
    {
        totalWin = 0f;
        List<PatternMatchResult> results = new();

        foreach (var pattern in patternDatabase.patterns)
        {
            if (!MatchesPattern(grid, pattern, out int symbolId, out int count))
                continue;

            // Minimum match requirement
            if (count < 3)
                continue;

            float multiplier = payTable.GetMultiplier(symbolId, count);
            float win = bet * multiplier;

            if (win <= 0)
                continue;

            totalWin += win;

            results.Add(new PatternMatchResult
            {
                pattern = pattern,
                symbolId = symbolId,
                count = count
            });
        }

        return results;
    }

    /// <summary>
    /// Checks whether a grid matches a given pattern.
    /// Supports wild symbols.
    /// </summary>
    private bool MatchesPattern(
        int[,] grid,
        SlotPatternSO pattern,
        out int symbolId,
        out int count)
    {
        symbolId = -1;
        count = 0;

        for (int r = 0; r < pattern.rows; r++)
        {
            for (int c = 0; c < pattern.columns; c++)
            {
                if (!pattern.GetCell(r, c))
                    continue;

                int symbol = grid[r, c];

                // Lock first non-wild symbol as reference
                if (symbolId == -1 && symbol != WILD_ID)
                    symbolId = symbol;

                // Match if same symbol or wild
                if (symbol == symbolId || symbol == WILD_ID)
                    count++;
                else
                    return false;
            }
        }

        return count >= 3;
    }
}

/// <summary>
/// Result data for a matched slot pattern.
/// </summary>
public class PatternMatchResult
{
    public SlotPatternSO pattern;
    public int symbolId;
    public int count;
}
