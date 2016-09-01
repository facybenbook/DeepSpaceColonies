/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 3, 2016
Description:
    - Generates a new solar system when a new game is started
        - Creates a star based on the star prefabs and their given probabilities
        - Calculates the heat ranges that planets can spawn in based on the star's heat and size
        - Creates a random number of planets to orbit the star. The planet types vary based on their orbit distance
    - Stores references to all objects in this system and parents them to this script's game object
 ****************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic; //Used for Lists

public class SolarSystem : MonoBehaviour
{
    //Bool that determines if this system spins clockwise (true) or counter clockwise (false)
    [HideInInspector]
    public bool spinClockwise = true;

    //The star in this system that can be referenced
    [HideInInspector]
    public GameObject solarStar;

    //The minimum and maximum range values for how many planets can be generated
    public Vector2 numPlanetsRange = new Vector2(1, 16);

    //The list of planets in this system that can be referenced
    [HideInInspector]
    public List<GameObject> planets = new List<GameObject>();

    //The list of other objects in this system that aren't stars, planets, or moons that can be referenced
    [HideInInspector]
    public List<GameObject> solarObjects = new List<GameObject>();

    //The radius that determines the closest distance a planet can be to the star. Determined by the star
    [HideInInspector]
    public float hotZoneRadius;

    //The distance radii of where habitable planets can spawn. X is the inner radius, Y is the outer radius. Determined by the star
    [HideInInspector]
    public Vector2 habitableRadius;

    //The distance radii of where cold planets can spawn. Determined by the star
    [HideInInspector]
    public float coldZoneRadius;

    //The distance radii of the furthest distance planets can spawn. Determined by the star
    [HideInInspector]
    public float iceZoneRadius;

    //References to different types of star prefabs that can be generated
    public List<GameObject> starTypes = new List<GameObject>(1);
    //Sliders for how likely this type of star is to be generated. MUST BE LISTED IN ASCENDING ORDER
    [Range(0,1)]
    public List<float> starProbabilities = new List<float>(1);

    //References to different types of planet prefabs that can be spawned in the hot zone
    public List<GameObject> hotPlanetTypes = new List<GameObject>(1);
    //Sliders for how likely this type of planet is to be generated. MUST BE LISTED IN ASCENDING ORDER
    [Range(0, 1)]
    public List<float> hotPlanetProbabilities = new List<float>(1);

    //References to different types of planet prefabs that can be spawned in the habitable zone
    public List<GameObject> habitPlanetTypes = new List<GameObject>(1);
    //Sliders for how likely this type of planet is to be generated. MUST BE LISTED IN ASCENDING ORDER
    [Range(0, 1)]
    public List<float> habitPlanetProbabilities = new List<float>(1);

    //References to different types of planet prefabs that can be spawned in the cold zone
    public List<GameObject> coldPlanetTypes = new List<GameObject>(1);
    //Sliders for how likely this type of planet is to be generated. MUST BE LISTED IN ASCENDING ORDER
    [Range(0, 1)]
    public List<float> coldPlanetProbabilities = new List<float>(1);

    //References to different types of planet prefabs that can be spawned in the ice zone
    public List<GameObject> icePlanetTypes = new List<GameObject>(1);
    //Sliders for how likely this type of planet is to be generated. MUST BE LISTED IN ASCENDING ORDER
    [Range(0, 1)]
    public List<float> icePlanetProbabilities = new List<float>(1);

    //The minimum distance that can be between planets to avoid any collisions
    public float collisionDistance = 20;

    //Reference to the mount for this solar system's designated camera
    public GameObject cameraMount;

    //References to the transforms for game objects that display heat zones for this system's star
    public Transform floor;
    public Transform hot;
    public Transform habitable1;
    public Transform habitable2;
    public Transform cold;
    public Transform ice;



    //Function called on startup to generate the star and planets
    void Start()
    {
        //Determines the direction of spin of this solar system
        float rand = Random.value;

        //Spins clockwise
        if (rand <= 0.5)
            this.spinClockwise = true;
        //Spins counter clockwise
        else
            this.spinClockwise = false;

        //Starts by creating this system's star at the center
        this.GenerateStar();
        //After that, we calculate the radii of all of the zones created by the star's size and heat
        this.CalculateRadii();
        //Then generate all of our planets and place them at their respective locations based on the radii
        this.GeneratePlanets();
    }


