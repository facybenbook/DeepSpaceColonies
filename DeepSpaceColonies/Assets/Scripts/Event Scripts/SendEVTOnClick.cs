/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - NOTE: This component requires a collider of some kind
    - Component that should be on all selectable game objects
    - Dispatches a Delegate Event when clicked so that the clicked object's data can be shown
 ****************************************************/
using UnityEngine;
using System.Collections;

public class SendEVTOnClick : MonoBehaviour
{
    //Function called when the mouse cursor is over this object
    private void OnMouseOver()
    {
        //Does nothing if the player mouse input is disabled
        if (MouseData.isMouseDisabled)
            return;

        //If left-clicked this frame, dispatches a delegate event with this object's data
        if(Input.GetMouseButtonDown(0))
        {
            EVTData clickData = new EVTData();
            clickData.objectSelected = new ObjectSelectedEVT(this.gameObject);

            //If left shift is held, this object is added to the current selection
            if (Input.GetKey(KeyCode.LeftShift))
            {
                EventManager.TriggerEvent("MultiObjectSelect", clickData);
            }
            //Otherwise, this is the only object currently selected
            else
            {
                EventManager.TriggerEvent("DisplayObjectSelected", clickData);
            }
        }
        //If right-clicked this frame, sets this object as the attack target of the current selection
        else if(Input.GetMouseButtonDown(1))
        {
            EVTData clickData = new EVTData();
            clickData.objectSelected = new ObjectSelectedEVT(this.gameObject);
            
            EventManager.TriggerEvent("AttackObjectSelected", clickData);
        }
    }
}
