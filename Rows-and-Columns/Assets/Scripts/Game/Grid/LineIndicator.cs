using UnityEngine;

public class LineIndicator : MonoBehaviour
{
    // 8x8 grid mapping each position to a unique index (0-63)
    // Represents the game board's coordinate system
    public int[,] line_data = new int[8, 8]
    {
        {  0,  1,  2,  3,  4,  5,  6,  7 },
        {  8,  9, 10, 11, 12, 13, 14, 15 },
        { 16, 17, 18, 19, 20, 21, 22, 23 },
        { 24, 25, 26, 27, 28, 29, 30, 31 },
        { 32, 33, 34, 35, 36, 37, 38, 39 },
        { 40, 41, 42, 43, 44, 45, 46, 47 },
        { 48, 49, 50, 51, 52, 53, 54, 55 },
        { 56, 57, 58, 59, 60, 61, 62, 63 }
    };

    // Column indexes for vertical line checking
    [HideInInspector]
    public int[] columnIndexes = new int[8]
    {
        0, 1, 2, 3, 4, 5, 6, 7
    };

    // Converts a square index (0-63) to its (row, column) position
    private (int, int) GetSquarePosition(int square_index)
    {
        int pos_row = -1;  // Initialize with invalid row
        int pos_col = -1;  // Initialize with invalid column

        // Search through the entire grid
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                if (line_data[row, col] == square_index)
                {
                    pos_row = row;  // Found matching row
                    pos_col = col;  // Found matching column
                }
            }
        }

        return (pos_row, pos_col);  // Returns tuple with (row, column)
    }

    // Gets all squares in the vertical line containing the given square index
    public int[] GetVerticalLine(int square_index)
    {
        int[] line = new int[8];  // Will store the vertical line's indices

        // Get the column position of the input square
        var square_position_col = GetSquarePosition(square_index).Item2;

        // Extract all squares in this column
        for (int index = 0; index < 8; index++)
        {
            line[index] = line_data[index, square_position_col];
        }

        return line;  // Returns array of 8 indices (the vertical line)
    }
}