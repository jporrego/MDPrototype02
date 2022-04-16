using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CodeMonkey.Utils;

public class GridVertical<TGridObject>
{
    // This creates an event of the delegat type "EventHandler". Additionally we give an object as a generic parameter to
    // handle event data.
    // The OnGridObjectChangedEventArgs keeps track of the x, y of the grid that had its value changed.
    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    // Define a two dimensional array and a cellsize to give a position to each x,y values of the array.
    // The grid is generic and takes values or "grid objects". With an x, y position we can access a specific value, or an object and its properties and methods.

    int width;
    int height;
    private float cellSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;

    public GridVertical(int width, int height, float cellSize, Vector3 originPosition, Func<TGridObject> createGridObject)
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
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createGridObject();
            }
        }

        bool showDebug = true;
        if (showDebug)
        {
            TextMesh[,] debugTextArray = new TextMesh[width, height];
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    debugTextArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * 0.5f, 20, Color.white, TextAnchor.MiddleCenter);
                    // Debug lines to visualize grid
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, float.MaxValue);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, float.MaxValue);
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
                debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y]?.ToString();
            };
        }
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void SetGridObject(int x, int y, TGridObject gridObject)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            gridArray[x, y] = gridObject;

            // OnGridObjectChanged is called if it has a method subscribed to it.
            // For subscribed functions, it passes 'this' and an OnGridObjectChangedEventArgs object containing the x,y of the modified grid object.
            if (OnGridObjectChanged != null)
            {
                OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
            }
        }

    }

    public void SetGridObject(Vector3 worldPosition, TGridObject gridObject)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, gridObject);
    }

    public TGridObject GetGridObject(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            // Reconsider how to deal with invalid values
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

    // Function to trigger the OnGridObjectChanged event.
    // Needed in cases where a GridObject itself is not changed, but instead a value inside it is.
    public void TriggerGridObjectChanged(int x, int y)
    {
        if (OnGridObjectChanged != null)
        {
            OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
        }
    }

}
