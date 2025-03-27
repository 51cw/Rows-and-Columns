using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.PlayerSettings;

public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public GameObject squareShapeImage;
    public Vector3 shapeSelectedScale;
    public Vector2 offset = new Vector2(0f, 200f);

    [HideInInspector]
    public ShapeData CurrentShapeData;

    private List<GameObject> _currentShape = new List<GameObject>();
    private Vector3 _shapeStartScale;
    private RectTransform _transform;
    private bool _shapeDraggable = true;
    private Canvas _canvas;

    public void Awake()
    {
        _shapeStartScale = this.GetComponent<RectTransform>().localScale;
        _transform = this.GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _shapeDraggable = true;
    }
    
    public void CreateShape(ShapeData shapeData)
    {
        CurrentShapeData = shapeData;
        var totalSquareNumber = GetNumberOfSgaures(shapeData);

        while(_currentShape.Count <= totalSquareNumber) 
        { 
            _currentShape.Add(Instantiate(squareShapeImage, transform) as GameObject);
        }

        foreach(var square in _currentShape)
        {
            square.gameObject.transform.position = Vector3.zero;
            square.gameObject.SetActive(false);
        }

        var squareRect = squareShapeImage.GetComponent<RectTransform>();
        var moveDistance = new Vector2(squareRect.rect.width * squareRect.localScale.x, squareRect.rect.height * squareRect.localScale.y);

        int currentIndexInList = 0;
        for(var row = 0; row < shapeData.rows; row++)
        {
            for(var column = 0; column < shapeData.columns; column++)
            {
                if (shapeData.board[row].column[column])
                {
                    _currentShape[currentIndexInList].SetActive(true);
                    _currentShape[currentIndexInList].GetComponent<RectTransform>().localPosition = new Vector2(GetXPositionForShapeSquare(shapeData,column,moveDistance), GetYPositionForShapeSquare(shapeData,row,moveDistance));

                    currentIndexInList++;
                }
            }
        }


    }
    private float GetYPositionForShapeSquare(ShapeData shapeData, int row, Vector2 moveDistance)
    {
        if (shapeData.rows <= 1) return 0f;

        float centerOffset;
        int middleRow = shapeData.rows / 2;

        if (shapeData.rows % 2 != 0) 
        {
            int rowOffset = row - middleRow;
            centerOffset = -rowOffset * moveDistance.y;
        }
        else 
        {
            float centerGap = moveDistance.y;
            int rowOffset = row - middleRow + 1;
            centerOffset = ((rowOffset - 0.5f) * moveDistance.y) * -1;
        }

        return centerOffset;
    }

    private float GetXPositionForShapeSquare(ShapeData shapeData, int column, Vector2 moveDistance)
    {
        if (shapeData.columns <= 1) return 0f;

        float centerOffset;
        int middleCol = shapeData.columns / 2;

        if (shapeData.columns % 2 != 0) 
        {
            int colOffset = column - middleCol;
            centerOffset = colOffset * moveDistance.x;
        }
        else 
        {
            float centerGap = moveDistance.x;
            int colOffset = column - middleCol + 1;
            centerOffset = (colOffset - 0.5f) * moveDistance.x;
        }

        return centerOffset;
    }
    private int GetNumberOfSgaures(ShapeData shapeData)
    {
        int number = 0;

        foreach (var rowData in shapeData.board)
        {
            foreach (var active in rowData.column)
            {
                if (active) 
                    number++;

            }
        }
        return number;
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = shapeSelectedScale;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _transform.anchorMin = Vector2.zero;
        _transform.anchorMax = Vector2.zero;
        _transform.pivot = Vector2.zero;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, eventData.position, Camera.main, out pos);
        _transform.localPosition = pos + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = _shapeStartScale;
        GameEvents.CheckIfShapeCanBePlaced();
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }
}
