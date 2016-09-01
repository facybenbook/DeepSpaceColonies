/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Component that counts down a timer and invokes a custom UnityEvent when it runs out
 ****************************************************/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class HUD_Timer : MonoBehaviour
{
    //How many minutes this timer starts with
    public int startingMinutes = 60;
    //How many seconds this timer starts with
    public float startingSeconds = 0;
    //Reference to the text that displays this timer
    public Text displayText;
    //UnityEvent triggered when the timer is up
    public UnityEvent timeUpEvent;

    
	
	//Function called every frame. Using FIXED Update because it handles different time scales better than regular Update
	private void FixedUpdate ()
    {
        //Reduces the current time
        this.startingSeconds -= Time.deltaTime;
        
        if(this.startingSeconds < 0)
        {
            //If there are minutes remaining, restarts the seconds and subtracts a minute
            if(this.startingMinutes > 0)
            {
                this.startingMinutes -= 1;
                this.startingSeconds += 60;
            }
            //If no minutes are remaining, the timer ends, the timeUpEvent is triggered, and this component is disabled
            else
            {
                this.timeUpEvent.Invoke();

                if(this.displayText != null)
                {
                    this.displayText.text = "00:00";
                }

                enabled = false;
            }
        }

        //If the displayText text field isn't null, displays the time remaining
        if(this.displayText != null)
        {
            string min = "";
            string sec = "";

            if(this.startingMinutes < 10)
            {
                min = ("0" + this.startingMinutes);
            }
            else
            {
                min = ("" + this.startingMinutes);
            }

            if (this.startingSeconds < 10)
            {
                sec = ("0" + Mathf.RoundToInt(this.startingSeconds));
            }
            else
            {
                sec = ("" + Mathf.RoundToInt(this.startingSeconds));
            }

            this.displayText.text = (min + ":" + sec );
        }
	}
}
