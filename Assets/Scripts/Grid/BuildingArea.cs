using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class BuildingArea : MonoBehaviour
{
    [SerializeField] private PlacedObjectTypeSO placedObjectTypeSO;

    public GridClass<GridObject> grid;
    // This should be in another class
    public PlacedObjectTypeSO.Dir dir = PlacedObjectTypeSO.Dir.Down;
    private int xScale;
    private int zScale;
    private float cellSize = 0.25f;
    // offset?
    // Start is called before the first frame update
    void Start()
    {

        calculateGridScale();
        grid = new GridClass<GridObject>(xScale, zScale, cellSize, transform.position, (GridClass<GridObject> g, int x, int z) => new GridObject(g, x, z));
    }


    private void Update()
    {
    }

    public void Test()
    {
        if (true /*Input.GetMouseButtonDown(0)*/)
        {

            grid.GetXZ(UtilsClass.Get3DMouseWorldPosition(), out int x, out int z);

            // Get list of positions that will be used by the object
            List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(new Vector2Int(x, z), dir);

            // Test if  can build is true in those positions
            bool canBuild = true;
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                if (grid.GetGridObject(gridPosition.x, gridPosition.y) == null)
                {
                    canBuild = false;
                    break;
                }
                if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
                {
                    // Can't build here
                    canBuild = false;
                    break;
                }
            }

            if (canBuild)
            {
                // Calculates the rotation offset and adds it to the corresponding world position.
                // Multiplies the offset with the grid cellsize to adjust it correctly.
                Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
                Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) +
                    new Vector3(rotationOffset.x, 0f, rotationOffset.y) * grid.GetCellSize();

                PlacedObject placedObject = PlacedObject.Create(placedObjectWorldPosition, new Vector2Int(x, z), dir, placedObjectTypeSO);

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
                }
            }
            else
            {
                Debug.Log("Cannot build");
            }

        }

        if (Input.GetMouseButtonDown(1))
        {
            GridObject gridObject = grid.GetGridObject(UtilsClass.Get3DMouseWorldPosition());
            PlacedObject placedObject = gridObject.GetPlacedObject();
            if (placedObject != null)
            {
                placedObject.DestroySelf();
                // Get list of positions that will be used by the object
                List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                }
            }
        }

    }

    public void PlaceObject(PlacedObjectTypeSO objectToPlace)
    {
        grid.GetXZ(UtilsClass.Get3DMouseWorldPosition(), out int x, out int z);
        // Get list of positions that will be used by the object
        List<Vector2Int> gridPositionList = objectToPlace.GetGridPositionList(new Vector2Int(x, z), dir);

        if (CheckIfCanBuild(objectToPlace, x, z))
        {
            // Calculates the rotation offset and adds it to the corresponding world position.
            // Multiplies the offset with the grid cellsize to adjust it correctly.

            // TEST
            //Vector2Int rotationOffset = objectToPlace.GetRotationOffset(dir);
            //Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) +
            //new Vector3(rotationOffset.x, 0f, rotationOffset.y) * grid.GetCellSize();
            // TEST
            Vector3 placedObjectWorldPosition = calculateWorldPosition(objectToPlace, x, z);
            PlacedObject placedObject = PlacedObject.Create(placedObjectWorldPosition, new Vector2Int(x, z), dir, objectToPlace);

            foreach (Vector2Int gridPosition in gridPositionList)
            {
                grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
            }

            dir = PlacedObjectTypeSO.Dir.Down;
        }
        else
        {
            Debug.Log("Cannot build");
        }
    }

    public void SetNextDir()
    {
        dir = PlacedObjectTypeSO.GetNextDir(dir);
    }

    public Vector3 calculateWorldPosition(PlacedObjectTypeSO objectToPlace, int x, int z)
    {
        Vector2Int rotationOffset = objectToPlace.GetRotationOffset(dir);
        Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) +
            new Vector3(rotationOffset.x, 0f, rotationOffset.y) * grid.GetCellSize();

        return placedObjectWorldPosition;
    }

    public bool CheckIfCanBuild(PlacedObjectTypeSO objectToPlace, int x, int z)
    {
        // Get list of positions that will be used by the object
        List<Vector2Int> gridPositionList = objectToPlace.GetGridPositionList(new Vector2Int(x, z), dir);

        // Check it's possible to build in those positions.
        // canBuild will be false under two conditions:
        // 1. The positions to occupy are outside the bounds of the grid array.
        // 2. The positions are already used by another object.
        bool canBuild = true;
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            if (grid.GetGridObject(gridPosition.x, gridPosition.y) == null)
            {
                canBuild = false;
                break;
            }
            if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
            {
                // Can't build here
                canBuild = false;
                break;
            }
        }
        return canBuild;
    }

    // This should be in another class
    public class GridObject
    {
        private GridClass<GridObject> grid;
        private int x;
        private int z;
        private PlacedObject placedObject;

        public GridObject(GridClass<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public void SetPlacedObject(PlacedObject placedObject)
        {
            this.placedObject = placedObject;
            grid.TriggerGridObjectChanged(x, z);
        }

        public PlacedObject GetPlacedObject()
        {
            return placedObject;
        }

        public void ClearPlacedObject()
        {
            placedObject = null;
            grid.TriggerGridObjectChanged(x, z);
        }

        public bool CanBuild()
        {
            return placedObject == null;
        }

        public override string ToString()
        {
            return x + ", " + z + "\n" + placedObject;
        }
    }

    void calculateGridScale()
    {
        Transform gridAreaTransform = transform.GetChild(0).transform;
        xScale = Mathf.FloorToInt(gridAreaTransform.localScale.x * 40);
        zScale = Mathf.FloorToInt(gridAreaTransform.localScale.z * 40);
    }

}
