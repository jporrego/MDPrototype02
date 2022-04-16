using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CodeMonkey.Utils;

public class GridClass<TGridObject>
{
    // This creates an event of the delegat type "EventHandler". Additionally we give an object as a generic parameter to
    // handle event data.
    // The OnGridObjectChangedEventArgs keeps track of the x, y of the grid that had its value changed.
    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public int x;
        public int z;
    }

    // Define a two dimensional array and a cellsize to give a position to each x,y values of the array.
    // The grid is generic and takes values or "grid objects". With an x, y position we can access a specific value, or an object and its properties and methods.

    int width;
    int height;
    private float cellSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;
    private bool showDebug = false;

    public GridClass(int width, int height, float cellSize, Vector3 originPosition, Func<GridClass<TGridObject>, int, int, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new TGridObject[width, height];

        // Initialize grid. In the constructor we pass a delegate function "Func<TGridObject> createGridObject" that returns
        // a TGridObject.
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                gridArray[x, z] = createGridObject(this, x, z);
            }
        }

        if (showDebug)
        {
            ShowDebug();
        }
    }

    public Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x, 0f, z) * cellSize + originPosition;
    }

    public void GetXZ(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }

    public void SetGridObject(int x, int z, TGridObject gridObject)
    {
        if (x >= 0 && x < width && z >= 0 && z < height)
        {
            gridArray[x, z] = gridObject;

            // OnGridObjectChanged is called if it has a method subscribed to it.
            // For subscribed functions, it passes 'this' and an OnGridObjectChangedEventArgs object containing the x,y of the modified grid object.
            if (OnGridObjectChanged != null)
            {
                OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, z = z });
            }
        }

    }

    public void SetGridObject(Vector3 worldPosition, TGridObject gridObject)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        SetGridObject(x, z, gridObject);
    }

    public TGridObject GetGridObject(int x, int z)
    {
        if (x >= 0 && x < width && z >= 0 && z < height)
        {
            return gridArray[x, z];
        }
        else
        {
            // Reconsider how to deal with invalid values
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        return GetGridObject(x, z);
    }

    // Function to trigger the OnGridObjectChanged event.
    // Needed in cases where a GridObject itself is not changed, but instead a value inside it is.
    public void TriggerGridObjectChanged(int x, int z)
    {
        if (OnGridObjectChanged != null)
        {
            OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, z = z });
        }
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    public void ShowDebug()
    {
        TextMesh[,] debugTextArray = new TextMesh[width, height];
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                debugTextArray[x, z] = UtilsClass.CreateWorldText(gridArray[x, z]?.ToString(), null, GetWorldPosition(x, z) + new Vector3(cellSize, 0f, cellSize) * 0.5f, 50, Color.white, TextAnchor.MiddleCenter);
                // Debug lines to visualize grid
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, float.MaxValue);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, float.MaxValue);
            }
        }
        // Debug lines to visualize grid
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, float.MaxValue);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, float.MaxValue);

        // A function is subscribed to the OnGridObjectChanged event.
        // When the even is called, this function is also called. It receives the x, y of the modified grid object and
        // updates the corresponding Text object witht the new value.
        OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) =>
        {
            debugTextArray[eventArgs.x, eventArgs.z].text = gridArray[eventArgs.x, eventArgs.z]?.ToString();
        };
    }

}
