using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System;
using UnityEngine.UI;

// Class used as base for all UI menus, set as baseline to navigate through UI and includes settings functions
public abstract class BaseMenuUI : MonoBehaviour
{
    private VolumeProfile _volumeProfile;
    private List<Transform> _sliderList = new List<Transform>();

    protected Stack<GameObject> _menuLayerSwap = new Stack<GameObject>();

    // Start function setting defaults for class
    void Start()
    {
        _volumeProfile = transform.Find("OptionsVolume").GetComponent<Volume>().profile;

        foreach (Transform sliderParent in transform.Find("SubMenus/Options/SliderParents")) //Populate sliderlist
            _sliderList.Add(sliderParent);

        foreach (Transform sliderParetTransform in _sliderList) //Update set slider values
            sliderParetTransform.Find("Slider").GetComponent<Slider>().value = PlayerPrefs.GetFloat(sliderParetTransform.name, 0);
    }

    // Function determining how UI should be navigated when going up submenus
    protected virtual void SwapLayer()
    {
        if (_menuLayerSwap.Count > 0)
        {
            GameObject item = _menuLayerSwap.Peek();
            item.GetComponent<Button>().onClick.Invoke();
        }
    }

    // Function adding the specified submenu to be able to accurately go back
    public void AddLayer(GameObject button)
    {
        _menuLayerSwap.Push(button);
    }

    // Function removing the current submenu from list when going up and saves set options if the current was the options submenu
    public void RemoveLayer()
    {
        if (_menuLayerSwap.Pop().transform.parent.name == "Options")
            foreach (Transform sliderParetTransform in _sliderList)
                PlayerPrefs.SetFloat(sliderParetTransform.name, sliderParetTransform.Find("Slider").GetComponent<Slider>().value);
    }

    // Function adjusting the volume of the game through the general audio asset, set on a slider
    public void AdjustMasterVolume(float input)
    {
        AudioListener.volume = input + 1;
    }

    // Function adjusting the brightness of the game, set on a slider
    public void AdjustBrightness(float input)
    {
        if (_volumeProfile != null && _volumeProfile.TryGet<ColorAdjustments>(out ColorAdjustments colorAdjustment))
            colorAdjustment.postExposure.value = input * 2;
        else
            Debug.Log("PauseMenuUI: Volume/ColorAdjustment not found");
    }

    // Function adjusting the mouse sensitivity of the game, set on a slider
    public void AdjustSensitivity(float input)
    {
        Camera.main.transform.parent.GetComponent<firstPersonController>().AdjustSensitivity(input);
    }

    // Function closing the game
    public void QuitGame()
    {
        Application.Quit();
    }
}
