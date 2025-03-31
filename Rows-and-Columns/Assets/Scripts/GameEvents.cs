using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    // Event for adding to player's score
    // Parameters:
    //   int - The amount of points to add
    public static Action<int> AddScore;

    // Event for game over state
    // Parameters:
    //   bool - Whether a new best score was achieved
    public static Action<bool> GameOver;

    // Event to check if current shape can be placed on grid
    // No parameters
    public static Action CheckIfShapeCanBePlaced;

    // Event to request new shapes when needed
    // No parameters
    public static Action RequestNewShapes;

    // Event to return current shape to start position
    // No parameters
    public static Action MoveShapeToStartPosition;

    // Event to deactivate current shape
    // No parameters
    public static Action SetShapeInactive;

    // Event to update the best score display
    // Parameters:
    //   int - The new best score value
    public static Action<int> UpdateBestScoreBar;
}