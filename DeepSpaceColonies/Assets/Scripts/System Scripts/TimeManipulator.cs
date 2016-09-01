/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Allows us to pause, unpause, and change the timescale
 ****************************************************/
using UnityEngine;
using System.Collections;

public class TimeManipulator : MonoBehaviour
{
    //Stores the time scale when paused so that it's the same when we unpause
    private float timeScaleAtPause = 1;


    //Function called externally. Pauses the timescale
    public void PauseTime()
    {
        this.timeScaleAtPause = Time.timeScale;
        Time.timeScale = 0;
    }


    //Function called externally. Unpauses the timescale
    public void UnPauseTime()
    {
        Time.timeScale = this.timeScaleAtPause;
    }


    //Function called externally. Changes the timescale to the float given
    public void SetTimeSpeed(float newSpeed_)
    {
        Time.timeScale = newSpeed_;
    }


    //Function called externally. Sets the timescale back to normal
    public void ReturnToDefaultTime()
    {
        Time.timeScale = 1;
    }
}
