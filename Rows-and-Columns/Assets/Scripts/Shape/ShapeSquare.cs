using UnityEngine;
using UnityEngine.UI;

public class ShapeSquare : MonoBehaviour
{
    public Image Image;

    void Start()
    {
        Image.gameObject.SetActive(true);
    }

    public void ActivateSquare()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = true;   
        gameObject.SetActive(true);
        
    }

    public void DeactivateSquare()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;   
        gameObject.SetActive(false);
        

    }


}
