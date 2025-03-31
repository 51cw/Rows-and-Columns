using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


// Shape class that handles draggable puzzle pieces with multiple interaction events
public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    // Configuration variables
    public GameObject squareShapeImage;       // Prefab for individual shape squares
    public Vector3 shapeSelectedScale;        // Scale when shape is selected/dragged
    public Vector2 offset = new Vector2(0f, 200f);  // Position offset during drag

    [HideInInspector]
    public ShapeData CurrentShapeData;        // Data structure defining this shape's pattern
    public int TotalSquareNumber { get; set; } // Total active squares in this shape

    // Runtime variables
    private List<GameObject> _currentShape = new List<GameObject>(); // All square objects in this shape
    private Vector3 _shapeStartScale;         // Original scale before selection
    private RectTransform _transform;         // This shape's transform component
    private Canvas _canvas;                   // Parent canvas for coordinate conversion
    private Vector3 _startPosition;           // Original position in the shape palette
    private bool _shapeActive = true;         // Whether this shape is currently active

    // Initialization
    public void Awake()
    {
        _shapeStartScale = this.GetComponent<RectTransform>().localScale;
        _transform = this.GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _startPosition = _transform.localPosition;
    }

    // Event subscription management
    private void OnEnable()
    {
        GameEvents.MoveShapeToStartPosition += MoveShapeToStartPosition;
        GameEvents.SetShapeInactive += SetShapeInactive;
    }

    private void OnDisable()
    {
        GameEvents.MoveShapeToStartPosition -= MoveShapeToStartPosition;
        GameEvents.SetShapeInactive -= SetShapeInactive;
    }

    // Shape state checks
    public bool IsOnStartPosition()
    {
        return _transform.localPosition == _startPosition;
    }

    public bool IsAnyOfShapeSquareActive()
    {
        foreach (var square in _currentShape)
        {
            if (square.gameObject.activeSelf)
                return true;
        }
        return false;
    }

    // Shape activation/deactivation
    public void ActivateShape()
    {
        if (!_shapeActive)
        {
            foreach (var square in _currentShape)
            {
                square?.GetComponent<ShapeSquare>().ActivateSquare();
            }
        }
        _shapeActive = true;
    }

    private void SetShapeInactive()
    {
        if (IsAnyOfShapeSquareActive() && !IsOnStartPosition())
        {
            foreach (var square in _currentShape)
            {
                square.gameObject.SetActive(false);
            }
        }
    }

    public void DeactivateShape()
    {
        if (_shapeActive)
        {
            foreach (var square in _currentShape)
            {
                square?.GetComponent<ShapeSquare>().DeactivateSquare();
            }
        }
        _shapeActive = false;
    }

    // Shape creation and management
    public void RequestNewShape(ShapeData shapeData)
    {
        _transform.localPosition = _startPosition;
        CreateShape(shapeData);
    }

    public void CreateShape(ShapeData shapeData)
    {
        CurrentShapeData = shapeData;
        TotalSquareNumber = GetNumberOfSgaures(shapeData);

        // Ensure we have enough square objects
        while (_currentShape.Count <= TotalSquareNumber)
        {
            _currentShape.Add(Instantiate(squareShapeImage, transform) as GameObject);
        }

        // Reset all squares
        foreach (var square in _currentShape)
        {
            square.gameObject.transform.position = Vector3.zero;
            square.gameObject.SetActive(false);
        }

        // Calculate square positioning
        var squareRect = squareShapeImage.GetComponent<RectTransform>();
        var moveDistance = new Vector2(squareRect.rect.width * squareRect.localScale.x,
                                     squareRect.rect.height * squareRect.localScale.y);

        // Activate and position squares according to shape data
        int currentIndexInList = 0;
        for (var row = 0; row < shapeData.rows; row++)
        {
            for (var column = 0; column < shapeData.columns; column++)
            {
                if (shapeData.board[row].column[column])
                {
                    _currentShape[currentIndexInList].SetActive(true);
                    _currentShape[currentIndexInList].GetComponent<RectTransform>().localPosition =
                        new Vector2(GetXPositionForShapeSquare(shapeData, column, moveDistance),
                                  GetYPositionForShapeSquare(shapeData, row, moveDistance));
                    currentIndexInList++;
                }
            }
        }
    }

    // Positioning calculations
    private float GetYPositionForShapeSquare(ShapeData shapeData, int row, Vector2 moveDistance)
    {
        if (shapeData.rows <= 1) return 0f;

        float centerOffset;
        int middleRow = shapeData.rows / 2;

        if (shapeData.rows % 2 != 0) // Odd number of rows
        {
            int rowOffset = row - middleRow;
            centerOffset = -rowOffset * moveDistance.y;
        }
        else // Even number of rows
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

        if (shapeData.columns % 2 != 0) // Odd number of columns
        {
            int colOffset = column - middleCol;
            centerOffset = colOffset * moveDistance.x;
        }
        else // Even number of columns
        {
            float centerGap = moveDistance.x;
            int colOffset = column - middleCol + 1;
            centerOffset = (colOffset - 0.5f) * moveDistance.x;
        }

        return centerOffset;
    }

    // Helper methods
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

    // Input event handlers
    public void OnPointerClick(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData) { }

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
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform,
            eventData.position, Camera.main, out pos);
        _transform.localPosition = pos + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = _shapeStartScale;
        GameEvents.CheckIfShapeCanBePlaced();
    }

    public void OnPointerDown(PointerEventData eventData) { }

    private void MoveShapeToStartPosition()
    {
        _transform.transform.localPosition = _startPosition;
    }
}