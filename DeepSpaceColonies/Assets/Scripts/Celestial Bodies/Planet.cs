/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Base class for all Planet objects
    - Inherits from SolarBody.cs
    - Generates the Planet's moons, color, orbit, and what resources it contains
 ****************************************************/
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Planet : SolarBody
{
    //Enum that designates which type of planet this is
    public PlanetTypes type = PlanetTypes.Terrestrial;

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

    //Range of temperatures this planet can be between
    [HideInInspector]
    public Vector2 surfaceTemps = new Vector2();

    //Gradient for all colors this planet can be
    public Gradient surfaceColors;

    //Chance ( % ) that this planet can have water
    [Range(0, 1)]
    public float chanceOfWater = 0;
    [HideInInspector]
    public bool hasWater = false;

    //Min and max number of moons this planet can have
    public Vector2 moonNumberRanges;
    //Scalar that determines how many moons will be created
    public EaseType massMoonCoorilation = EaseType.CubeIn;
    //Min and max distances moons can be from this planet
    public Vector2 moonRadiusRanges;

    //List of moons that orbit this planet
    [HideInInspector]
    public List<GameObject> moons;

    //List of references to the different Moon prefabs that can be generated
    public List<GameObject> moonTypes;
    [Range(0,1)]
    //Sliders for how likely this type of moon is to be generated. MUST BE LISTED IN ASCENDING ORDER
    public List<float> moonProbabilities;

    //The minimum distance that can be between moons to avoid any collisions
    public float moonCollisionDistance = 5;



    /*Function called from SolarSystem.cs (GeneratePlanets function)
    Override of the SolarBody parent class' GenerateStats function since stars will have their own unique stats */
    public void GenerateStats(SolarSystem systemData_, Vector3 orbitDimensions_, float orbitTime_ = 1, bool spinClockwise_ = true)
    {
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

        //Determines if this planet has water
        if(Random.value <= this.chanceOfWater)
        {
            this.hasWater = true;
        }

        //Sets this planet's mesh material to a random color from surfaceColors gradient
        this.displayObject.GetComponent<MeshRenderer>().materials[0].color = this.surfaceColors.Evaluate(Random.value);

        //Fills this planet with resources for the player to gather
        this.GenerateResources(systemData_);

        //Creates all of the moons that orbit this planet
        this.GenerateMoons();
    }


    //~~~~~~~~ UNFINISHED ~~~~~~~~~
    //Influences the amount of resources this planet has based on the star's type and this planet's type
    private void GenerateResources(SolarSystem systemData_)
    {

    }


    //Called from GenerateResources. Generates a random assortment of moons to orbit this planet
    private void GenerateMoons()
    {
        //Initialize the new list of moons so that it's empty
        this.moons = new List<GameObject>();

        //Created a scalar that uses the massMoonCoorilation curve to skew the random value
        Interpolator scalar = new Interpolator(this.massMoonCoorilation);
        scalar.AddTime(Random.value);

        //Uses the scalar, min number of moons, and max number of moons to find out how many this planet will have
        int numMoons = Mathf.RoundToInt( scalar.GetProgress() * (this.moonNumberRanges.y - this.moonNumberRanges.x) - this.moonNumberRanges.x );

        //Loop to generate each moon
        for(int i = 0; i < numMoons; ++i)
        {
            //Created a new game object to store the instance of the created moon
            GameObject newMoon = null;

            //Finds the orbit distance of the new moon
            float radius = this.GetMoonRadius();

            //If the radius is invalid, we stop generating moons
            if (radius <= 0)
                break;

            //Random value from 0 to 1
            float rand = Random.value;

            //Loop through each moon prefab probability to check against the random value
            for(int m = 0; m < this.moonProbabilities.Count; ++m)
            {
                //If the current moon probability is higher than our rand, that prefab is created
                if(this.moonProbabilities[m] >= rand)
                {
                    newMoon = GameObject.Instantiate(this.moonTypes[m], this.transform.localPosition, this.transform.rotation) as GameObject;
                    break;
                }
                //If this is the last moon probability in the list, it's spawned by default
                else if((m + 1) >= this.moonProbabilities.Count)
                {
                    newMoon = GameObject.Instantiate(this.moonTypes[m], this.transform.localPosition, this.transform.rotation) as GameObject;
                }
            }

            //creates a random orbit time
            //NOTE: The values used are for testing
            float orbitTime = Random.Range(45, 300);

            //Adds the moon to this planet's list and parents it at the local center
            this.moons.Add(newMoon);
            newMoon.transform.parent = this.displayObject.transform;
            newMoon.transform.localPosition = new Vector3(0,0,0);

            //Generates the moon's stats
            newMoon.GetComponent<Moon>().GenerateStats(new Vector3(radius, 0, radius), orbitTime, this.spinClockwise);
        }
    }


    //Called from GenerateMoons. Returns a radius (float) to spawn a new moon at. Returns 0 if a valid radius isn't found
    private float GetMoonRadius()
    {
        //The radius that's returned by the end of the function
        float returnRadius = 0;
        //Counter to track the number of times we've attempted to generate a valid radius
        int endlessCount = 0;
        
        for(int i = 0; i < 1; ++i)
        {
            //Calculates the radius of the moon using the min/max radii
            returnRadius = Random.value * (this.moonRadiusRanges.y - this.moonRadiusRanges.x) + this.moonRadiusRanges.x;

            //Loops through each moon and see if its orbit is within collision distance of other moons
            for(int m = 0; m < this.moons.Count; ++m)
            {
                //Find the distance between each moon
                float check1 = Mathf.Abs(this.moons[m].GetComponent<Moon>().orbitDimensions.x - returnRadius);

                //If we find a moon within collision distance of the new radius, the loop is reset to try again
                if(check1 <= moonCollisionDistance)
                {
                    i = -1;
                    endlessCount += 1;

                    //If there's been 20 failed attempts to generate a new radius, the loop is broken and 0 is returned
                    if(endlessCount > 20)
                    {
                        i = 1;
                        return 0;
                    }
                    break;
                }
            }
        }
        
        return returnRadius;
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


//Enum for the different types of planets in Planet.cs
public enum PlanetTypes
{
    Terrestrial, //Rocky planet composed primarily of carbonaceous or silicate rocks or metals
    Sillicate, //Ter
    GasDwarf, //Low-mass planet made of mostly hydrogen and helium
    GasGiant, //High-mass planet made of mostly hydrogen and helium
    Ice, //Surface is icy
    IceGiant, //Large planet made of heavy substances like water, methane and ammonia
    Iron, //Iron-rich core and little to no mantle
    Lava, //Surface is mostly or entirely covered by molten lava
    Ocean, //Most to all of its mass is made of water
    Desert, //Terrestrial planet with almost no water
    Carbon, //Metal core surrounded by carbon-based minerals. AKA Diamond Planet
    Plutoid
}