using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    public static Action<int> AddScore;

    public static Action CheckIfShapeCanBePlaced;

    public static Action RequestNewShapes;

    public static Action MoveShapeToStartPosition;

    public static Action SetShapeInactive;
}
