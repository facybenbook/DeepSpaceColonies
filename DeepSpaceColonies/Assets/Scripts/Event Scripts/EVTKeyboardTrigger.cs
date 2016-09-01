/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Component that triggers custom UnityEvents when designated keyboard buttons are pressed
 ****************************************************/
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

public class EVTKeyboardTrigger : MonoBehaviour
{
    //List of keyboard keys that will trigger the UnityEvents of the same index
    public List<KeyCode> keysThatTrigger;
    //List of UnityEvents that are triggered by the key of the same index
    public List<UnityEvent> eventsToTrigger;

    
	
	// Update is called once per frame
	private void Update ()
    {
        //If the player is typing, this script won't activate
        if (GlobalData.isTyping)
            return;

        //Checks each key to see if any of them are pressed this frame
        for (int i = 0; i < this.keysThatTrigger.Count; ++i)
        {
            //If a key is pressed, invokes the UnityEvent of the same index
            if (Input.GetKeyDown(this.keysThatTrigger[i]))
            {
                //Makes sure we don't try to go outside the list's index range
                if(i > this.eventsToTrigger.Count)
                    return;

                this.eventsToTrigger[i].Invoke();
            }
        }
    }
}
