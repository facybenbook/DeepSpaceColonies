/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Component to interpolate one object to follow another
 ****************************************************/
using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour
{
    //Reference to the transform of the object to follow
    public Transform objectToFollow = null;
    //Percent of distance that's covered each frame
    [Range(0.01f, 1)]
    public float interpSpeed = 0.1f;

    //Delegate for when the player clicks a display object
    private DelegateEvent<EVTData> trackObjectEVT;



    //Function called on initialization
    private void Awake()
    {
        this.trackObjectEVT = new DelegateEvent<EVTData>(this.SetObjectToTrack);
    }


    //Function called when this component is enabled on a game object
    private void OnEnable()
    {
        EventManager.StartListening("CameraTrackSelected", this.trackObjectEVT);
    }


    //Function called when this component is enabled on a game object
    private void OnDisable()
    {
        EventManager.StopListening("CameraTrackSelected", this.trackObjectEVT);
    }


    /* Function called from the EventManager.cs using the trackObjectEVT delegate event
    Sets the object transform that this object will interpolate to, or clears it to stop interpolating */ 
    private void SetObjectToTrack(EVTData data_)
    {
        if (data_.cameraTrackObject.objectToFollow != null)
            this.objectToFollow = data_.cameraTrackObject.objectToFollow.transform;
        else
            this.objectToFollow = null;
    }

    
    // Update is called once per frame
    private void Update ()
    {
        if (this.objectToFollow == null)
            return;

        //Closes the distance between this object and the target object using the interp speed
        this.transform.position += (this.objectToFollow.position - this.transform.position) * this.interpSpeed;
	}
}
