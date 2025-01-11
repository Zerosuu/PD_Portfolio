using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : BaseMenuUI
{
    private readonly List<GameObject> visualCollectables = new List<GameObject>();
    private readonly RotateObject rotateScript;

    public AudioSource papperAudio1;
    public AudioSource papperAudio2;
    public AudioSource papperAudio3;
    public AudioSource papperAudio4;
    public AudioSource papperAudio5;
    public AudioSource papperAudio6;

    /*void Start()
    {
        //rotateScript = transform.Find("CollectablesMenu").Find("CollectableView").Find("ItemRenderer").GetComponent<RotateObject>();

        Transform objectsParent = transform.Find("VisualCollectables").Find("Objects");

        foreach (Transform child in objectsParent)
            visualCollectables.Add(child.gameObject);
    }*/

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

    public void SetInspectItem(int num)
    {
        rotateScript.SetItem(visualCollectables[num]);
    }

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

    public void ResumeGame()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // play inner papper audio functions we will connect it to on click so when we read the collectible pages we hear VA
    public void PlayInnerPapperAudio1() { papperAudio1.Play(); }
    public void PlayInnerPapperAudio2() { papperAudio2.Play(); }
    public void PlayInnerPapperAudio3() { papperAudio3.Play(); }
    public void PlayInnerPapperAudio4() { papperAudio4.Play(); }
    public void PlayInnerPapperAudio5() { papperAudio5.Play(); }
    public void PlayInnerPapperAudio6() { papperAudio6.Play(); }

    // stop playing the inner papper audio functions we will connect it to on click for the back button
    public void StopInnerPapperAudio1() { papperAudio1.Stop(); }
    public void StopInnerPapperAudio2() { papperAudio2.Stop(); }
    public void StopInnerPapperAudio3() { papperAudio3.Stop(); }
    public void StopInnerPapperAudio4() { papperAudio4.Stop(); }
    public void StopInnerPapperAudio5() { papperAudio5.Stop(); }
    public void StopInnerPapperAudio6() { papperAudio6.Stop(); }
}