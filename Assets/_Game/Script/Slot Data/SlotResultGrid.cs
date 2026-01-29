public class SlotResultGrid
{
    public int Rows { get; }
    public int Columns { get; }

    private int[,] values;

    public SlotResultGrid(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        values = new int[rows, columns];
    }

    public void Set(int row, int col, int value)
    {
        values[row, col] = value;
    }

    public int Get(int row, int col)
    {
        return values[row, col];
    }
}
