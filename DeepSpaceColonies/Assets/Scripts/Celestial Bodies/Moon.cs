/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Base class for all Moon objects
    - Inherits from SolarBody.cs
    - Generates the moon's color, orbit, and what resources it contains
 ****************************************************/
using UnityEngine;
using System.Collections;

public class Moon : SolarBody
{
    //The width, length, and height of the orbit. Make sure that X and Y are within 5% of each other, and Z is relatively small
    [HideInInspector]
    public Vector3 orbitDimensions;

    //Rotation of the orbit to make it look unique
    [HideInInspector]
    public float orbitRotation = 0;

    //How long it takes to complete a full orbit in in-game seconds. Is positive if it spins counter clockwise, negative if clockwise. Determined by the system
    [HideInInspector]
    public float orbitTime = 1;
    //Counter for the current progress of the orbit
    private float currentOrbitTime = 0;

    //Gradient for all colors this moon can be
    public Gradient surfaceColors;



    /*Function called from Planet.cs (GenerateMoons function)
    Override of the SolarBody parent class' GenerateStats function since stars will have their own unique stats */
    public void GenerateStats(Vector3 orbitDimensions_, float orbitTime_ = 1, bool spinClockwise_ = true)
    {
        //Calls the GenerateStats base function to set the spin direction
        base.GenerateStats(spinClockwise_);

        //Setting the XYZ dimensions of the orbit
        this.orbitDimensions = orbitDimensions_;

        //Setting the length of time of the orbit in seconds
        this.orbitTime = orbitTime_;

        //Rotates the Orbit Center for a more random look
        this.transform.Rotate(0, Random.Range(0, 360), 0);

        //Sets the displayed gameobject to the crest of it's rotation
        this.displayObject.transform.localPosition = new Vector3(this.orbitDimensions.x, this.orbitDimensions.y, this.orbitDimensions.z);

        //Sets the trail renderer data of the display object if it has the component
        if (this.displayObject.GetComponent<TrailRenderer>() != null)
        {
            this.displayObject.GetComponent<TrailRenderer>().time = 0.5f * this.orbitTime;
        }

        //Sets this moon's mesh material to a random color from surfaceColors gradient
        this.displayObject.GetComponent<MeshRenderer>().materials[0].color = this.surfaceColors.Evaluate(Random.value);

        //Fills this moon with resources for the player to gather
        this.GenerateResources();
    }


    //~~~~~~~~ UNFINISHED ~~~~~~~~~
    //Called from GenerateStats. Creates a random amount of resources to fill this moon's Inventory component with
    private void GenerateResources()
    {

    }


    //Override of the SolarBody parent class' FixedUpdate function
    protected override void FixedUpdate()
    {
        //Calls the base function from SolarBody parent script to revolve along an axis
        base.FixedUpdate();

        //Increases the current timer
        this.currentOrbitTime += Time.deltaTime;

        //Prevents the current counter from exceeding the max time
        if (this.currentOrbitTime > this.orbitTime)
        {
            this.currentOrbitTime -= this.orbitTime;
        }

        //calculates the angle 
        var angle = (this.currentOrbitTime / this.orbitTime) * 2 * Mathf.PI;

        //Temp float used to determine the revolution speed
        float spinValue = 1;

        if (!this.spinClockwise)
            spinValue = -1;

        //Sets the position of the display object based on the angle
        this.displayObject.transform.localPosition = new Vector3(Mathf.Cos(angle) * this.orbitDimensions.x * spinValue,
                                                            0,
                                                            Mathf.Sin(angle) * this.orbitDimensions.z);
    }
}
