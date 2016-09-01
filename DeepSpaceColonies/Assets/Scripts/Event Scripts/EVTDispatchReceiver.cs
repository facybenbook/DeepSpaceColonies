/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Component that receives empty Delegate Events to perform custom UnityEvents
 ****************************************************/
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class EVTDispatchReceiver : MonoBehaviour
{
    //The name of the event that triggers the eventOnReceive UnityEvent
    public string eventNameToListenFor;
    //List of Delegate Events that can be dispatched using UnityEvents
    public List<string> eventNamesToDispatch;
    //UnityEvent that is triggered when eventNameToListenFor is received
    public UnityEvent eventOnReceive;
    //Delegate Event that this component is listening for
    private DelegateEvent<EVTData> customListener;


    // Use this for initialization
    private void Awake()
    {
        this.customListener = new DelegateEvent<EVTData>(this.EventTriggered);
    }


    //Function called when this component is enabled on a game object
    private void OnEnable()
    {
        EventManager.StartListening(this.eventNameToListenFor, this.customListener);
    }


    //Function called when this component is disabled ona game object
    private void OnDisable()
    {
         EventManager.StopListening(this.eventNameToListenFor, this.customListener);
    }


    /*Function called from the EventManager.cs using the customListener delegate event
    Invokes the eventOnReceive UnityEvent */ 
    public void EventTriggered(EVTData data_)
    {
        this.eventOnReceive.Invoke();
    }


    //Function called externally. Calls an empty event using the index of the event name to dispatch
    public void DispatchEvent(int index_)
    {
        //Makes sure we don't try to go outside the list's index range
        if (index_ > this.eventNamesToDispatch.Count)
            return;

        EventManager.TriggerEvent(this.eventNamesToDispatch[index_]);
    }
}