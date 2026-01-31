using System.Collections.Generic;
using UnityEngine;

public class PatternManager
{
    private PatternDatabaseSO patternDatabase;
    private PayTableDatabaseSO payTable;

    private const int WILD_ID = 0;

    public PatternManager(
        PatternDatabaseSO patternDatabase,
        PayTableDatabaseSO payTable)
    {
        this.patternDatabase = patternDatabase;
        this.payTable = payTable;
    }

   public List<PatternMatchResult> Evaluate(
    int[,] grid,
    float bet,
    out float totalWin)
{
    totalWin = 0f;
    List<PatternMatchResult> results = new();

    foreach (var pattern in patternDatabase.patterns)
    {
        int symbolId;
        int count;

        if (!MatchesPattern(grid, pattern, out symbolId, out count))
            continue;

        if (count < 3)
            continue;

        float multiplier = payTable.GetMultiplier(symbolId, count);
        float win = bet * multiplier;

        if (win > 0)
        {
            totalWin += win;

            results.Add(new PatternMatchResult
            {
                pattern = pattern,
                symbolId = symbolId,
                count = count
            });
        }
    }

    return results;
}


    bool MatchesPattern(
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

                if (symbolId == -1 && symbol != WILD_ID)
                    symbolId = symbol;

                if (symbol == symbolId || symbol == WILD_ID)
                    count++;
                else
                    return false;
            }
        }

        return count >= 3;
    }
}
public class PatternMatchResult
{
    public SlotPatternSO pattern;
    public int symbolId;
    public int count;
}
