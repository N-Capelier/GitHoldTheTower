using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuVideo : MonoBehaviour
{
    [SerializeField]
    private Toggle fullSceenToggle;
    [SerializeField]
    private Toggle windonToggle;
    [SerializeField]
    private Dropdown resolutionDropDown;


    [SerializeField]
    private ScriptableMenuParams menuParams;

    private bool firstStart = true;

    void Start()
    {
        //Load all params for the window
        fullSceenToggle.isOn = menuParams.isFullScreen;
        windonToggle.isOn = !menuParams.isFullScreen;
        resolutionDropDown.value = menuParams.ValueInDropDownScreenResolution;
        SetResolution(resolutionDropDown);
        firstStart = false;
    }

    public void OnPressedFullScreen(bool isToogle)
    {
        //Set window
        fullSceenToggle.isOn = isToogle;
        windonToggle.isOn = !isToogle;
        //Save you menu Params script
        menuParams.isFullScreen = isToogle;
        if (isToogle)
        {
            Screen.fullScreen = true;
        }
        if(!firstStart)
            SaveManager.SaveParams(ref menuParams);
    }

    public void OnPressedWindow(bool isToogle)
    {
        //Set window
        fullSceenToggle.isOn = !isToogle;
        windonToggle.isOn = isToogle;

        //Save you menu Params script
        menuParams.isFullScreen = !isToogle;

        if (isToogle)
        {
            Screen.fullScreen = false;
        }
        if(!firstStart)
            SaveManager.SaveParams(ref menuParams);
    }

    public void SetResolution(Dropdown dropDown)
    {
        switch (dropDown.value)
        {
            case 0:
                Screen.SetResolution(1920, 1080, fullSceenToggle.isOn);
            break;
            case 1:
                Screen.SetResolution(2560, 1440, fullSceenToggle.isOn);
                break;
            case 2:
                Screen.SetResolution(1600, 900, fullSceenToggle.isOn);
                break;
            case 3:
                Screen.SetResolution(1920, 1200, fullSceenToggle.isOn);
                break;
            case 4:
                Screen.SetResolution(2560, 1600, fullSceenToggle.isOn);
                break;
            case 5:
                Screen.SetResolution(1680, 1050, fullSceenToggle.isOn);
                break;

            default:
                break;
        }
        menuParams.ValueInDropDownScreenResolution = dropDown.value;
        if (!firstStart)
        {
            SaveManager.SaveParams(ref menuParams);
        }
    }
}
