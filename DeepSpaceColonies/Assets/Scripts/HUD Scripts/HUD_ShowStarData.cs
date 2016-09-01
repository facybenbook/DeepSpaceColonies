/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Component to be attached to the UI object
    - Displays a description of a clicked star on the HUD
 ****************************************************/
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class HUD_ShowStarData : MonoBehaviour
{
    //The UnityEvent triggered when a star is selected
    public UnityEvent onSelectEvent;
    //Reference to the text field that displays the star's name
    public InputField starName;
    //Reference to the text field that displays the star's type
    public Text starType;
    //Reference to the text field that displays the star's mass
    public Text mass;
    //Reference to the text field that displays the star's radius
    public Text radius;
    //Reference to the text field that displays the star's temperature
    public Text temperature;
    //Reference to the text field that displays the star's luminosity
    public Text luminosity;

    //Delegate for when the player clicks a display object
    private DelegateEvent<EVTData> displayObjectSelectedEVT;



    // Function called on initialization
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
    Updates the HUD to show the selected star's data */
    private void DisplayObjectSelect(EVTData data_)
    {
        //Checks to make sure the object selected was a star
        if (data_.objectSelected.objectSelected == null || data_.objectSelected.objectSelected.transform.parent == null
            || data_.objectSelected.objectSelected.transform.parent.GetComponent<Star>() == null || MouseData.isMouseDisabled)
            return;

        Star selectedStar = data_.objectSelected.objectSelected.transform.parent.GetComponent<Star>();

        //Turns on the game object
        this.onSelectEvent.Invoke();

        //Displays the selected moon's data in the text fields on the HUD
        this.starName.text = selectedStar.name;
        GlobalData.isTyping = false;

        this.mass.text = ("Mass: " + selectedStar.mass + " Solar Masses");
        this.radius.text = ("Radius: " + selectedStar.radius + " Solar Radius");
        this.temperature.text = ("Temperature: " + selectedStar.surfaceTemp + " Kelvin");
        this.luminosity.text = ("Brightness: " + selectedStar.luminosity + " Solar Luminosity");

        //Switch statement to display the correct star type
        switch(selectedStar.type)
        {
            case StarTypes.MainSequence:
                this.starType.text = "Type: Main Sequence";
                break;
            case StarTypes.RedGiant:
                this.starType.text = "Type: Red Giant";
                break;
            case StarTypes.RedSupergiant:
                this.starType.text = "Type: Red Supergiant";
                break;
            case StarTypes.YellowSupergiant:
                this.starType.text = "Type: Yellow Supergiant";
                break;
            case StarTypes.BlueGiant:
                this.starType.text = "Type: Blue Giant";
                break;
            case StarTypes.BlueSupergiant:
                this.starType.text = "Type: Blue Supergiant";
                break;
            case StarTypes.RedDwarf:
                this.starType.text = "Type: Red Dwarf";
                break;
            case StarTypes.BlueDwarf:
                this.starType.text = "Type: Blue Dwarf";
                break;
            case StarTypes.WhiteDwarf:
                this.starType.text = "Type: White Dwarf";
                break;
            case StarTypes.BlackDwarf:
                this.starType.text = "Type Black Dwarf";
                break;
            case StarTypes.Neutron:
                this.starType.text = "Type: Neutron";
                break;
            case StarTypes.BlackHole:
                this.starType.text = "Black Hole";
                this.mass.text = "Mass: Unimaginably Vast";
                break;
        }
    }
}
