/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Component to be attached to a player-controlled camera
    - Allows for basic WASD movement that takes into account the object's Y rotation
 ****************************************************/
using UnityEngine;
using System.Collections;

public class WASD_Movement : MonoBehaviour
{
    //Reference to the object where the player camera pivots
    public Transform cameraPivot;
    //Movement speed of this object
    public float moveSpeed = 50;
    //Amount of drag to slow down this object when no input is given
    [Range(0, 1)]
    public float moveDrag = 0.91f;
    //Maximum movement ranges from the origin that the player camera can move
    public Vector2 maxXZRanges = new Vector2(500, 500);
    //Tracks the player camera's current velocity
    private Vector3 currentVelocity = new Vector3();

	

	// Update is called once per frame
	private void Update ()
    {
        //Does nothing if the player is typing
        if (GlobalData.isTyping)
            return;

        //Slows down the player's movement with drag
        this.currentVelocity = this.currentVelocity * this.moveDrag;

        //Velocities to calculate and add to the current veloctiy
        float frontBackVelocity = 0;
        float leftRightVelocity = 0;

        //Moving Up
	    if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            frontBackVelocity += this.moveSpeed;
        }
        //Moving Down
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            frontBackVelocity -= this.moveSpeed;
        }

        //Moving Left
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            leftRightVelocity -= moveSpeed;
        }
        //Moving Right
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            leftRightVelocity += this.moveSpeed;
        }

        //Adjusts the new velocities to take rotation into account
        float xSpeed = frontBackVelocity * Mathf.Sin(this.cameraPivot.eulerAngles.y * Mathf.Deg2Rad);
        float zSpeed = frontBackVelocity * Mathf.Cos(this.cameraPivot.eulerAngles.y * Mathf.Deg2Rad);

        xSpeed += leftRightVelocity * Mathf.Sin((this.cameraPivot.eulerAngles.y + 90) * Mathf.Deg2Rad);
        zSpeed += leftRightVelocity * Mathf.Cos((this.cameraPivot.eulerAngles.y + 90) * Mathf.Deg2Rad);


        //Prevents the x coord from going beyond the max ranges
        if(this.transform.localPosition.x + xSpeed > this.maxXZRanges.x)
        {
            xSpeed = this.maxXZRanges.x - this.transform.localPosition.x;
        }
        else if(this.transform.localPosition.x + xSpeed < -this.maxXZRanges.x)
        {
            xSpeed = this.maxXZRanges.x + this.transform.localPosition.x;
        }


        //Prevents the z coord from going beyond the max ranges
        if (this.transform.localPosition.z + zSpeed > this.maxXZRanges.y)
        {
            zSpeed = this.maxXZRanges.y - this.transform.localPosition.z;
        }
        else if (this.transform.localPosition.z + zSpeed < -this.maxXZRanges.y)
        {
            zSpeed = this.maxXZRanges.y + this.transform.localPosition.z;
        }

        //Updates the position of this object using the final velocities
        this.transform.localPosition += new Vector3(xSpeed, 0, zSpeed);
    }
}
