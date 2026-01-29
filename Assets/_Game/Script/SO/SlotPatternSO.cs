using UnityEngine;

[CreateAssetMenu(menuName = "Slot/Pattern")]
public class SlotPatternSO : ScriptableObject
{
    [Header("Pattern Info")]
    public string patternId = "1";
    public int reward;
    public string description;

    [Header("Grid Size")]
    public int rows = 3;
    public int columns = 5;

    // Serialized as 1D but drawn as grid
    public bool[] cells;

    private void OnValidate()
    {
        int requiredSize = rows * columns;
        if (cells == null || cells.Length != requiredSize)
        {
            cells = new bool[requiredSize];
        }
    }

    public bool GetCell(int row, int col)
    {
        int index = row * columns + col;
        return cells[index];
    }
}
