using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSquare : MonoBehaviour
{
    public Image hoverImage;
    public Image activeImage;
    public Image normalImage;

    public bool Selected { get; set; }
    public int SquareIndex { get; set; }
    public bool SquareOccupied { get; set; }

    public Grid grid;
    void Start()
    {
        Selected = false;
        SquareOccupied = false;
        normalImage.gameObject.SetActive(true);
    }

    public bool CanWeUseThisSQ()
    {
        return hoverImage.gameObject.activeSelf;
    }

    public void PlaceShapeOnBoard()
    {
        ActivateSquare();
    }

    public void ActivateSquare()
    {
        hoverImage.gameObject.SetActive(false);
        activeImage.gameObject.SetActive(true);
        Selected = true;
        SquareOccupied = true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (!SquareOccupied)
        {
            Selected = true;
            hoverImage.gameObject.SetActive(grid.CanShapeBePlaced());
            normalImage.gameObject.SetActive(!grid.CanShapeBePlaced());
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        Selected = true;

        if (!SquareOccupied && grid.CanShapeBePlaced())
        {
            hoverImage.gameObject.SetActive(grid.CanShapeBePlaced());
            normalImage.gameObject.SetActive(!grid.CanShapeBePlaced());
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!SquareOccupied)
        {
            Selected = false;
            hoverImage.gameObject.SetActive(false);
            normalImage.gameObject.SetActive(true);

        }
    }
}