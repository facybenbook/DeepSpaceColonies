/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Component to be attached to the UI object
    - Displays a description of a clicked moon on the HUD
 ****************************************************/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class HUD_ShowMoonData : MonoBehaviour
{
    //The UnityEvent triggered when a moon is selected
    public UnityEvent onSelectEvent;
    //Reference to the text field that displays the moon's name
    public InputField moonName;
    //Reference to the text field that displays the moon's mass
    public Text mass;
    //Reference to the text field that displays the moon's radius
    public Text radius;

    //Delegate for when the player clicks a display object
    private DelegateEvent<EVTData> displayObjectSelectedEVT;



    // Function called on initialization
    private void Awake()
    {
        this.displayObjectSelectedEVT = new DelegateEvent<EVTData>(this.DisplayObjectSelect);
    }


    //Function called when this component is enabled on a game object
    private void OnEnable()
    {
        EventManager.StartListening("DisplayObjectSelected", this.displayObjectSelectedEVT);
    }


    //Function called when this component is disabled on a game object
    private void OnDisable()
    {
        EventManager.StopListening("DisplayObjectSelected", this.displayObjectSelectedEVT);
    }


    /* Function called from the EventManager.cs using the displayObjectSelectedEVT delegate event
    Updates the HUD to show the selected moon's data */ 
    private void DisplayObjectSelect(EVTData data_)
    {
        //Checks to make sure the object selected was a moon, or nothing happens
        if (data_.objectSelected.objectSelected == null || data_.objectSelected.objectSelected.transform.parent == null
            || data_.objectSelected.objectSelected.transform.parent.GetComponent<Moon>() == null || MouseData.isMouseDisabled)
            return;

        Moon SelectedMoon = data_.objectSelected.objectSelected.transform.parent.GetComponent<Moon>();

        //Turns on the game object
        this.onSelectEvent.Invoke();

        //Displays the selected moon's data in the text fields on the HUD
        this.moonName.text = SelectedMoon.name;
        GlobalData.isTyping = false;

        this.mass.text = ("Mass: " + SelectedMoon.mass + " Earth Masses");
        this.radius.text = ("Radius: " + SelectedMoon.radius + " Earth Radius");
    }
}
