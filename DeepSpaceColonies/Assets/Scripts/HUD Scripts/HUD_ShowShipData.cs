/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Component to be attached to each Ship icon in the HUD
    - Displays a clicked ship's icon and health
 ****************************************************/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class HUD_ShowShipData : MonoBehaviour
{
    //Ship index in the selection list that this component tracks
    [Range(0,11)]
    public int shipNumber;
    //Reference to the selected ship's Ship component
    private Ship selectedShip = null;
    //Reference to the selected ship's inventory
    private Inventory shipInventory = null;
    //Reference to the selected ship's health tracker
    private HealthTracker shipHealth = null;
    //Reference to the HUD panel that displays this ship's info
    public GameObject panel;
    //Reference to the image that displays the selected ship's icon
    public Image icon;
    //Reference to the image that displays the selected ship's health
    public Image healthBar;

    //Delegates for when new ships are selected or the selection is cleared
    private DelegateEvent<EVTData> newSelectionEVT;
    private DelegateEvent<EVTData> clearSelectionEVT;



    //Function called on initialize
    private void Awake()
    {
        this.newSelectionEVT = new DelegateEvent<EVTData>(this.NewSelection);
        this.clearSelectionEVT = new DelegateEvent<EVTData>(this.ClearSelection);
    }


    //Function called when this component is enabled on a game object
    private void OnEnable()
    {
        EventManager.StartListening("DisplayObjectSelected", this.newSelectionEVT);
        EventManager.StartListening("MultiObjectSelect", this.newSelectionEVT);
        EventManager.StartListening("ClearSelected", this.clearSelectionEVT);

        //Makes sure the selection is empty by default
        this.ClearSelection();
    }


    //Function called when this component is disabled on a game object
    private void OnDisable()
    {
        EventManager.StopListening("DisplayObjectSelected", this.newSelectionEVT);
        EventManager.StopListening("MultiObjectSelect", this.newSelectionEVT);
        EventManager.StopListening("ClearSelected", this.clearSelectionEVT);
    }


    //Update is called every frame
    private void Update()
    {
        //Does nothing if there's no ship selected
        if (this.selectedShip == null)
            return;
        
        //Keeps the selected ship's health up to date
        this.healthBar.rectTransform.localScale = new Vector3(((this.shipHealth.currentHealth * 1.0f) / this.shipHealth.maxHealth), 1, 1);
    }


    /* Function called from the EventManager.cs using the newSelectionEVT delegate event
    Updates the ship panels on the HUD to show the selected ships' data */
    private void NewSelection(EVTData data_)
    {
        //If the first object in the selection isn't a ship, the selection is cleared and nothing else happens
        if (MouseData.objectSelected[0].GetComponent<Ship>() == null)
        {
            this.ClearSelection();
            return;
        }

        //Makes sure that the number of ships selected matches this component's index
        if(MouseData.objectSelected.Count > this.shipNumber)
        {
            //Sets the references to the selected ship's Ship, Inventory, and Health components
            this.selectedShip = MouseData.objectSelected[this.shipNumber].GetComponent<Ship>();
            this.shipInventory = MouseData.objectSelected[this.shipNumber].GetComponent<Inventory>();
            this.shipHealth = MouseData.objectSelected[this.shipNumber].GetComponent<HealthTracker>();
            
            //Turns on this panel's game object and sets its icon
            this.panel.SetActive(true);

            this.icon.sprite = this.selectedShip.hudIcon;
        }
        //Otherwise, this ship panel is cleared
        else
        {
            ClearSelection();
        }
    }


    /*Function called from NewSelection and OnEnable, and from the EventManager.cs using the clearSelectionEVT delegate event
    Nulls all references and disables the panel's game object */
    private void ClearSelection(EVTData data_ = null)
    {
        this.selectedShip = null;
        this.shipInventory = null;
        this.shipHealth = null;
        this.panel.SetActive(false);
    }
}
