using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class InputController : MonoBehaviour
{
    private bool isSelected = false;
    private PlacedObjectTypeSO selectedObject;
    private BuildingArea currentBuildingArea;
    // Transform for the preview visual, and a vector3 hitpoint to send it to lateUpdate
    Transform previewVisual;
    Vector3 raycastHitLocation;
    // Beign used when instantiating the preview. Review later.
    private Vector3 worldPosition = Vector3.zero;


    private void Start()
    {

    }

    private void Update()
    {
        // If an object is selected, we cast a ray that collides with a grid object.
        // From the grid object we obtain the BuildingArea component and save it currentBuildingArea.
        // The current grid and the raycast are used to show a preview of the object position in the grid, and whether it can be placed or not.
        // When place input happens, we call PlaceObject() in the building area, with the currently selected boject as argument.

        if (isSelected == true)
        {
            LayerMask mask = LayerMask.GetMask("GridLayer");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, mask))
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);

                Transform gridObject = hit.transform.parent;
                currentBuildingArea = gridObject.GetComponent<BuildingArea>();

                raycastHitLocation = hit.point;

                if (Input.GetMouseButtonDown(0))
                {
                    // If it's possible to build, place the object and clear all the variables.
                    // If not, nothing happens.
                    currentBuildingArea.grid.GetXZ(UtilsClass.Get3DMouseWorldPosition(), out int x, out int z);
                    if (currentBuildingArea.CheckIfCanBuild(selectedObject, x, z))
                    {
                        currentBuildingArea.PlaceObject(selectedObject);
                        isSelected = false;
                        Destroy(previewVisual.gameObject);
                        currentBuildingArea = null;
                    };

                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    currentBuildingArea.SetNextDir();
                }

            }

        }
    }
    private void LateUpdate()
    {
        if (selectedObject != null && currentBuildingArea != null && raycastHitLocation != null)
        {
            PreviewObjectLocation(currentBuildingArea, selectedObject, raycastHitLocation);
        }

    }

    public void SetSelectedObject(PlacedObjectTypeSO objectToSelect)
    {
        selectedObject = objectToSelect;
        previewVisual = Instantiate(selectedObject.prefab, worldPosition, Quaternion.identity);
        isSelected = true;
    }

    // Revisit Preview visual to use a PlacedObjectTypeSO to be able to set its rotation.
    public void PreviewObjectLocation(BuildingArea buildingArea, PlacedObjectTypeSO objectToPlace, Vector3 hitPoint)
    {
        float lerpSpeedd = 20f;
        buildingArea.grid.GetXZ(hitPoint, out int x, out int z);
        buildingArea.grid.GetWorldPosition(x, z);
        previewVisual.rotation = Quaternion.Lerp(previewVisual.rotation, Quaternion.Euler(0f, objectToPlace.GetRotationAngle(buildingArea.dir), 0f), Time.deltaTime * lerpSpeedd);
        previewVisual.position = Vector3.Lerp(previewVisual.position, buildingArea.calculateWorldPosition(objectToPlace, x, z), Time.deltaTime * lerpSpeedd);


        // PROOF OF CONCEPT. REVISIT ONCE THE OBJECT STRUCTURE IS DEFINED.
        Color originalColor = previewVisual.GetChild(0).GetChild(0).gameObject.GetComponent<Renderer>().materials[0].GetColor("_AlbedoColor");
        if (!currentBuildingArea.CheckIfCanBuild(selectedObject, x, z))
        {
            int numOfChildren = previewVisual.GetChild(0).childCount;


            for (int i = 0; i < numOfChildren; i++)
            {
                GameObject child = previewVisual.GetChild(0).GetChild(i).gameObject;
                Material material = child.GetComponent<Renderer>().materials[0];
                material.SetColor("_AlbedoColor", Color.red);
            }
        }
        else
        {
            int numOfChildren = previewVisual.GetChild(0).childCount;

            for (int i = 0; i < numOfChildren; i++)
            {
                GameObject child = previewVisual.GetChild(0).GetChild(i).gameObject;
                Material material = child.GetComponent<Renderer>().materials[0];
                material.SetColor("_AlbedoColor", Color.black);
            }
        }
    }
}
