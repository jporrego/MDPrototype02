using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Shop : MonoBehaviour
{
    public Transform menuItemTemplate;
    private Object[] menuItemsArray;

    private GameObject inputController;
    // Start is called before the first frame update
    void Start()
    {
        inputController = GameObject.Find("InputController");
        menuItemsArray = Resources.LoadAll("ScriptableObjects", typeof(PlacedObjectTypeSO));
        foreach (PlacedObjectTypeSO item in menuItemsArray)
        {
            Transform newMenuItem = Instantiate(menuItemTemplate) as Transform;
            newMenuItem.transform.SetParent(transform, false);
            newMenuItem.Find("Text").GetComponent<UnityEngine.UI.Text>().text = item.nameString;
            newMenuItem.GetComponent<Button>().onClick.AddListener(() => OnMenuItemClick(item));
        }
    }

    void OnMenuItemClick(PlacedObjectTypeSO item)
    {
        inputController.GetComponent<InputController>().SetSelectedObject(item);
    }
}
