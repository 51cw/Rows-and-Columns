using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.ProbeAdjustmentVolume;

public class ShapeStorage : MonoBehaviour
{
    // List of all possible shape configurations available
    public List<ShapeData> shapeData;

    // List of active shape objects in the game
    public List<Shape> shapeList;

    // Subscribe to shape request events when enabled
    private void OnEnable()
    {
        GameEvents.RequestNewShapes += RequestNewShapes;
    }

    // Unsubscribe from events when disabled
    private void OnDisable()
    {
        GameEvents.RequestNewShapes -= RequestNewShapes;
    }

    // Initialize shapes with random configurations at game start
    void Start()
    {
        foreach (var shape in shapeList)
        {
            // Assign a random shape configuration to each shape object
            var shapeIndex = UnityEngine.Random.Range(0, shapeData.Count);
            shape.RequestNewShape(shapeData[shapeIndex]);
        }
    }

    // Gets the currently selected/dragged shape
    public Shape GetCurrentSelectedShape()
    {
        foreach (var shape in shapeList)
        {
            // A shape is considered selected if:
            // 1. It's not in its start position
            // 2. It has at least one active square
            if (!shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
                return shape;
        }

        Debug.LogError("No shape selected");
        return null;
    }

    // Handles requests for new shapes (called when needed)
    private void RequestNewShapes()
    {
        foreach (var shape in shapeList)
        {
            // Assign new random shape configurations
            var shapeIndex = UnityEngine.Random.Range(0, shapeData.Count);
            shape.RequestNewShape(shapeData[shapeIndex]);
        }
    }
}