using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System;
using UnityEngine.UI;

public abstract class BaseMenuUI : MonoBehaviour
{
    private VolumeProfile _volumeProfile;
    private List<Transform> _sliderList = new List<Transform>();

    protected Stack<GameObject> _menuLayerSwap = new Stack<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        _volumeProfile = transform.Find("OptionsVolume").GetComponent<Volume>().profile;

        foreach (Transform sliderParent in transform.Find("SubMenus/Options/SliderParents")) //Populate sliderlist
            _sliderList.Add(sliderParent);

        foreach (Transform sliderParetTransform in _sliderList) //Update set slider values
            sliderParetTransform.Find("Slider").GetComponent<Slider>().value = PlayerPrefs.GetFloat(sliderParetTransform.name, 0);
    }

    protected virtual void SwapLayer()
    {
        if (_menuLayerSwap.Count > 0)
        {
            GameObject item = _menuLayerSwap.Peek();
            item.GetComponent<Button>().onClick.Invoke();
        }
    }

    public void AddLayer(GameObject button)
    {
        _menuLayerSwap.Push(button);
    }

    public void RemoveLayer()
    {
        if (_menuLayerSwap.Pop().transform.parent.name == "Options")
            foreach (Transform sliderParetTransform in _sliderList)
                PlayerPrefs.SetFloat(sliderParetTransform.name, sliderParetTransform.Find("Slider").GetComponent<Slider>().value);
    }

    public void AdjustMasterVolume(float input)
    {
        AudioListener.volume = input + 1;
    }

    public void AdjustBrightness(float input)
    {
        if (_volumeProfile != null && _volumeProfile.TryGet<ColorAdjustments>(out ColorAdjustments colorAdjustment))
            colorAdjustment.postExposure.value = input * 2;
        else
            Debug.Log("PauseMenuUI: Volume/ColorAdjustment not found");
    }

    public void AdjustSensitivity(float input)
    {
        Camera.main.transform.parent.GetComponent<firstPersonController>().AdjustSensitivity(input);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
