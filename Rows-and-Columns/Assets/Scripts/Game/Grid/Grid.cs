using NUnit.Framework;
using System.Collections.Generic;
using System;
using UnityEngine;
using JetBrains.Annotations;

public class Grid : MonoBehaviour
{
    // Configuration variables
    public ShapeStorage shapeStorage; // Reference to shape storage component
    public GameObject GridSquare; // Prefab for grid squares
    public Vector2 StartPosition = new Vector2(0.0f, 0.0f); // Starting position for grid
    public float SquareScale = 0.5f; // Scale factor for each grid square
    public float EverySquareOffset = 0.0f; // Offset between grid squares

    // Private state variables
    private Vector2 _offset = new Vector2(0.0f, 0.0f); // Calculated offset between squares
    private List<GameObject> _gridSquares = new List<GameObject>(); // Stores all grid squares
    private LineIndicator _lineIndicator; // Reference to line indicator component
    private int Columns = 8; // Number of columns in grid
    private int Rows = 8; // Number of rows in grid
    private int combo = 1; // Current combo multiplier
    private int shapesPlacedSinceLastCompleted = 0; // Tracks shapes placed since last line clear

    // Subscribe to events when enabled
    private void OnEnable()
    {
        GameEvents.CheckIfShapeCanBePlaced += CheckIfShapeCanBePlaced;
    }

    // Unsubscribe from events when disabled
    private void OnDisable()
    {
        GameEvents.CheckIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;
    }

    // Initialization
    void Start()
    {
        _lineIndicator = GetComponent<LineIndicator>();
        CreateGrid();
    }

    // Creates the game grid
    private void CreateGrid()
    {
        SpawnGridSquares(); // Instantiate all grid squares
        SetGridSquarePosition(); // Position all grid squares
    }

