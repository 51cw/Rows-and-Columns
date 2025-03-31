using UnityEngine;

// Creates a menu entry in Unity's Assets > Create menu
[CreateAssetMenu]
[System.Serializable]
public class ShapeData : ScriptableObject
{
    // Nested class representing a single row in the shape grid
    [System.Serializable]
    public class Row
    {
        public bool[] column;  // Array representing active/inactive cells in this row
        private int _size = 0; // Track the row size

        // Default constructor
        public Row() { }

        // Constructor with size initialization
        public Row(int size)
        {
            CreateRow(size);
        }

        // Initializes a new row of given size
        public void CreateRow(int size)
        {
            _size = size;
            column = new bool[_size];  // Create new boolean array
            ClearRow();                 // Initialize all cells to false
        }

        // Resets all cells in this row to false (inactive)
        public void ClearRow()
        {
            for (int i = 0; i < _size; i++)
            {
                column[i] = false;
            }
        }
    }

    // Shape configuration parameters
    public int columns = 0;  // Number of columns in the shape
    public int rows = 0;     // Number of rows in the shape
    public Row[] board;      // 2D array representing the shape's pattern

    // Clears all cells in the current board
    public void Clear()
    {
        for (var i = 0; i < rows; i++)
        {
            board[i].ClearRow();  // Clear each row
        }
    }

    // Creates a new empty board with current rows/columns dimensions
    public void CreateNewBoard()
    {
        board = new Row[rows];  // Initialize rows array

        // Initialize each row with proper column count
        for (var i = 0; i < rows; i++)
        {
            board[i] = new Row(columns);
        }
    }
}