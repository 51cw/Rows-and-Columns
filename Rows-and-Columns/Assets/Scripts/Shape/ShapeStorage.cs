using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    public List<ShapeData> shapeData;
    public List<Shape> shapeList;


    void Start()
    {
        foreach (var shape in shapeList)
        {
            var shapeIndex = UnityEngine.Random.Range(0, shapeData.Count);
            shape.CreateShape(shapeData[shapeIndex]);
        }
    }
}
