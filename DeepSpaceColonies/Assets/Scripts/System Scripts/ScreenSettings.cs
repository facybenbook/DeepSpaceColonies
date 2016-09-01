/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Allows UnityEvents to toggle screen settings
        - Resolution
        - Darkness
        - Fullscreen
 ****************************************************/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenSettings : MonoBehaviour
{
    //A reference to this manager that can be accessed anywhere
    public static ScreenSettings screenManagerRef;

    [HideInInspector]
    public int screenResDropdownNum = 0;
    [HideInInspector]
    public ScreenResolution screenResDropdownEnum = ScreenResolution.r1920x1200;

    [HideInInspector]
    public float darkness = 0f;


    //Function called on Initialization
    private void Awake()
    {
        //If there isn't already a static reference to this manager, this instance becomes the static reference
        if (screenManagerRef == null)
        {
            screenManagerRef = GetComponent<ScreenSettings>();
        }
    }


    //Function called externally. Turns on/off full screen mode
    public void ToggleFullscreen(bool isFullscreenOn)
    {
        Screen.fullScreen = isFullscreenOn;
    }

    
    //Function called externally. Changes the screen resolution based on the enum given
    public void ToggleScreenResolution(ScreenResolution screenRes_)
    {
        screenManagerRef.screenResDropdownEnum = screenRes_;

        //Sets the screen resolution based on the enum given
        switch (screenRes_)
        {
            case ScreenResolution.r1024x768:
                Screen.SetResolution(1024, 768, Screen.fullScreen);
                break;

            case ScreenResolution.r1280x800:
                Screen.SetResolution(1280, 800, Screen.fullScreen);
                break;

            case ScreenResolution.r1280x1024:
                Screen.SetResolution(1280, 1024, Screen.fullScreen);
                break;

            case ScreenResolution.r1366x768:
                Screen.SetResolution(1366, 768, Screen.fullScreen);
                break;

            case ScreenResolution.r1440x900:
                Screen.SetResolution(1440, 900, Screen.fullScreen);
                break;

            case ScreenResolution.r1600x900:
                Screen.SetResolution(1600, 900, Screen.fullScreen);
                break;

            case ScreenResolution.r1680x1050:
                Screen.SetResolution(1680, 1050, Screen.fullScreen);
                break;

            case ScreenResolution.r1920x1080:
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                break;

            case ScreenResolution.r1920x1200:
                Screen.SetResolution(1920, 1200, Screen.fullScreen);
                break;
        }
    }

    
    //Function called externally. Changes the screen resolution based on the enum given
    public void ToggleScreenResolutionDropdown(int screenRes_)
    {
        screenManagerRef.screenResDropdownNum = screenRes_;

        //Sets the screen resolution based on the enum given
        switch (screenRes_)
        {
            case 8:
                Screen.SetResolution(1024, 768, Screen.fullScreen);
                break;

            case 7:
                Screen.SetResolution(1280, 800, Screen.fullScreen);
                break;

            case 6:
                Screen.SetResolution(1280, 1024, Screen.fullScreen);
                break;

            case 5:
                Screen.SetResolution(1366, 768, Screen.fullScreen);
                break;

            case 4:
                Screen.SetResolution(1440, 900, Screen.fullScreen);
                break;

            case 3:
                Screen.SetResolution(1600, 900, Screen.fullScreen);
                break;

            case 2:
                Screen.SetResolution(1680, 1050, Screen.fullScreen);
                break;

            case 1:
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                break;

            case 0:
                Screen.SetResolution(1920, 1200, Screen.fullScreen);
                break;
        }
    }

    
    //Function called externally. Changes the alpha of the screen darkness object on the global data object's canvas
    public void SetDarkness(float screenDarkness_)
    {
        screenManagerRef.darkness = 0.8f - screenDarkness_;

        if (screenManagerRef.darkness > 1)
        {
            screenManagerRef.darkness = 1;
        }
        else if (screenManagerRef.darkness < 0)
        {
            screenManagerRef.darkness = 0;
        }

        EventManager.TriggerEvent("ChangeDarkness");
    }
}


//Enums used in ScreenSettings.cs to set the screen resolution size
public enum ScreenResolution
{
    r1920x1200,
    r1920x1080,
    r1680x1050,
    r1600x900,
    r1440x900,
    r1366x768,
    r1280x1024,
    r1280x800,
    r1024x768
}