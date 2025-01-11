using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Class used for tracking and displaying the items the player has in their inventory
public class InventoryUI : MonoBehaviour
{
    private static Vector2 size = new Vector2(55f, 55f);
    private static Vector2 pos = new Vector2(0, -225f + size.x / 2);

    private GameObject basePanel; //Used as a template for other panels
    private List<GameObject> items = new List<GameObject>();

    private string selectedItem = null;

    // Start function getting the template UI element
    void Start()
    {
        basePanel = transform.GetChild(0).gameObject;
    }

    // Function used to create new inventory UI elements from template UI element and setting parameters
    public void NewItem(string name)
    {
        GameObject newPanel = Instantiate(basePanel);
        RectTransform panelTransform = newPanel.GetComponent<RectTransform>();

        newPanel.transform.SetParent(gameObject.transform);
        newPanel.transform.localScale = Vector3.one;
        newPanel.name = name;
        newPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = name;
        newPanel.SetActive(true);

        items.Add(newPanel);
        panelTransform.sizeDelta = size;

        UpdateUI();
    }

    // Function removing the specified item from inventory
    public void RemoveItem(string name)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].name == name)
            {
                Destroy(items[i]);
                items.RemoveAt(i);
                selectedItem = null;
                UpdateUI();
                break;
            }
        }
    }

    // Function setting the selected item as held, or deselcting held item
    public void SelectItem(int itemNum)
    {
        if (itemNum < items.Count)
            if (items[itemNum].GetComponent<Image>().color == Color.white)
            {
                ClearSelectedItem();
                items[itemNum].GetComponent<Image>().color = Color.gray;
                selectedItem = items[itemNum].name;
            }
            else
            {
                ClearSelectedItem();
            }
    }

    // Function deselecting held item
    private void ClearSelectedItem()
    {
        foreach (GameObject item in items)
            if (item.GetComponent<Image>().color == Color.gray)
            {
                item.GetComponent<Image>().color = Color.white;
                selectedItem = null;
            }
    }

    // Function dynamically setting UI elements evenly spaced, base on the amount of items in invetory
    private void UpdateUI()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].GetComponent<RectTransform>().localPosition = new Vector2(i * size.x - size.x * (items.Count - 1) / 2, pos.y);
            items[i].transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{i + 1}";
        }
            
    }

    // Function returning the name of the selected item
    public string GetActiveItemName()
    {
        return selectedItem;
    }
}