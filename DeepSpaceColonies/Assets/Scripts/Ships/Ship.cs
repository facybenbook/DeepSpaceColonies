/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Parent class for all ship classes
    - Stores basic properties such as movement speed, damage, and structures required
    - Handles ship movement and basic attacking
 ****************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;//For Lists

[RequireComponent(typeof(SendEVTOnClick))]
[RequireComponent(typeof( HealthTracker))]
[RequireComponent(typeof(Inventory))]
public class Ship : MonoBehaviour
{
    //The icon shown on the HUD when this ship is selected
    public Sprite hudIcon;

    //How fast this ship traverses a system
    public float speed = 0;

    //How fast this ship can travel between systems
    public float intergalacticSpeed = 0;

    //The distance that this ship stops at to prevent running into objects
    public float movementStopRange = 10;

    //If this ship is weaponless, it cannot attack at all
    public bool weaponless = false;

    //The minimum and maximum damage that this ship can deal
    public Vector2 damageMinMax = new Vector2(1,5);

    //How far away this ship can be from the target it's attacking
    public float attackRange = 10;

    //The amount of time in seconds between attacks
    public float attackDelay = 1;
    private float currentAttackTimer = 0;

    //Reference to the game object that this ship is currently moving toward
    private GameObject targetObj = null;
    //If this ship will attack the targetObj
    private bool hostile = false;
    //Position where this ship is moving toward
    private Vector3 movePos = new Vector3(1,1,1);

    //Delegate events that this ship is listening for
    private DelegateEvent<EVTData> selectedEVT;
    private DelegateEvent<EVTData> attackTargetEVT;
    private DelegateEvent<EVTData> moveToTargetEVT;
    private DelegateEvent<EVTData> stopActionsEVT;
    private DelegateEvent<EVTData> objectDestroyedEVT;
    private DelegateEvent<EVTData> mapClickEVT;



    //Function called on initialization
    virtual protected void Awake()
    {
        //Initializes new DelegateEvents for the Event Manager
        this.selectedEVT = new DelegateEvent<EVTData>(this.SetIcons);
        this.attackTargetEVT = new DelegateEvent<EVTData>(this.AttackTarget);
        this.moveToTargetEVT = new DelegateEvent<EVTData>(this.MoveToTarget);
        this.stopActionsEVT = new DelegateEvent<EVTData>(this.ClearTarget);
        this.objectDestroyedEVT = new DelegateEvent<EVTData>(this.Destroyed);
        this.mapClickEVT = new DelegateEvent<EVTData>(this.MoveToTarget);

        GlobalData.GlobalDataRef.GetComponent<BoxSelection>().AddUnitForSelection(this.transform);
    }


    //Function called when this component is enabled on a game object
    virtual protected void OnEnable()
    {
        EventManager.StartListening("DisplayObjectSelected", this.selectedEVT);
        EventManager.StartListening("AttackObjectSelected", this.attackTargetEVT);
        EventManager.StartListening("MoveToObjectSelected", this.moveToTargetEVT);
        EventManager.StartListening("SelectedObjStopActions", this.stopActionsEVT);
        EventManager.StartListening("ObjectWasDestroyed", this.objectDestroyedEVT);
        EventManager.StartListening("MapClick", this.mapClickEVT);
    }


    //Function called when this component is disabled on a game object
    virtual protected void OnDisable()
    {
        EventManager.StopListening("DisplayObjectSelected", this.selectedEVT);
        EventManager.StopListening("AttackObjectSelected", this.attackTargetEVT);
        EventManager.StopListening("MoveToObjectSelected", this.moveToTargetEVT);
        EventManager.StopListening("SelectedObjStopActions", this.stopActionsEVT);
        EventManager.StopListening("MapClick", this.mapClickEVT);
    }


    /*Function called from the EventManager.cs using the attackTargetEVT delegate event
    Determines if this ship is allowed to attack a target and sets hostility */
    private void AttackTarget(EVTData data_)
    {
        //Does nothing if this ship isn't player controlled. Can't have the player control enemy ships now can we?
        if (GetComponent<HealthTracker>().reputationStanding != Reputation.PlayerControlled)
            return;

        //Does nothing if this object is not selected
        if (!MouseData.objectSelected.Contains(this.gameObject))
            return;

        this.targetObj = data_.objectSelected.objectSelected;

        //Can't attack object's unless certain parameters are met
        if (this.targetObj.GetComponent<HealthTracker>() != null && !this.weaponless)
        {
            Reputation thisShip = this.gameObject.GetComponent<HealthTracker>().reputationStanding;
            Reputation otherShip = this.targetObj.GetComponent<HealthTracker>().reputationStanding;
            
            //If this ship is non-hostile and non-chaotic, it won't attack other non-hostile/non-chaotic ships
            if ((thisShip != Reputation.Hostile && thisShip != Reputation.Chaotic) &&
                (otherShip != Reputation.Hostile && otherShip != Reputation.Chaotic))
            {
                this.hostile = false;
            }
            //If this ship is hostile, it won't attack other hostile ships
            else if (thisShip == Reputation.Hostile && otherShip == Reputation.Hostile)
            {
                this.hostile = false;
            }
            //If this ship is chaotic, it won't attack other chaotic ships
            else if (thisShip == Reputation.Chaotic && otherShip == Reputation.Chaotic)
            {
                this.hostile = false;
            }
            //Otherwise the ships are enemies and will attack
            else
            {
                this.hostile = true;
            }
        }
        else
        {
            this.hostile = false;
        }

        this.movePos = new Vector3(1, 1, 1);
    }


