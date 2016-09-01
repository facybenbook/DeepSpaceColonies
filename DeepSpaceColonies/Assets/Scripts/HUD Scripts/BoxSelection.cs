/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Logic that allows the player to select multiple game units at once
 ****************************************************/
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BoxSelection : MonoBehaviour
{
    //List of all game object transforms inside the box selection
    private List<Transform> unitsForSelection;

    //Screen position where the box selection starts
    private Vector3 mouseDownPos;
    //Screen position where the box selection ends
    private Vector3 mouseUpPos;
    //Rectangle created from the mouse up and down positions
    private Rect selectionBox;
    //Reference to the image that displays where the box selection is
    public Image selectionImage;
    //Bool that determines if the box selection should be active or not
    private bool ignoreClick = false;



    // Use this for initialization
    private void Awake ()
    {
        this.unitsForSelection = new List<Transform>();
	}


    // Update is called once per frame
    private void Update()
    {
        //If the Left Alt button is down (for camera movement), we can't use box selection
        if (Input.GetKey(KeyCode.LeftAlt))
            return;

        //On the first frame where L-mouse is down, stores the screen location of the mouse
        if (Input.GetMouseButtonDown(0))
        {
            this.mouseDownPos = Input.mousePosition;
            
            //If the mouse pointer is over a canvas element (HUD), the box selection isn't started
            if(EventSystem.current.IsPointerOverGameObject())
            {
                this.ignoreClick = true;
            }
        }

        //While the L-mouse is held, positions the selection image so that it scales to match the actual box
        if(Input.GetMouseButton(0))
        {
            if (this.ignoreClick)
                return;
            
            this.selectionImage.enabled = true;
            this.selectionImage.rectTransform.anchoredPosition = new Vector2( (Input.mousePosition.x + this.mouseDownPos.x) / 2,
                                                                        (Input.mousePosition.y + this.mouseDownPos.y) / 2);
            this.selectionImage.rectTransform.sizeDelta = new Vector2(Mathf.Abs(Input.mousePosition.x - this.mouseDownPos.x),
                                                                  Mathf.Abs(Input.mousePosition.y - this.mouseDownPos.y));
        }

        //When L-mouse is released, creates a selection box to find all objects inside
        if (Input.GetMouseButtonUp(0))
        {
            if(this.ignoreClick)
            {
                this.ignoreClick = false;
                return;
            }

            this.mouseUpPos = Input.mousePosition;
            this.selectionBox = new Rect();

            //Finding the XY dimentions of the selection box
            if(this.mouseDownPos.x < this.mouseUpPos.x)
            {
                this.selectionBox.xMin = this.mouseDownPos.x;
                this.selectionBox.xMax = this.mouseUpPos.x;
            }
            else
            {
                this.selectionBox.xMax = this.mouseDownPos.x;
                this.selectionBox.xMin = this.mouseUpPos.x;
            }

            if (this.mouseDownPos.y < this.mouseUpPos.y)
            {
                this.selectionBox.yMin = this.mouseDownPos.y;
                this.selectionBox.yMax = this.mouseUpPos.y;
            }
            else
            {
                this.selectionBox.yMax = this.mouseDownPos.y;
                this.selectionBox.yMin = this.mouseUpPos.y;
            }

            //Disables the selection image
            this.selectionImage.enabled = false;
            //Checks the selection box for selectable objects
            this.CheckRectSelection(this.selectionBox);

            //Resets the selection box's dimentions and mouse positions
            this.selectionBox = new Rect(0,0,0,0);
            this.mouseDownPos = new Vector3();
            this.mouseUpPos = new Vector3();
        }
    }


    /* Function called from Update
    Loops through all of the player units for selection to see if they are within the selection bounderies */
    private void CheckRectSelection(Rect selection_)
    {
        Vector2 screenPos;

        //Loops through each transform in the list
        for(int i = 0; i < this.unitsForSelection.Count; ++i)
        {
            //Converts the transform to a position in screen space
            screenPos = Camera.main.WorldToScreenPoint(this.unitsForSelection[i].position);
            
            //Checks that position in screen space to see if it's within the selection rectangle
            if (selection_.Contains(screenPos))
            {
                EVTData clickData = new EVTData();
                clickData.objectSelected = new ObjectSelectedEVT(this.unitsForSelection[i].gameObject);
                EventManager.TriggerEvent("MultiObjectSelect", clickData);
            }
        }
    }


    //Function called externally. Adds a new unit to the list of game units that the player can select
    public void AddUnitForSelection(Transform unitTransform_)
    {
        if(!this.unitsForSelection.Contains(unitTransform_))
        {
            this.unitsForSelection.Add(unitTransform_);
        }
    }


    //Function called externally. Removes a unit from the list of game objects that the player can select
    public void RemoveUnitForSelection(Transform unitTransform_)
    {
        if (this.unitsForSelection.Contains(unitTransform_))
        {
            this.unitsForSelection.Remove(unitTransform_);
        }
    }
}
