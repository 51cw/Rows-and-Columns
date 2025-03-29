using NUnit.Framework;
using System.Collections.Generic;
using System;
using UnityEngine;
using JetBrains.Annotations;

public class Grid : MonoBehaviour
{
    public ShapeStorage shapeStorage;
    public int Columns = 0;
    public int Rows = 0;
    public GameObject GridSquare;
    public Vector2 StartPosition = new Vector2(0.0f, 0.0f);
    public float SquareScale = 0.5f;
    public float EverySquareOffset = 0.0f;

    private Vector2 _offset = new Vector2(0.0f, 0.0f);
    private List<GameObject> _gridSquares = new List<GameObject>();
    private LineIndicator _lineIndicator;

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
                //gridSquare.ActivateSquare();
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

        if(completedLines > 2)
        {
            //TODO: play bonus animation
        }

        if (completedLines > 0)
        {
            var totalScore = (10 * completedLines);
            GameEvents.AddScore(totalScore);
        }
        
        //TODO: Add score
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
}