    /*Function called from the EventManager.cs using the moveToTargetEVT delegate event
    Sets the target position for this ship to move to */
    private void MoveToTarget(EVTData data_)
    {
        //Does nothing if this object is not selected
        if (!MouseData.objectSelected.Contains(gameObject))
            return;
        
        this.movePos = new Vector3(data_.mapClick.clickCoords.x, 0, data_.mapClick.clickCoords.z);
    }


    /*Function called from the EventManager.cs using the stopActionsEVT delegate event
    Clears this ship's hostility, target, and location to move toward */
    public void ClearTarget(EVTData data_)
    {
        //Does nothing if this object is not selected
        if (!MouseData.objectSelected.Contains(gameObject))
            return;

        this.targetObj = null;
        this.hostile = false;
        this.movePos = new Vector3(1, 1, 1);
    }


    //Function called every frame. Using FIXED Update because it handles different time scales better than regular Update
    private void FixedUpdate()
    {
        //If this ship has a target object, moves toward it
        if (this.targetObj != null)
        {
            this.MoveToTarget();
        }
        //Otherwise, if this ship has a target location, moves toward it
        else if(this.movePos.y == 0)
        {
            this.MoveToLocation();
        }
    }


    //Function called from FixedUpdate to move toward the target game object
    private void MoveToTarget()
    {
        //Rotates to face the target
        float xDiff = this.targetObj.transform.position.x - this.transform.position.x;
        float zDiff = this.targetObj.transform.position.z - this.transform.position.z;
        float newRot = Mathf.Atan2(zDiff, xDiff) * Mathf.Rad2Deg;

        //If the ship hostile and is within attack range of the target
        if (this.hostile && Vector3.Distance(this.targetObj.transform.localPosition, this.transform.localPosition) <= this.attackRange)
        {
            //Attacks the target if the attack cooldown is finished
            if (this.currentAttackTimer <= 0)
            {
                this.Fire();
                this.currentAttackTimer = this.attackDelay;
            }
            //Counts down the attack cooldown
            else
            {
                this.currentAttackTimer -= Time.fixedDeltaTime;
                this.transform.LookAt(this.targetObj.transform.position);
            }
        }
        //Otherwise it keeps moving toward the target
        else if (Vector3.Distance(this.targetObj.transform.localPosition, this.transform.localPosition) > this.movementStopRange)
        {
            //Translates this ship to move toward the target
            float xSpeed = this.speed * Mathf.Cos(newRot);
            float zSpeed = this.speed * Mathf.Sin(newRot);
            this.transform.localPosition += new Vector3(xSpeed, 0, zSpeed);
            this.transform.localEulerAngles = new Vector3(0, Mathf.Atan2(xSpeed, zSpeed) * Mathf.Rad2Deg, 0);

            //Stops moving if the target is within the stop range (to avoid model collision)
            if (Vector3.Distance(this.targetObj.transform.position, this.transform.position) <= this.movementStopRange)
            {
                this.targetObj = null;
            }
        }
    }


    //Function called from FixedUpdate to move toward a location in space
    private void MoveToLocation()
    {
        //Rotates to face the target
        float xDiff = this.movePos.x - this.transform.position.x;
        float zDiff = this.movePos.z - this.transform.position.z;
        float newRot = Mathf.Atan2(zDiff, xDiff) * Mathf.Rad2Deg;

        //Moves this object in the direction it's facing
        float xSpeed = this.speed * Mathf.Cos(newRot);
        float zSpeed = this.speed * Mathf.Sin(newRot);
        this.transform.localPosition += new Vector3(xSpeed, 0, zSpeed);
        this.transform.localEulerAngles = new Vector3(0, Mathf.Atan2(xSpeed, zSpeed) * Mathf.Rad2Deg, 0);

        //Stops moving if the target position is within the stop range
        if (Vector3.Distance(this.movePos, this.transform.position) <= this.movementStopRange)
        {
            this.movePos = new Vector3(0,1,0);
        }
    }


    //Function called from MoveToTarget to attack the target this ship is hostile toward
    public void Fire()
    {
        //Inflicts random damage to the target based on the min and max damage values
        int damage = Mathf.RoundToInt( Random.Range(this.damageMinMax.x, this.damageMinMax.y) );
        this.targetObj.GetComponent<HealthTracker>().Damage(damage, this.gameObject);
    }


    //Function called from the EventManager.cs using the objectDestroyedEVT delegate event
    public void Destroyed(EVTData data_)
    {
        //If the object destroyed was this ship's target, the target is cleared and this ship becomes non-hostile
        if(data_.objectDestroyed.destroyedObj == this.targetObj)
        {
            this.targetObj = null;
            this.hostile = false;
        }

        //Does nothing if the object destroyed wasn't this object
        if (data_.objectDestroyed.destroyedObj != this.gameObject)
            return;
    }


    //~~~~~~~~ UNFINISHED ~~~~~~~~~
    //Function called from the EventManager.cs using the selectedEVT delegate event
    protected virtual void SetIcons(EVTData data_)
    {
        Debug.Log("Ship test set icon");
    }
    
}
