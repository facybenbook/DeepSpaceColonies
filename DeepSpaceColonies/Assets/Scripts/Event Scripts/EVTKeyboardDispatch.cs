/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Component that dispatches custom Delegate Events when designated keyboard buttons are pressed
 ****************************************************/
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EVTKeyboardDispatch : MonoBehaviour
{
    //List of Keyboard keys that will dispatch the event of the same list index
    public List<KeyCode> keysThatDispatchEvents;
    //List of event names that are dispatched by the key of the same list index
    public List<string> eventNamesToDispatch;


    //Checks if any of the keys are pressed on this frame
    private void Update()
    {
        //If the player is typing, this script won't activate
        if (GlobalData.isTyping)
            return;

        //Checks each key to see if any of them are pressed this frame. If so, dispatches its event
        for(int i = 0; i < this.keysThatDispatchEvents.Count; ++i)
        {
            if(Input.GetKeyDown(this.keysThatDispatchEvents[i]))
            {
                this.DispatchEvent(i);
            }
        }
    }


    //Function called from Update. Calls an empty event using the event name to dispatch
    private void DispatchEvent(int index_)
    {
        //Makes sure we don't try to go outside the list's index range
        if (index_ > this.eventNamesToDispatch.Count)
            return;

        //Dispatches the event through the EventManager
        EventManager.TriggerEvent(this.eventNamesToDispatch[index_]);
    }
}
