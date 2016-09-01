/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Component to be attached to the UI object
    - Displays a description of a clicked planet on the HUD
 ****************************************************/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class HUD_ShowPlanetData : MonoBehaviour
{
    //The UnityEvent triggered when a planet is selected
    public UnityEvent onSelectEvent;
    //Reference to the text field that displays the planet's name
    public InputField planetName;
    //Reference to the text field that displays the planet's type
    public Text planetType;
    //Reference to the text field that displays the planet's mass
    public Text mass;
    //Reference to the text field that displays the planet's radius
    public Text radius;
    //Reference to the text field that displays if the planet has water
    public Text hasWater;

    //Delegate for when the player clicks a display object
    private DelegateEvent<EVTData> displayObjectSelectedEVT;



    //Function called on initialization
    private void Awake()
    {
        this.displayObjectSelectedEVT = new DelegateEvent<EVTData>(this.DisplayObjectSelect);
    }


    //Function called when this component is enabled on a game object
    private void OnEnable()
    {
        EventManager.StartListening("DisplayObjectSelected", this.displayObjectSelectedEVT);
    }


    //Function called when this component is disabled on a game object
    private void OnDisable()
    {
        EventManager.StopListening("DisplayObjectSelected", this.displayObjectSelectedEVT);
    }


    /* Function called from the EventManager.cs using the displayObjectSelectedEVT delegate event
    Updates the HUD to show the selected planet's data */
    private void DisplayObjectSelect(EVTData data_)
    {
        //Checks to make sure the object selected was a planet
        if (data_.objectSelected.objectSelected == null || data_.objectSelected.objectSelected.transform.parent == null
            || data_.objectSelected.objectSelected.transform.parent.GetComponent<Planet>() == null || MouseData.isMouseDisabled)
            return;

        Planet SelectedPlanet = data_.objectSelected.objectSelected.transform.parent.GetComponent<Planet>();

        //Turns on the game object
        this.onSelectEvent.Invoke();
        
        //Displays the selected planet's data in the text fields on the HUD
        this.planetName.text = SelectedPlanet.name;
        GlobalData.isTyping = false;

        this.mass.text = ("Mass: " + SelectedPlanet.mass + " Earth Masses");
        this.radius.text = ("Radius: " + SelectedPlanet.radius + " Earth Radius");
        this.hasWater.text = ("Has Water: " + SelectedPlanet.hasWater);

        //Displays the planet's type based on the SelectedPlanet type enum
        this.planetType.text = "Type: " + SelectedPlanet.type.ToString();
            //For multi-word cases
        switch (SelectedPlanet.type)
        {
            case PlanetTypes.GasDwarf:
                this.planetType.text = "Type: Gas Dwarf";
                break;
            case PlanetTypes.GasGiant:
                this.planetType.text = "Type: Gas Giant";
                break;
            case PlanetTypes.IceGiant:
                this.planetType.text = "Type: Ice Giant";
                break;
            case PlanetTypes.Terrestrial:
                this.planetType.text = "Type: Terrestrial";
                break;
        }
    }
}
