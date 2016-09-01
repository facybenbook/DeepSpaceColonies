/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Handles logic for action buttons so that they are enabled/disabled correctly for each
    interactable object, and changes the icons to match the button actions.
 ****************************************************/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class ActionButtonLogic : MonoBehaviour
{
    //The number ID of this action (number on the button)
    public int actionNum = 0;
    //Quick reference to the Button script on the same game object as this script
    private Button thisButton;
    //Reference to the Image component on the game object that displays the button's image
    public Image buttonIcon;
    //The sprite used when this button is disabled
    public Sprite disabledIcon;

    //Delegate event to set this button's 
    private DelegateEvent<EVTData> setActionEVT;



    // Use this for initialization
    private void Awake ()
    {
        //Initializes a new DelegateEvent for the Event Manager
        this.setActionEVT = new DelegateEvent<EVTData>(this.SetAction);
        //Sets a quick reference to this game object's button component
        this.thisButton = GetComponent<Button>();
        //Disables the icon game object by default
        this.buttonIcon.enabled = false;
	}


    //Function called when this component is enabled on a game object
    private void OnEnable()
    {
        EventManager.StartListening(SetActionButtonEVT.eventName, this.setActionEVT);
    }


    //Function called when this component is disabled on a game object
    private void OnDisable()
    {
        EventManager.StopListening(SetActionButtonEVT.eventName, this.setActionEVT);
    }


    /* Function called from the EventManager.cs using the setActionEVT delegate event
    Sets this button's sprite and if the player can interact with it */
    private void SetAction(EVTData data_)
    {
        //Does nothing if this isn't the action button being set
        if (data_.setActionButton.actionNum != this.actionNum)
            return;

        //If this action button is being set, enables or disables it based on the EVTData
        this.thisButton.interactable = data_.setActionButton.interactable;

        //If this button's icon isn't null, enables the icon and sets its sprite
        if (data_.setActionButton.buttonIcon != null)
        {
            this.buttonIcon.enabled = true;
            this.buttonIcon.sprite = data_.setActionButton.buttonIcon;
        }
        //If it is null, disables the icon
        else
        {
            this.ClearIcon();
        }
    }


    //Function called externally. Clears disables this button's icon
    public void ClearIcon()
    {
        this.buttonIcon.enabled = false;
        this.buttonIcon.sprite = this.disabledIcon;
    }
}
