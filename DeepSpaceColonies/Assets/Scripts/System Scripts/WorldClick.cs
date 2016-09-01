/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - NOTE: This component requires a collider of some kind
    - Component to be attached to the flat object that acts as the "ground"
    - Allows us to clear the current selection or set a position for units to move to
 ****************************************************/
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class WorldClick : MonoBehaviour
{
    //Function called when the player cursor is over this object
	private void OnMouseOver()
    {
        //Does nothing if the player mouse input is disabled
        if (MouseData.isMouseDisabled)
            return;

        //Clears the current selection if this object is left-clicked by itself
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()
            && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftAlt))
        {
            EventManager.TriggerEvent("ClearSelected");
        }

        //If right clicked over, sends an event with the click coords for selected objects to move to
        if(Input.GetMouseButtonDown(1))
        {
            //Creates a raycast from the mouse to the world position where it collides with this object
            RaycastHit clickCast;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out clickCast, 3000.0f))
            {
                //If the raycast hits something, dispatches a delegate event with the point hit
                if (clickCast.point != null)
                {
                    EVTData clickData = new EVTData();
                    clickData.mapClick = new MapClickEVT(clickCast.point);
                    EventManager.TriggerEvent("MapClick", clickData);
                }
            }
        }
    }
}
