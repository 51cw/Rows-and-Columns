using NUnit.Framework;
using System.Collections.Generic;
using System;
using UnityEngine;
using JetBrains.Annotations;

public class Grid : MonoBehaviour
{
    public ShapeStorage shapeStorage;
    public GameObject GridSquare;
    public Vector2 StartPosition = new Vector2(0.0f, 0.0f);
    public float SquareScale = 0.5f;
    public float EverySquareOffset = 0.0f;

    private Vector2 _offset = new Vector2(0.0f, 0.0f);
    private List<GameObject> _gridSquares = new List<GameObject>();
    private LineIndicator _lineIndicator;
    private int Columns = 8;
    private int Rows = 8;
    private int combo = 1;
    private int shapesPlacedSinceLastCompleted = 0;

    private void OnEnable()
    {
        GameEvents.CheckIfShapeCanBePlaced += CheckIfShapeCanBePlaced;
    }

    private void OnDisable()
    {
        GameEvents.CheckIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;
    }



    void Start()
    {
        _lineIndicator = GetComponent<LineIndicator>();
        CreateGrid();
    }


    private void CreateGrid()
    {
        SpawnGridSquares();
        SetGridSquarePosition();
    }

    private void SpawnGridSquares()
    {
        int SquareIndex = 0;

        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Columns; col++)
            {
                _gridSquares.Add(Instantiate(GridSquare) as GameObject);
                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SquareIndex = SquareIndex;
                _gridSquares[_gridSquares.Count - 1].transform.SetParent(this.transform);
                _gridSquares[_gridSquares.Count - 1].transform.localScale = new Vector3(SquareScale, SquareScale, SquareScale);
                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().grid = this;
                SquareIndex++;
            }
        }
    }

    private void SetGridSquarePosition()
    {
        int ColNum = 0;
        int RowNum = 0;

        var square_rect = _gridSquares[0].GetComponent<RectTransform>();

        _offset.x = square_rect.rect.width * square_rect.transform.localScale.x + EverySquareOffset;
        _offset.y = square_rect.rect.height * square_rect.transform.localScale.y + EverySquareOffset;

        foreach (GameObject square in _gridSquares)
        {
            if (ColNum + 1 > Columns)
            {
                ColNum = 0;
                RowNum++;
            }

            var pos_x_offset = _offset.x * ColNum;
            var pos_y_offset = _offset.y * RowNum;

            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(StartPosition.x + pos_x_offset, StartPosition.y - pos_y_offset);
            square.GetComponent<RectTransform>().localPosition = new Vector3(StartPosition.x + pos_x_offset, StartPosition.y - pos_y_offset, 0.0f);
            ColNum++;
        }

    }
    private void CheckIfShapeCanBePlaced()
    {
        var squareIndexes = new List<int>();
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

        if (currentSelectedShape.TotalSquareNumber == squareIndexes.Count)
        {
            foreach (var squareIndex in squareIndexes)
            {
                _gridSquares[squareIndex].GetComponent<GridSquare>().PlaceShapeOnBoard();
            }

            var shapesLeft = 0;

            foreach (var shape in shapeStorage.shapeList)
            {
                if(shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
                {
                    shapesLeft++;
                }
            }

            if (shapesLeft == 0) 
            {
                GameEvents.RequestNewShapes();
            }
            else
            {
                GameEvents.SetShapeInactive();
            }
            shapesPlacedSinceLastCompleted++;
            CheckIfAnyLineIsCompleted();
        }
        else
        {
            GameEvents.MoveShapeToStartPosition();
        }
       
    }
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

    private void CheckIfAnyLineIsCompleted()
    {
        List < int[]> lines = new List <int[]>();

        foreach (var column in _lineIndicator.columnIndexes) 
        { 
            lines.Add(_lineIndicator.GetVerticalLine(column));
        }

        for(var row = 0; row < 8; row++)
        {
            List<int> data = new List<int>(8);
            for(var index = 0; index < 8; index++)
            {
                data.Add(_lineIndicator.line_data[row,index]);
            }

            lines.Add(data.ToArray());
        }

        var completedLines = CheckIfSquaresAreCompleted(lines);

        if (completedLines > 0)
        {
            if (shapesPlacedSinceLastCompleted < 4)
                combo += 1 * (completedLines * completedLines);
            else combo = 1;
            shapesPlacedSinceLastCompleted = 0;
            var totalScore = (10 * completedLines * combo);
            GameEvents.AddScore(totalScore);
        }
        
        CheckIfPlayerLost();
    }

    private int CheckIfSquaresAreCompleted(List<int[]> data)
    {
        List <int[]> completedLines = new List<int[]>();
        var linesCompleted = 0;
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

    private void CheckIfPlayerLost()
    {
        var validShapes = 0;
        var activeShapes = 0;
        for (var index = 0; index  < shapeStorage.shapeList.Count; index++)
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
        
        if (validShapes < 1 && activeShapes > 2) 
        {
            GameEvents.RequestNewShapes();
            
        }
        else if (validShapes < 1) 
        {
            GameEvents.GameOver(false);
            
        }
    }

    private bool CheckIfShapeCanBePlacedOnGrid(Shape currentShape) 
    {
        var currentShapeData = currentShape.CurrentShapeData;
        var shapeColumns = currentShapeData.columns;
        var shapeRows = currentShapeData.rows;

        List<int> originalShapeFilledUpSquares = new List<int>();
        var squareIndex = 0;

        for(var rowIndex = 0; rowIndex < shapeRows; rowIndex++)
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

        if (currentShape.TotalSquareNumber != originalShapeFilledUpSquares.Count)
        {
            Debug.LogError("shapes don't match");
        }

        var squareList = GetAllSquaresCombination(shapeColumns, shapeRows);

        bool canBePlaced = false;

        foreach (var number in squareList)
        {
            bool shapeCanBePlacedOnTheBoard = true;
            foreach (var squareIndexToCheck in originalShapeFilledUpSquares)
            {
                var comp = _gridSquares[number[squareIndexToCheck]].GetComponent<GridSquare>();
                if(comp.SquareOccupied)
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

    private List<int[]> GetAllSquaresCombination(int columns, int rows)
    {
        var squareList = new List<int[]>();
        var lastColumnIndex = 0;
        var lastRowIndex = 0;

        int safeIndex = 0;

        while (lastRowIndex + (rows - 1) < 8)
        {
            var rowData = new List<int>();

            for(var row = lastRowIndex; row < lastRowIndex + rows; row++)
            {
                for(var column = lastColumnIndex; column < lastColumnIndex + columns; column++)
                {
                    rowData.Add(_lineIndicator.line_data[row, column]);
                }
            }

            squareList.Add(rowData.ToArray());

            lastColumnIndex++;

            if (lastColumnIndex + (columns - 1) >= 8)
            {
                lastRowIndex++;
                lastColumnIndex = 0;
            }

            safeIndex++;
            if (safeIndex > 100) 
            {
                break;
            }

        }

        return squareList;
    }
}