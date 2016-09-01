/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Inherits from Ship.cs
    - Low-level combat ship
    - Sets this ship's specific actions
 ****************************************************/
using UnityEngine;
using System.Collections;

public class Fighter : Ship
{
    //Icons for this ship's action buttons
    public Sprite action1Icon;
    public Sprite action2Icon;
    public Sprite cancelIcon;

    //Delegate events for actions this ship can perform
    private DelegateEvent<EVTData> action1EVT;
    private DelegateEvent<EVTData> action2EVT;


    
    /*Function called on Initialization
    Inherits from Ship.cs to connect base delegate events */
    protected override void Awake()
    {
        base.Awake();

        //Initializes new delegate events for the Event Manager
        this.action1EVT = new DelegateEvent<EVTData>(this.AttackTest);
        this.action2EVT = new DelegateEvent<EVTData>(this.MoveTest);
    }


    //Function called when this component is enabled on a game object
    protected override void OnEnable()
    {
        base.OnEnable();
        EventManager.StartListening("Action1Pressed", this.action1EVT);
        EventManager.StartListening("Action2Pressed", this.action2EVT);
    }


    //Function called when this component is disabled on a game object
    protected override void OnDisable()
    {
        base.OnDisable();
        EventManager.StopListening("Action1Pressed", this.action1EVT);
        EventManager.StopListening("Action2Pressed", this.action2EVT);
    }

    
    //Override the empty SetIcons function from Ship.cs to set this fighter's specific icons
    protected override void SetIcons(EVTData data_)
    {
        //Creating a new EVTData structure to set this ship's action buttons
        EVTData setButton = new EVTData();

        //Icon 1
        setButton.setActionButton = new SetActionButtonEVT(1, true, this.action1Icon);
        EventManager.TriggerEvent(SetActionButtonEVT.eventName, setButton);
        //Icon 2
        setButton.setActionButton = new SetActionButtonEVT(2, true, this.action2Icon);
        EventManager.TriggerEvent(SetActionButtonEVT.eventName, setButton);
    }


    //~~~~~~~~ UNFINISHED ~~~~~~~~~
    //Function called from the EventManager.cs using the action1EVT delegate event
    private void AttackTest(EVTData data_)
    {
        if (!MouseData.objectSelected.Contains(this.gameObject))
            return;
    }


    //~~~~~~~~ UNFINISHED ~~~~~~~~~
    //Function called from the EventManager.cs using the action2EVT delegate event
    private void MoveTest(EVTData data_)
    {
        if (!MouseData.objectSelected.Contains(this.gameObject))
            return;
    }
}
