using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour
{
    // This class will be instantiated instead of a simple GameObject.
    // It holds the data for the building. 
    // The create function instantiates a prefab with the placedObject component, and then fills the component data with
    // the data passed when calling Create()

    private PlacedObjectTypeSO placedObjectTypeSO;
    private Vector2Int origin;
    private PlacedObjectTypeSO.Dir dir;

    public static PlacedObject Create(Vector3 worldPosition, Vector2Int origin, PlacedObjectTypeSO.Dir dir, PlacedObjectTypeSO placedObjectTypeSO)
    {
        Transform placedObjectTransform = Instantiate(
                    placedObjectTypeSO.prefab,
                    worldPosition,
                    Quaternion.Euler(0f, placedObjectTypeSO.GetRotationAngle(dir), 0f));

        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject.placedObjectTypeSO = placedObjectTypeSO;
        placedObject.origin = origin;
        placedObject.dir = dir;

        return placedObject;
    }

    public List<Vector2Int> GetGridPositionList()
    {
        return placedObjectTypeSO.GetGridPositionList(origin, dir);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

}
