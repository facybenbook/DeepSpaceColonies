/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Component that allows for spherical rotation for a camera mount
 ****************************************************/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Camera_Orbit : MonoBehaviour
{
    //The camera attached to this mount
    public GameObject thisMountsCamera = null;
    public Transform thisMountsPosition = null;

    //The radius that this camera starts at
    public float startRadius = 4.0f;

    //The closest and furthest away the camera can get to the mount
    public Vector2 radiusRanges = new Vector2(1, 20);

    //How much the camera zooms in/out when the scroll wheel is used
    public float scrollSpeed = 1;

    //The xy angle that this mount starts out at
    public Vector2 startXYAngle = new Vector2(0, 0);

    //The speed that this camera rotates each frame
    public Vector2 turnSpeedXY = new Vector2(6, 5);

    //The max x angle that this mount can rotate to prevent it from flipping over
    public float maxXRotation = 88;
    public float minXRotation = -55;



    // Use this for initialization
    private void Start()
    {
        //Sets the position of this mount's camera to the correct starting distance
        this.thisMountsCamera.transform.localPosition = new Vector3(0, 0, -this.startRadius);

        //Makes sure that our starting angle isn't below the min x angle
        if (this.startXYAngle.x < this.minXRotation)
        {
            this.startXYAngle.x = this.minXRotation;
        }

        //Sets the rotation of this mount to the correct starting rotation
        transform.eulerAngles = new Vector3(this.startXYAngle.x, this.startXYAngle.y, 0);
    }



    // Update is called once per frame
    private void Update()
    {
        //Moves the camera in and out based on the input from the mouse scroll wheel
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel") * this.scrollSpeed;
        this.thisMountsCamera.transform.localPosition += new Vector3(0,0, scrollDelta);

        //Makes sure camera stays within the min/max distance radii
        if (this.thisMountsCamera.transform.localPosition.z < -this.radiusRanges.y)
            this.thisMountsCamera.transform.localPosition = new Vector3(0,0, -this.radiusRanges.y);
        else if (this.thisMountsCamera.transform.localPosition.z > -this.radiusRanges.x)
            this.thisMountsCamera.transform.localPosition = new Vector3(0,0, -this.radiusRanges.x);


        //Can't rotate unless the left mouse button and left alt are held
        if (!Input.GetMouseButton(0) || !Input.GetKey(KeyCode.LeftAlt))
            return;

        //Finds the amount to rotate the x and y values based on the mouse movements
        float xRotToAdd = Input.GetAxis("Mouse Y") * -this.turnSpeedXY.x;
        float yRotToAdd = Input.GetAxis("Mouse X") * this.turnSpeedXY.y;


        //If the current rotation is between 0 and 90 degrees
        if (transform.eulerAngles.x >= 0 && transform.eulerAngles.x < 90)
        {
            //If adding to the x rotation would put it over the max x rotation, it stops it from going over
            if (xRotToAdd + this.transform.eulerAngles.x > this.maxXRotation)
            {
                xRotToAdd = this.maxXRotation - this.transform.eulerAngles.x;
            }
            //if the min x rotation is at or above 0 and rotating would put it below the min, it stops it from going under
            else if (this.minXRotation >= 0 && xRotToAdd + this.transform.eulerAngles.x < this.minXRotation)
            {
                xRotToAdd = this.minXRotation - this.transform.eulerAngles.x;
            }
        }
        //Otherwise, the rotation went below 0 and looped back around past 360 degrees
        else
        {
            //If adding to the x rotation would put it under the min x rotation, it stops it from going under
            if (xRotToAdd + this.transform.eulerAngles.x < 360 + this.minXRotation)
            {
                xRotToAdd = ((360 + this.minXRotation) - this.transform.eulerAngles.x);
            }
        }

        //Adds the final rotation values to the current transforms
        this.transform.eulerAngles += new Vector3(xRotToAdd, 0, 0);
        this.thisMountsPosition.eulerAngles += new Vector3(0,yRotToAdd,0);
    }
}
