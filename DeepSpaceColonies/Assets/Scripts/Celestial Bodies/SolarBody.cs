/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Parent class for all large space objects (stars, planets, moons, etc)
    - Stores basic properties such as mass, radius, and revolution speed
 ****************************************************/
 using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Inventory))]
public class SolarBody : MonoBehaviour
{
    //The gameobject that represents this script
    public GameObject displayObject;

    //Mass in terms of Solar Mass. 1Sm = 1.988 * 10^30 Kg
    [HideInInspector]
    public float mass = 0;
    //Min and max mass range
    public Vector2 massRange = new Vector2(0,1);

    //Use Cube out to scale toward the low end, Cube in to scale toward the high end, and Cube InOut for a bell curve. Switch to Sine functions for less drastic results
    public EaseType massDistribution = EaseType.CubeOut;

    //Radius in terms of Solar Radius. 1Sr = 695,700 Km
    [HideInInspector]
    public float radius = 0;
    //Min and max radius size
    public Vector2 radiusRange = new Vector2(0,1);

    //Determines how the radius is factored based on what this object's mass is
    public EaseType massRadiusCoorilation = EaseType.Linear;

    //Time to complete a full revolution in seconds. Positive is counter clockwise, negative is clockwise.
    [HideInInspector]
    public float revolutionSpeed = 0;
    //Min and max revolution speeds
    public Vector2 revSpeedRange = new Vector2(0, 1);

    //Use Cube out to scale toward the low end, Cube in to scale toward the high end, and Cube InOut for a bell curve. Switch to Sine functions for less drastic results
    public EaseType revolutionDistribution = EaseType.CubeOut;

    [HideInInspector]
    public bool spinClockwise = true;

    //Delegate for when the player clicks a display object
    private DelegateEvent<EVTData> displayObjectSelectedEVT;

    

    //Function to be called as soon as this object is spawned
    public virtual void GenerateStats(bool spinClockwise_ = true)
    {
        //Sets the direction of the revolution
        this.spinClockwise = spinClockwise_;

        //Temp float used to determine the revolution speed
        float spinValue = 1;

        if (!this.spinClockwise)
            spinValue = -1;

        //Makes this planet rotate with its spin
        this.revolutionSpeed = Random.Range(this.revSpeedRange.x, this.revSpeedRange.y) * spinValue;

        //Creating an interpolator and random value that is used as the baseline for our mass and radius
        float rand = Random.value;
        Interpolator scalar = new Interpolator(this.massDistribution);
        scalar.AddTime(rand);

        //Sets the Mass based on the scalar's distribution
        this.mass = Mathf.Round( (scalar.GetProgress() * (this.massRange.y - this.massRange.x) + this.massRange.x) * 100) / 100;

        //Sets the mass based on the same random value as the mass
        scalar.ease = this.massRadiusCoorilation;
        this.radius = Mathf.Round( (scalar.GetProgress() * (this.radiusRange.y - this.radiusRange.x) + this.radiusRange.x) * 100 ) / 100;

        //Creates a new random value for the scalar to determine the spin
        rand = Random.value;
        scalar.ResetTime();
        scalar.AddTime(rand);

        this.revolutionSpeed = scalar.GetProgress() * (this.revSpeedRange.y - this.revSpeedRange.x) + this.revSpeedRange.x;
    }


    //Function called every frame. Using FIXED Update because it handles different time scales better than regular Update
    protected virtual void FixedUpdate()
    {
        //Temp float used to determine the revolution speed
        float spinValue = 1;

        if (!this.spinClockwise)
            spinValue = -1;

        //Revolves along the axis
        this.displayObject.transform.Rotate(0, 0, this.revolutionSpeed * spinValue);
    }

    
    //Function called on initialization
    private void Awake()
    {
        //Creates a new DelegateEvent for DisplayObjectSelect event for the Event Manager
        this.displayObjectSelectedEVT = new DelegateEvent<EVTData>(this.DisplayObjectSelect);
    }


    //Function called when this component is enabled on a game object
    private void OnEnable()
    {
        //Makes this component start listening for the DisplayObjectSelect event
        EventManager.StartListening("DisplayObjectSelected", this.displayObjectSelectedEVT);
    }


    //Function called when this component is disabled on a game object
    private void OnDisable()
    {
        //Makes this component stop listening for the DisplayObjectSelect event
        EventManager.StopListening("DisplayObjectSelected", this.displayObjectSelectedEVT);
    }


    //Function called when the DisplayObjectSelect event is triggered
    private void DisplayObjectSelect(EVTData data_)
    {
        //Checks to see if this script's display object was the one selected
        if (data_.objectSelected.objectSelected != this.displayObject)
            return;
    }
}
