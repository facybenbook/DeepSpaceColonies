/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Base class for all Star objects
    - Inherits from SolarBody.cs
    - Generates the star Temperature, Luminosity, Color, and Scale of the game object
 ****************************************************/
using UnityEngine;
using System.Collections;

public class Star : SolarBody
{
    //Enum that designates which type of star this is
    public StarTypes type = StarTypes.MainSequence;

    //The temperature (in Kelvin) of this star's surface. Randomly generated using TempRange
    [HideInInspector]
    public int surfaceTemp = 0;

    //Range of temperatue that this star can have. X is minimum, Y is maximum
    public Vector2 tempRange = new Vector2(0, 1);

    /* Enum that designates the scalar for how temperature is distributed
    Use Cube out to scale toward the low end, Cube in to scale toward the high end, and Cube InOut for a bell curve. Switch to Sine functions for less drastic results*/
    public EaseType temperatureDistribution = EaseType.CubeInOut;

    //Range of light emitted by this star
    [HideInInspector]
    public float luminosity = 0;

    //Colors that the star's texture can be between. Uses the same scalar as Temperature
    public Gradient starColors;

    /*Setting the min, middle, and maximum ranges that star game object can have their scale set to.
    This makes sure that the game object's size doesn't 100% match realistic proportions, because some stars will be WAY too big or small */
    public Vector3 starScaleRange = new Vector3(15, 50, 100);

    //The min, middle, and maximum ranges of star radii that determines the scale
    public Vector3 starRadiusRange = new Vector3(0.5f, 1, 150);

    //Scalars that determine how the star's scale is distributed between the min and middle, and between the middle and max ranges
    public EaseType lowMidCoorilation = EaseType.CubeIn;
    public EaseType midMaxCoorilation = EaseType.CubeOut;



    /* Function called from SolarSystem.cs (GenerateStar function)
    Override of the SolarBody parent class' GenerateStats function since stars will have their own unique stats*/
    public override void GenerateStats(bool spinClockwise_ = true)
    {
        //Calls the GenerateStats base function to set the spin direction
        base.GenerateStats(spinClockwise_);

        //Generating a random value between 0 and 1 to use in our scalar
        float rand = Random.value;

        //The new scalar uses the curve designated with TemperatureDistribution and has a time length of 1
        Interpolator scaler = new Interpolator(this.temperatureDistribution, 1);
        scaler.AddTime(rand);

        //Finds the surface temperature based on the temperature distribution
        this.surfaceTemp = Mathf.RoundToInt( (scaler.GetProgress() * (this.tempRange.y - this.tempRange.x) + this.tempRange.x) / 100) * 100;

        /*Finds the luminosity based off the area, the temperature to the 4th power, and the Stefan-Bolzmann constant
        NOTE: the radius used is a float variable inherited from the SolarBody parent class that's generated in base.GenerateStats */
        float area = (4 * Mathf.PI * (this.radius * this.radius));
        float SBConstant = 5.670367f * Mathf.Pow(10f, -8);
        float tempQuad = Mathf.Pow(this.surfaceTemp, 4);
        this.luminosity = area * SBConstant * tempQuad;

        //Determines the display object's color based on the temperature distribution. Uses the Mesh Renderer if there's no Sprite Renderer component
        if (this.displayObject.GetComponent<SpriteRenderer>() != null)
        {
            this.displayObject.GetComponent<SpriteRenderer>().color = this.starColors.Evaluate(scaler.GetProgress());
        }
        else if(this.displayObject.GetComponent<MeshRenderer>() != null)
        {
            this.displayObject.GetComponent<MeshRenderer>().materials[0].color = this.starColors.Evaluate(scaler.GetProgress());
        }
        

        //If this star is at or below the smallest radius, it's game object is set to the smallest scale in the StarScaleRange
        if (this.radius <= this.starRadiusRange.x)
        {
            this.displayObject.transform.localScale = new Vector3(this.starScaleRange.x, this.starScaleRange.x, this.starScaleRange.x);
        }
        //If it's between the smallest and middle radius, finds the scale to set it at based on the coorilation
        else if (this.radius > this.starRadiusRange.x && this.radius <= this.starRadiusRange.y)
        {
            Interpolator scalar = new Interpolator(this.lowMidCoorilation);
            float percent = (this.radius - this.starRadiusRange.x) / (this.starRadiusRange.y - this.starRadiusRange.x);
            scalar.AddTime(percent);

            float newScale = scalar.GetProgress() * (this.starScaleRange.y - this.starScaleRange.x) + this.starScaleRange.x;

            this.displayObject.transform.localScale = new Vector3(newScale, newScale, newScale);
        }
        //If it's between the middle radius and the max radius, we find the scale to set it at based on the coorilation
        else if (this.radius > this.starRadiusRange.y && this.radius <= this.starRadiusRange.z)
        {
            Interpolator scalar = new Interpolator(this.midMaxCoorilation);
            float percent = (this.radius - this.starRadiusRange.y) / (this.starRadiusRange.z - this.starRadiusRange.y);
            scalar.AddTime(percent);

            float newScale = scalar.GetProgress() * (this.starScaleRange.z - this.starScaleRange.y) + this.starScaleRange.y;

            this.displayObject.transform.localScale = new Vector3(newScale, newScale, newScale);
        }
        //Otherwise the radius is at or above the max radius, so it's set to the largest scale in the StarScaleRange
        else
        {
            this.displayObject.transform.localScale = new Vector3(this.starScaleRange.z, this.starScaleRange.z, this.starScaleRange.z);
        }
    }


    //Override of the SolarBody parent class' FixedUpdate function
    protected override void FixedUpdate()
    {
        //Calls the base function from SolarBody parent script to revolve along an axis
        base.FixedUpdate();
    }
}


//Enum for the different types of stars in Star.cs
public enum StarTypes
{
    MainSequence,

    RedGiant,
    RedSupergiant,

    YellowSupergiant,

    BlueGiant,
    BlueSupergiant,

    RedDwarf,
    BlueDwarf,
    WhiteDwarf,
    BlackDwarf,

    Neutron,
    BlackHole
}