    // Instantiates grid squares in a 8x8 pattern
    private void SpawnGridSquares()
    {
        int SquareIndex = 0; // Tracks unique index for each square

        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Columns; col++)
            {
                // Create new square and add to list
                _gridSquares.Add(Instantiate(GridSquare) as GameObject);

                // Configure square properties
                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SquareIndex = SquareIndex;
                _gridSquares[_gridSquares.Count - 1].transform.SetParent(this.transform);
                _gridSquares[_gridSquares.Count - 1].transform.localScale = new Vector3(SquareScale, SquareScale, SquareScale);
                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().grid = this;

                SquareIndex++;
            }
        }
    }

    // Positions all grid squares with proper spacing
    private void SetGridSquarePosition()
    {
        int ColNum = 0; // Current column counter
        int RowNum = 0; // Current row counter

        // Get dimensions from first square
        var square_rect = _gridSquares[0].GetComponent<RectTransform>();
        _offset.x = square_rect.rect.width * square_rect.transform.localScale.x + EverySquareOffset;
        _offset.y = square_rect.rect.height * square_rect.transform.localScale.y + EverySquareOffset;

        // Position each square
        foreach (GameObject square in _gridSquares)
        {
            if (ColNum + 1 > Columns) // Move to next row
            {
                ColNum = 0;
                RowNum++;
            }

            // Calculate position offsets
            var pos_x_offset = _offset.x * ColNum;
            var pos_y_offset = _offset.y * RowNum;

            // Set square positions
            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(StartPosition.x + pos_x_offset, StartPosition.y - pos_y_offset);
            square.GetComponent<RectTransform>().localPosition = new Vector3(StartPosition.x + pos_x_offset, StartPosition.y - pos_y_offset, 0.0f);
            ColNum++;
        }
    }

    // Checks if current shape can be placed on selected squares
    private void CheckIfShapeCanBePlaced()
    {
        var squareIndexes = new List<int>();

        // Collect all selected, unoccupied squares
        foreach (var square in _gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();
            if (gridSquare.Selected && !gridSquare.SquareOccupied)
            {
                squareIndexes.Add(gridSquare.SquareIndex);
                gridSquare.Selected = false;
            }
        }

        var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();
        if (currentSelectedShape == null) return;

        // If shape fits selected squares
        if (currentSelectedShape.TotalSquareNumber == squareIndexes.Count)
        {
            // Place shape on all selected squares
            foreach (var squareIndex in squareIndexes)
            {
                _gridSquares[squareIndex].GetComponent<GridSquare>().PlaceShapeOnBoard();
            }

            // Check if any shapes remain to place
            var shapesLeft = 0;
            foreach (var shape in shapeStorage.shapeList)
            {
                if (shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
                {
                    shapesLeft++;
                }
            }

            // Handle shape placement completion
            if (shapesLeft == 0)
            {
                GameEvents.RequestNewShapes(); // Request new shapes if none left
            }
            else
            {
                GameEvents.SetShapeInactive(); // Deactivate current shape
            }

            shapesPlacedSinceLastCompleted++;
            CheckIfAnyLineIsCompleted(); // Check for completed lines
        }
        else
        {
            GameEvents.MoveShapeToStartPosition(); // Return shape if placement invalid
        }
    }

    // Checks if current shape can be placed on selected squares (similar to above but returns bool)
    public bool CanShapeBePlaced()
    {
        var squareIndexes = new List<int>();
        foreach (var square in _gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();
            if (gridSquare.Selected && !gridSquare.SquareOccupied)
            {
                squareIndexes.Add(gridSquare.SquareIndex);
            }
        }

        var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();
        if (currentSelectedShape == null) return false;

        return currentSelectedShape.TotalSquareNumber == squareIndexes.Count;
    }

    // Checks for completed lines (rows or columns)
    private void CheckIfAnyLineIsCompleted()
    {
        List<int[]> lines = new List<int[]>();

        // Add all vertical lines to check
        foreach (var column in _lineIndicator.columnIndexes)
        {
            lines.Add(_lineIndicator.GetVerticalLine(column));
        }

        // Add all horizontal lines to check
        for (var row = 0; row < 8; row++)
        {
            List<int> data = new List<int>(8);
            for (var index = 0; index < 8; index++)
            {
                data.Add(_lineIndicator.line_data[row, index]);
            }
            lines.Add(data.ToArray());
        }

        // Check for completed lines and handle scoring
        var completedLines = CheckIfSquaresAreCompleted(lines);
        if (completedLines > 0)
        {
            // Calculate combo multiplier
            if (shapesPlacedSinceLastCompleted < 4)
                combo += 1 * (completedLines * completedLines);
            else
                combo = 1;

            shapesPlacedSinceLastCompleted = 0;
            var totalScore = (10 * completedLines * combo);
            GameEvents.AddScore(totalScore); // Trigger score event
        }

        CheckIfPlayerLost(); // Check if player has lost
    }

    // Checks if any lines are fully occupied
    private int CheckIfSquaresAreCompleted(List<int[]> data)
    {
        List<int[]> completedLines = new List<int[]>();
        var linesCompleted = 0;

        // Find all completed lines
        foreach (var line in data)
        {
            var completed = true;
            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                if (comp.SquareOccupied == false)
                {
                    completed = false;
                }
            }

            if (completed)
            {
                completedLines.Add(line);
            }
        }

        // Clear completed lines
        foreach (var line in completedLines)
        {
            var completed = false;
            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.DeactivateSquare();
                completed = true;
            }

            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.ClearOccupied();
            }

            if (completed)
            {
                linesCompleted++;
            }
        }

        return linesCompleted;
    }

    // Checks if player has lost (no valid moves)
    private void CheckIfPlayerLost()
    {
        var validShapes = 0;
        var activeShapes = 0;

        // Check all shapes for possible placements
        for (var index = 0; index < shapeStorage.shapeList.Count; index++)
        {
            var isShapeActive = shapeStorage.shapeList[index].IsAnyOfShapeSquareActive();
            if (isShapeActive)
            {
                if (CheckIfShapeCanBePlacedOnGrid(shapeStorage.shapeList[index]))
                {
                    shapeStorage.shapeList[index]?.ActivateShape();
                    validShapes++;
                }
                activeShapes++;
            }
        }

        // Handle game over conditions
        if (validShapes < 1 && activeShapes > 2)
        {
            GameEvents.RequestNewShapes(); // Request new shapes if stuck
        }
        else if (validShapes < 1)
        {
            GameEvents.GameOver(false); // Game over if no valid moves
        }
    }

    // Checks if a specific shape can be placed anywhere on grid
    private bool CheckIfShapeCanBePlacedOnGrid(Shape currentShape)
    {
        var currentShapeData = currentShape.CurrentShapeData;
        var shapeColumns = currentShapeData.columns;
        var shapeRows = currentShapeData.rows;

        // Get all filled squares in the shape
        List<int> originalShapeFilledUpSquares = new List<int>();
        var squareIndex = 0;

        for (var rowIndex = 0; rowIndex < shapeRows; rowIndex++)
        {
            for (var columnIndex = 0; columnIndex < shapeColumns; columnIndex++)
            {
                if (currentShapeData.board[rowIndex].column[columnIndex])
                {
                    originalShapeFilledUpSquares.Add(squareIndex);
                }
                squareIndex++;
            }
        }

        // Validate shape data
        if (currentShape.TotalSquareNumber != originalShapeFilledUpSquares.Count)
        {
            Debug.LogError("shapes don't match");
        }

        // Check all possible positions
        var squareList = GetAllSquaresCombination(shapeColumns, shapeRows);
        bool canBePlaced = false;

        foreach (var number in squareList)
        {
            bool shapeCanBePlacedOnTheBoard = true;
            foreach (var squareIndexToCheck in originalShapeFilledUpSquares)
            {
                var comp = _gridSquares[number[squareIndexToCheck]].GetComponent<GridSquare>();
                if (comp.SquareOccupied)
                {
                    shapeCanBePlacedOnTheBoard = false;
                }
            }

            if (shapeCanBePlacedOnTheBoard)
            {
                canBePlaced = true;
            }
        }

        return canBePlaced;
    }

    // Generates all possible positions a shape could be placed
    private List<int[]> GetAllSquaresCombination(int columns, int rows)
    {
        var squareList = new List<int[]>();
        var lastColumnIndex = 0;
        var lastRowIndex = 0;
        int safeIndex = 0; // Prevents infinite loops

        // Generate all possible positions
        while (lastRowIndex + (rows - 1) < 8)
        {
            var rowData = new List<int>();

            // Create position data for current location
            for (var row = lastRowIndex; row < lastRowIndex + rows; row++)
            {
                for (var column = lastColumnIndex; column < lastColumnIndex + columns; column++)
                {
                    rowData.Add(_lineIndicator.line_data[row, column]);
                }
            }

            squareList.Add(rowData.ToArray());
            lastColumnIndex++;

            // Move to next row if at end of columns
            if (lastColumnIndex + (columns - 1) >= 8)
            {
                lastRowIndex++;
                lastColumnIndex = 0;
            }

            safeIndex++;
            if (safeIndex > 100) // Safety check
            {
                break;
            }
        }

        return squareList;
    }
}