    //Function called from Start. Randomly creates a star from the designated prefabs based on their probabilities
    private void GenerateStar()
    {
        //Makes sure there are star prefabs to pick from. If none exist, the solar system cannot be created.
        if(this.starProbabilities.Count == 0)
        {
            Debug.LogError("ERROR: SolarSystem.GenerateStar, there are no star prefabs to pick from.");
            return;
        }

        //Create a new game object to store the instance of the created star
        GameObject newStar = null;

        //Creating a random value from 0-1 to determine what type of star is created
        float rand = Random.value;

        //Incriment through each star's probability until we find one that is larger than the rand value generated
        for(int i = 0; i < this.starProbabilities.Count; ++i)
        {
            //When we find a probability that is larger than our rand, we create that type of star.
            if(this.starProbabilities[i] >= rand)
            {
                newStar = GameObject.Instantiate(this.starTypes[i], this.transform.position, this.transform.rotation) as GameObject;
                break;
            }
            //If the loop ends after this and we still haven't found a star, the last one in the list is created and we log a warning
            else if((i + 1) >= this.starProbabilities.Count)
            {
                newStar = GameObject.Instantiate(this.starTypes[i], this.transform.position, this.transform.rotation) as GameObject;
                Debug.LogWarning("WARNING: SolarSystem.GenerateStar, no valid star probabilities available. Our Rand: " + rand);
            }
        }

        //Store the reference to the created star and generates its stats
        this.solarStar = newStar;
        this.solarStar.GetComponent<Star>().GenerateStats(this.spinClockwise);

        //Parent the new star to this solar system
        this.solarStar.transform.SetParent(this.gameObject.transform);
    }


    //Function called from Start. Generates the different heat zones for planets based on this system's star luminosity
    private void CalculateRadii()
    {
        /*Formula for radius: radius = sqrt( Luminosity / stellar flux value)
        NOTE: I took the cube root of this calculated radius and put in some multipliers to fudge the size to look
        accurate (given the small scale of the models) and make sure everything could fit on screen reasonably */

        //Stores the value of the star's luminosity for quick reference
        float luminosity = this.solarStar.GetComponent<Star>().luminosity;

        //Finding the radius of the Hot Zone (about 3 times closer than the habitable zone)
        this.hotZoneRadius = 5 * Mathf.Sqrt(Mathf.Sqrt(Mathf.Sqrt(luminosity / 9f)));

        //Finding the habitable zone's inner and outer radii
        this.habitableRadius = new Vector2(9 * Mathf.Sqrt(Mathf.Sqrt(Mathf.Sqrt(luminosity / 1.1f))),
                                        10 * Mathf.Sqrt(Mathf.Sqrt(Mathf.Sqrt(luminosity / 0.53f))) );

        //Finding the cold zone's radius (about 10 times the distance as the habitable zone)
        this.coldZoneRadius = 12 * Mathf.Sqrt(Mathf.Sqrt(Mathf.Sqrt(luminosity / 0.1f)));

        //Finding the radius of the Ice Zone (about 30 times the distance as the habitable zone)
        this.iceZoneRadius = 20 * Mathf.Sqrt(Mathf.Sqrt(Mathf.Sqrt(luminosity / 0.033f)));

        //Sets the camera's max movement range so that it's only as large as the maximum planet radius
        this.cameraMount.GetComponent<WASD_Movement>().maxXZRanges = new Vector2(this.iceZoneRadius * 1.3f, this.iceZoneRadius * 1.3f);

        //Scales up the floor so that it's only as big as the maximum planet radius to prevent the player from sending units outside of the system
        this.floor.localScale = new Vector3(this.iceZoneRadius * 0.235f, this.iceZoneRadius * 0.235f, this.iceZoneRadius * 0.235f);

        /*Scaling up the heat zone display objects to match the actual distances calculated
        NOTE: The sprite source image changes the amount multiplied to look correct, and has a different scale to the Floor sprite*/
        Vector3 scaleMultiplier = new Vector3(12.1f, 12.1f, 12.1f);

        this.hot.localScale = this.hotZoneRadius * scaleMultiplier;
        this.habitable1.localScale = this.habitableRadius.x * scaleMultiplier;
        this.habitable2.localScale = this.habitableRadius.y * scaleMultiplier;
        this.cold.localScale = this.coldZoneRadius * scaleMultiplier;
        this.ice.localScale = this.iceZoneRadius * scaleMultiplier;
    }


