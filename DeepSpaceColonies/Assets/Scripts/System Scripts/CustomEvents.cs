/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Holds the EVTData class used in DelegateEvents so that variables can be sent
    - Holds classes that can be sent through the EVTData class 
 ****************************************************/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;


//Class that's sent through all of our custom events as an argument
public class EVTData
{
    public UISoundCueEVT soundCue = null;
    public UIMusicCueEVT musicCue = null;
    public SoundCutoutEVT soundCutout = null;
    public SceneTransitionEVT sceneTransition = null;

    public CameraTrackObjectEVT cameraTrackObject = null;
    public ObjectSelectedEVT objectSelected = null;
    public ObjectDestroyedEVT objectDestroyed = null;
    public MapClickEVT mapClick = null;

    public SetActionButtonEVT setActionButton = null;
}

/****************************************************************** UI EVENTS ******************************************************************/


//Event data used when a UI element needs to play a sound
public class UISoundCueEVT
{
    public AudioClip soundToPlay = null;
    public float soundVolume = 1;
}


//Event data used when a UI element needs to play music
public class UIMusicCueEVT
{
    public AudioClip musicToPlay = null;
    public float musicVolume = 1;
}


//used when the sound cuts out after an impactful sound cue
public class SoundCutoutEVT
{
    //how long the cutout lasts
    public float stopDuration = 0;
    //How long it takes for sounds to return to normal levels again
    public float fadeInDuration = 0;

    //How low the music volume is set when the cutout initially happens
    [Range(0, 1.0f)]
    public float musicLowPoint = 0;
    //How low the dialogue volume is set when the cutout initially happens
    [Range(0, 1.0f)]
    public float dialogueLowPoint = 0;
    //How low the SFX volume is set when the cutout initially happens
    [Range(0, 1.0f)]
    public float sfxLowPoint = 0;
}


//Event data used when we transition to a new scene
public class SceneTransitionEVT
{
    public string newSceneName = "";
    public float transitionTime = 1;
}


//Switches the current camera to a new camera
public class TransitionCameraEVT
{
    //Camera that's currently being used
    public GameObject currentCamera = null;
    //Camera to switch to
    public GameObject nextCamera = null;
    public float transitionTime = 0.1f;
    //True if nextCamera should be stationary, False if nextCamera will move
    public bool transitionToStaticCam = false;
    //Curve to set the ease in/out of the transition cam to the next camera
    public EaseType interpEase = EaseType.Linear;
    //Percent of distance covered each frame (only used for cameras that follow moving objects)
    [Range(0.01f, 1.0f)]
    public float movePercent = 0.1f;
}



//Designates which game object a camera should follow
public class CameraTrackObjectEVT
{
    //The game object this camera will follow
    public GameObject objectToFollow = null;

    public CameraTrackObjectEVT(GameObject objectToFollow_)
    {
        this.objectToFollow = objectToFollow_;
    }
}


/****************************************************************** DEEP SPACE COLONIES EVENTS ******************************************************************/

//Event dispatched to designate which game object the player clicks on
public class ObjectSelectedEVT
{
    //The game object selected
    public GameObject objectSelected = null;

    public ObjectSelectedEVT(GameObject objectSelected_)
    {
        this.objectSelected = objectSelected_;
    }
}


//Event dispatched when an object's health (HealthTracker.CurrentHealth) reaches 0
public class ObjectDestroyedEVT
{
    public GameObject attackingObj = null;
    public GameObject destroyedObj = null;

    public ObjectDestroyedEVT(GameObject attackingObj_, GameObject destroyedObj_)
    {
        this.attackingObj = attackingObj_;
        this.destroyedObj = destroyedObj_;
    }
}


//Event dispatched from WorldClick when the map is right clicked
public class MapClickEVT
{
    //The position in space where the player clicked the map
    public Vector3 clickCoords;

    public MapClickEVT(Vector3 coords_)
    {
        this.clickCoords = coords_;
    }
}


//Event dispatched from Structure class to enable/disable specific action buttons on the HUD
public class SetActionButtonEVT
{
    //String to be used when invoking this event in the Event Manager
    static public string eventName = "ActionButtonPressEVT";
    //Number for the action button that is being set
    public int actionNum = 0;
    //True if the action button being set will be interactable
    public bool interactable = false;
    //Sprite to appear on the action button
    public Sprite buttonIcon = null;

    public SetActionButtonEVT(int actionNum_, bool interactable_, Sprite icon_)
    {
        this.actionNum = actionNum_;
        this.interactable = interactable_;
        this.buttonIcon = icon_;
    }
}