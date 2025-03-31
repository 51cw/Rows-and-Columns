using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSquare : MonoBehaviour
{
    // UI Image references for different square states
    public Image hoverImage;    // Visual when square is hovered/selected
    public Image activeImage;  // Visual when square is occupied by a shape
    public Image normalImage;  // Default visual state

    // Square state properties
    public bool Selected { get; set; }      // Whether square is currently selected
    public int SquareIndex { get; set; }    // Unique identifier for this square
    public bool SquareOccupied { get; set; } // Whether square contains a shape

    public Grid grid;  // Reference to parent Grid controller

    // Initialization
    void Start()
    {
        // Reset to default state
        Selected = false;
        SquareOccupied = false;
        normalImage.gameObject.SetActive(true);  // Show normal visual
    }

    // Checks if square is available for placement
    public bool CanWeUseThisSQ()
    {
        // Square is usable if hover image is active (shape fits here)
        return hoverImage.gameObject.activeSelf;
    }

    // Called when a shape is placed on this square
    public void PlaceShapeOnBoard()
    {
        ActivateSquare();  // Mark as occupied
    }

    // Activates square (marks as occupied visually and logically)
    public void ActivateSquare()
    {
        hoverImage.gameObject.SetActive(false);  // Hide hover
        activeImage.gameObject.SetActive(true);  // Show occupied visual
        Selected = true;                        // Mark as selected
        SquareOccupied = true;                  // Mark as occupied
    }

    // Deactivates square (returns to normal state)
    public void DeactivateSquare()
    {
        activeImage.gameObject.SetActive(false);  // Hide occupied visual
        normalImage.gameObject.SetActive(true);   // Show normal visual
    }

    // Clears occupation status
    public void ClearOccupied()
    {
        Selected = false;        // No longer selected
        SquareOccupied = false;  // No longer occupied
    }

    // Triggered when a shape enters this square's collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!SquareOccupied)  // Only respond if not already occupied
        {
            Selected = true;  // Mark as selected
            // Show hover state if shape can be placed, otherwise show normal
            hoverImage.gameObject.SetActive(grid.CanShapeBePlaced());
            normalImage.gameObject.SetActive(!grid.CanShapeBePlaced());
        }
    }

    // Triggered while a shape remains in this square's collider
    private void OnTriggerStay2D(Collider2D collision)
    {
        Selected = true;  // Keep marked as selected

        if (!SquareOccupied)  // Only respond if not already occupied
        {
            // Show hover state if shape can be placed, otherwise show normal
            hoverImage.gameObject.SetActive(grid.CanShapeBePlaced());
            normalImage.gameObject.SetActive(!grid.CanShapeBePlaced());
        }
    }

    // Triggered when a shape exits this square's collider
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!SquareOccupied)  // Only respond if not already occupied
        {
            Selected = false;  // No longer selected
            hoverImage.gameObject.SetActive(false);  // Hide hover
            normalImage.gameObject.SetActive(true);  // Show normal visual
        }
    }
}