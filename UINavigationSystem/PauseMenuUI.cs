using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Subclass of BaseMenuUI used for UI Navigation and various functionality for the pause menu
public class PauseMenuUI : BaseMenuUI
{
    private readonly List<GameObject> visualCollectables = new List<GameObject>(); // Unused in this version
    private readonly RotateObject rotateScript; // Unused in this version

    // Update function listening to button inputs to determine if program should go up a submenu or resume the game
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            foreach (GameObject item in _menuLayerSwap)
                Debug.Log(item.transform.parent.name);

            if (Time.timeScale != 0)
            {
                Cursor.lockState = CursorLockMode.None;
                transform.Find("Content").gameObject.SetActive(true);
                Time.timeScale = 0;
            }
            else
                SwapLayer();
        }
    }

    // Ovveride function handling how submenus are navigated within the UI
    protected override void SwapLayer()
    {
        if (_menuLayerSwap.Count > 0)
        {
            GameObject item = _menuLayerSwap.Peek();
            item.GetComponent<Button>().onClick.Invoke();
        }
        else
        {
            transform.Find("Content").gameObject.SetActive(false);
            ResumeGame();
        }
    }
    
    // Function setting the specifid item to be displayed and rotated; Unused in this version
    public void SetInspectItem(int num)
    {
        rotateScript.SetItem(visualCollectables[num]);
    }

    // Function loading all available rotateable items in collection, triggered on Start function; Unused in this version
    public void LoadCollectedItems()
    {
        GameObject buttonList = transform.Find("SubMenus/CollectablesMenu/PagesView/CollectableButtons").gameObject;
        List<string> implementedItems = new List<string>(); //Keeps track of what items are implemented in UI

        foreach (Transform item in transform.Find("SubMenus/CollectablesMenu/CollectableView"))
            implementedItems.Add(item.name);

        foreach(string itemName in PlayerPrefs.GetString("CollectedItems").Split(","))
        {
            if (itemName != "" && implementedItems.Contains(itemName)) //Check if item is implemented in UI
                buttonList.transform.Find(itemName).gameObject.SetActive(true);
        }
    }

    // Function resuming the game when paused
    public void ResumeGame()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
