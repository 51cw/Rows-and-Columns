using UnityEngine;
using UnityEngine.UI;

public class ShapeSquare : MonoBehaviour
{
    // Reference to the Image component used to visually represent this square
    public Image Image;

    // Initialization - ensures the square is visible when created
    void Start()
    {
        Image.gameObject.SetActive(true); // Activate the visual representation
    }

    // Enables this square for interaction and makes it visible
    public void ActivateSquare()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = true; // Enable collision detection
        gameObject.SetActive(true); // Make the entire square active
    }

    // Disables this square for interaction and hides it
    public void DeactivateSquare()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false; // Disable collision detection
        gameObject.SetActive(false); // Make the entire square inactive
    }
}