    //Function called from Start. Generates a random assortment of planets to orbit the system's star based on their radius
    private void GeneratePlanets()
    {
        //Determine how many planets there are given our min/max range
        int numPlanets = Mathf.RoundToInt(Random.Range(this.numPlanetsRange.x, this.numPlanetsRange.y));

        //Initialize the list of planets so that it's empty
        this.planets = new List<GameObject>();

        //Loops through each planet that we need to create and determines its distance from the star, which then determines what type of planet spawns
        for(int p = 0; p < numPlanets; ++p)
        {
            //Create a new game object to store the instance of the created planet
            GameObject newPlanet = null;

            //Calls the GetPlanetRadius function to set this planet's distance from the star
            float radius = this.GetPlanetRadius();

            //If the radius returned from GetPlanetRadius function isn't valid, no new planets can be created and the loop is broken
            if (radius <= 0)
                break;

            //If the radius is inside the hot zone
            if (radius < this.habitableRadius.x)
            {
                //Creating a random value from 0-1 to determine what type of hot planet is created
                float rand = Random.value;

                //Incriment through each hot planet's probability until we find one that is larger than the rand value generated
                for (int i = 0; i < this.hotPlanetProbabilities.Count; ++i)
                {
                    //When we find a probability that is larger than our rand, we create that type of planet.
                    if (this.hotPlanetProbabilities[i] >= rand)
                    {
                        newPlanet = GameObject.Instantiate(this.hotPlanetTypes[i], this.transform.position, this.transform.rotation) as GameObject;
                        break;
                    }
                    //If the loop ends after this and we still haven't found a planet, the last one in the list is created and we log a warning
                    else if ((i + 1) >= this.hotPlanetProbabilities.Count)
                    {
                        newPlanet = GameObject.Instantiate(this.hotPlanetTypes[i], this.transform.position, this.transform.rotation) as GameObject;
                        Debug.LogWarning("WARNING: SolarSystem.GeneratePlanet, no valid probabilities available. Our Rand: " + rand);
                    }
                }
            }
            //If the radius is inside the habitable zone
            else if(radius >= this.habitableRadius.x && radius < this.habitableRadius.y)
            {
                //Creating a random value from 0-1 to determine what type of habitable planet is created
                float rand = Random.value;

                //Incriment through each habitable planet's probability until we find one that is larger than the rand value generated
                for (int i = 0; i < this.habitPlanetProbabilities.Count; ++i)
                {
                    //When we find a probability that is larger than our rand, we create that type of planet.
                    if (this.habitPlanetProbabilities[i] >= rand)
                    {
                        newPlanet = GameObject.Instantiate(this.habitPlanetTypes[i], this.transform.position, this.transform.rotation) as GameObject;
                        break;
                    }
                    //If the loop ends after this and we still haven't found a planet, the last one in the list is created and we log a warning
                    else if ((i + 1) >= this.habitPlanetProbabilities.Count)
                    {
                        newPlanet = GameObject.Instantiate(this.habitPlanetTypes[i], this.transform.position, this.transform.rotation) as GameObject;
                        Debug.LogWarning("WARNING: SolarSystem.GeneratePlanet, no valid probabilities available. Our Rand: " + rand);
                    }
                }
            }
            //If the radius is inside the cold zone
            else if(radius >= this.habitableRadius.y && radius < this.coldZoneRadius)
            {
                //Creating a random value from 0-1 to determine what type of habitable planet is created
                float rand = Random.value;

                //Incriment through each cold planet's probability until we find one that is larger than the rand value generated
                for (int i = 0; i < this.coldPlanetProbabilities.Count; ++i)
                {
                    //When we find a probability that is larger than our rand, we create that type of planet.
                    if (this.coldPlanetProbabilities[i] >= rand)
                    {
                        newPlanet = GameObject.Instantiate(this.coldPlanetTypes[i], this.transform.position, this.transform.rotation) as GameObject;
                        break;
                    }
                    //If the loop ends after this and we still haven't found a planet, the last one in the list is created and we log a warning
                    else if ((i + 1) >= this.coldPlanetProbabilities.Count)
                    {
                        newPlanet = GameObject.Instantiate(this.coldPlanetTypes[i], this.transform.position, this.transform.rotation) as GameObject;
                        Debug.LogWarning("WARNING: SolarSystem.GeneratePlanet, no valid probabilities available. Our Rand: " + rand);
                    }
                }
            }
            //If the radius is inside the ice zone
            else
            {
                //Creating a random value from 0-1 to determine what type of habitable planet is created
                float rand = Random.value;

                //Incriment through each ice planet's probability until we find one that is larger than the rand value generated
                for (int i = 0; i < this.icePlanetProbabilities.Count; ++i)
                {
                    //When we find a probability that is larger than our rand, we create that type of planet.
                    if (this.icePlanetProbabilities[i] >= rand)
                    {
                        newPlanet = GameObject.Instantiate(this.icePlanetTypes[i], this.transform.position, this.transform.rotation) as GameObject;
                        break;
                    }
                    //If the loop ends after this and we still haven't found a planet, the last one in the list is created and we log a warning
                    else if ((i + 1) >= this.icePlanetProbabilities.Count)
                    {
                        newPlanet = GameObject.Instantiate(this.icePlanetTypes[i], this.transform.position, this.transform.rotation) as GameObject;
                        Debug.LogWarning("WARNING: SolarSystem.GeneratePlanet, no valid probabilities available. Our Rand: " + rand);
                    }
                }
            }
            
            
            //Creates a random orbit time for the planet (in real-world seconds)
            float orbitTime = Random.Range(60, 600);

            //Calls the planet's constructor function to generate its stats
            newPlanet.GetComponent<Planet>().GenerateStats(GetComponent<SolarSystem>(), new Vector3(radius, 0, radius), orbitTime, this.spinClockwise);
            //Adds the new planet to the system's list so it can be referenced, and parent it to the system
            this.planets.Add(newPlanet);
            newPlanet.transform.parent = this.transform;
        }
    }


