/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Holds references to all game objects that the player is currently selecting
    - Holds functions to be called through UnityEvents to follow/stop following objects, and stop actions
 ****************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MouseData : MonoBehaviour
{
    //Bool that determines if the player can move/click the mouse
    public static bool isMouseDisabled = false;

    //List of game objects that are currently selected
    public static List<GameObject> objectSelected = null;

    //Delegate events that this script is listening for
    private DelegateEvent<EVTData> trackObjectEVT;
    private DelegateEvent<EVTData> multiTrackEVT;
    private DelegateEvent<EVTData> clearSelectedEVT;



    //Function called on Initialization
    private void Awake()
    {
        objectSelected = new List<GameObject>();

        //Initializes new DelegateEvents for the Event Manager
        this.trackObjectEVT = new DelegateEvent<EVTData>(this.TrackSelectedObj);
        this.multiTrackEVT = new DelegateEvent<EVTData>(this.TrackMultipleObj);
        this.clearSelectedEVT = new DelegateEvent<EVTData>(this.ClearSelected);
    }


    //Function called when this component is enabled on a game object
    private void OnEnable()
    {
        EventManager.StartListening("DisplayObjectSelected", this.trackObjectEVT);
        EventManager.StartListening("MultiObjectSelect", this.multiTrackEVT);
        EventManager.StartListening("ClearSelected", this.clearSelectedEVT);
    }


    //Function called when this component is disabled on a game object
    private void OnDisable()
    {
        EventManager.StopListening("DisplayObjectSelected", this.trackObjectEVT);
        EventManager.StopListening("MultiObjectSelect", this.multiTrackEVT);
        EventManager.StopListening("ClearSelected", this.clearSelectedEVT);
    }
    

    /* Function called from the EventManager.cs using the trackObjectEVT delegate event
    Selects a single object to track*/
    private void TrackSelectedObj(EVTData data_)
    {
        //Clears the current selection of game objects
        this.ClearSelected();
        //Adds the newly selected objects to the selection list
        objectSelected.Add( data_.objectSelected.objectSelected);
    }


    /* Function called from the EventManager.cs using the multiTrackEVT delegate event
    Adds a gameobject to the list of ObjectSelected*/
    private void TrackMultipleObj(EVTData data_)
    {
        //Makes sure that the only objects that can be multi-selected are ships
        if(data_.objectSelected.objectSelected.GetComponent<Ship>() == null || objectSelected.Count >= 12)
            return;
        
        //If the first object selected wasn't a ship, removes it from the selection before adding the new ship
        if(objectSelected.Count > 0 && objectSelected[0].GetComponent<Ship>() == null)
        {
            this.ClearSelected();
        }

        //If the new object wasn't already selected, adds it to the selection list
        if(!objectSelected.Contains(data_.objectSelected.objectSelected))
        {
            objectSelected.Add(data_.objectSelected.objectSelected);
        }
        //If the object was selected, deselects it
        else
        {
            objectSelected.Remove(data_.objectSelected.objectSelected);
        }
    }
    

    /* Function called from the EventManager.cs using the clearSelectedEVT delegate event
    Function called externally. Stops tracking all objects that are currently being tracked*/
    public void ClearSelected(EVTData data_ = null)
    {
        EventManager.TriggerEvent("ClearIcons");
        objectSelected.Clear();
    }


    //Function called externally. Tells the player camera to interpolate to the first object in the selection
    public void CameraFollowThisObj()
    {
        //Makes sure there's an object to follow before sending the event
        if (objectSelected.Count < 1)
            return;

        //Dispatches an event to tell the current camera to follow the first object in the selection
        EVTData trackevt = new EVTData();
        trackevt.cameraTrackObject = new CameraTrackObjectEVT(objectSelected[0]);
        EventManager.TriggerEvent("CameraTrackSelected", trackevt);
    }


    //Function called externally. Tells the player camera to stop interpolating to follow an object
    public void CameraStopFollowingThisObj()
    {
        EVTData trackevt = new EVTData();
        trackevt.cameraTrackObject = new CameraTrackObjectEVT(null);
        EventManager.TriggerEvent("CameraTrackSelected", trackevt);
    }


    //Function called externally. Renames the selected object to the string given. Only works if there's 1 object selected
    public void RenameSelectedObj(string newName_)
    {
        //We can't rename anything unless there's only one object selected
        if (objectSelected.Count != 1)
            return;

        //Renaming the solar system
        if (objectSelected[0].transform.parent == null || objectSelected[0].transform.parent.GetComponent<SolarSystem>() != null)
        {
            objectSelected[0].name = newName_;
        }
        //Renaming a star
        else if (objectSelected[0].transform.parent.GetComponent<Star>() != null)
        {
            objectSelected[0].transform.parent.name = newName_;
        }
        //Renaming a planet
        else if (objectSelected[0].transform.parent.GetComponent<Planet>() != null)
        {
            objectSelected[0].transform.parent.name = newName_;
        }
        //Renaming a moon
        else if (objectSelected[0].transform.parent.GetComponent<Moon>() != null)
        {
            objectSelected[0].transform.parent.name = newName_;
        }

        //Stops typing so that keyboard shortcuts will work again
        GlobalData.isTyping = false;
    }


    //Function called externally. Tells the selected objects to stop all actions
    public void SelectedObjStopActions()
    {
        EventManager.TriggerEvent("SelectedObjStopActions");
    }
}
