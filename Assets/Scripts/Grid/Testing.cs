using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CodeMonkey.Utils;

public class Testing : MonoBehaviour
{
    private GridClass<bool> grid;
    // Start is called before the first frame update
    void Start()
    {
        //grid = new GridClass<bool>(5, 2, 1f, Vector3.zero, () => false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            grid.SetGridObject(UtilsClass.GetMouseWorldPositionWithZ(), true);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(grid.GetGridObject(UtilsClass.GetMouseWorldPositionWithZ()));
        }
    }
}