    //Function called from GeneratePlanets. Returns a radius (float) to spawn a new planet at. Returns 0 if a valid radius isn't found.
    private float GetPlanetRadius()
    {
        //The radius that is returned by the end of the function
        float returnRadius = 0;

        //Create a sine-in scalar that will weight more of the planets to spawn closer to the star
        Interpolator scalar = new Interpolator(EaseType.SineIn);

        //Counter to track the number of times we've attempted to generate a valid radius
        int endlessCount = 0;

        for(int i = 0; i < 1; ++i)
        {
            //Gives the scalar a random value between 0 and 1 to give us a percentage that's weighted closer to 0 (closer to the star)
            scalar.ResetTime();
            scalar.AddTime(Random.value);

            //Calculates the radius of the planet using the min/max radii (hot and cold) and the scalar's weighted percentage
            returnRadius = scalar.GetProgress() * (this.iceZoneRadius - this.hotZoneRadius) + this.hotZoneRadius;

            //Loops through each planet to see if its orbit is within collision distance
            for (int p = 0; p < this.planets.Count; ++p)
            {
                float check1 = Mathf.Abs(this.planets[p].GetComponent<Planet>().orbitDimensions.x - returnRadius);

                //If we find a planet within collision distance of the new radius, the loop is reset to try again
                if(check1 <= this.collisionDistance)
                {
                    i = -1;
                    endlessCount += 1;

                    //If there have been 20 failed attempts to generate a new radius, the loop is broken and 0 is returned
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
}
