using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int Columns = 0;
    public int Rows = 0;
    public GameObject GridSquare;
    public Vector2 StartPosition = new Vector2(0.0f, 0.0f);
    public float SquareScale = 0.5f;
    public float EverySquareOffset = 0.0f;

    private Vector2 _offset = new Vector2(0.0f, 0.0f);
    private List<GameObject> _gridSquares = new List<GameObject>();



    void Start()
    {
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
                _gridSquares[_gridSquares.Count - 1].transform.SetParent(this.transform);
                _gridSquares[_gridSquares.Count - 1].transform.localScale = new Vector3(SquareScale, SquareScale, SquareScale);
                
                SquareIndex++;
            }
        }
    }

    private void SetGridSquarePosition()
    {
        int ColNum = 0;
        int RowNum = 0;
        bool row_moved = false;

        var square_rect = _gridSquares[0].GetComponent<RectTransform>();

        _offset.x = square_rect.rect.width * square_rect.transform.localScale.x + EverySquareOffset;
        _offset.y = square_rect.rect.height * square_rect.transform.localScale.y + EverySquareOffset;

        foreach (GameObject square in _gridSquares)
        {
            if (ColNum + 1 > Columns)
            {
                ColNum = 0;
                RowNum++;
                row_moved = false;
            }

            var pos_x_offset = _offset.x * ColNum;
            var pos_y_offset = _offset.y * RowNum;
             
            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(StartPosition.x + pos_x_offset, StartPosition.y - pos_y_offset);
            square.GetComponent<RectTransform>().localPosition = new Vector3(StartPosition.x + pos_x_offset, StartPosition.y - pos_y_offset, 0.0f);
            ColNum++;
        }

    }
}
