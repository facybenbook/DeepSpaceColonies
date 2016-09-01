/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Component to be attached to logos on the splash screen
    - Fades the target logo in, lingers, fades out, and begins the next action when finished
 ****************************************************/
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Misc_SplashScreenLogic : MonoBehaviour
{
    //How long it takes for this logo to fade in from invisible
    public float fadeInTime = 0.2f;
    //How long this logo is on screen between fading in and fading out
    public float onScreenTime = 2.0f;
    //How long it takes for this logo to fade out
    public float fadeOutTime = 0.2f;
    //What happens when this logo is finished fading out
    public UnityEvent eventOnFinish;
    //Interpolator used to fade the canvas' alpha
    private Interpolator myInterp;
    //Quick reference to the canvas to fade
    private CanvasRenderer thisCanvas;
    //Bool that determines if the player is skipping the splash screen
    private bool isSkipping = false;



    //Function called on the first frame
    private void Start()
    {
        //Creating a new interpolator and setting the fade time
        this.myInterp = new Interpolator();
        this.myInterp.ease = EaseType.SineIn;
        this.myInterp.SetDuration(this.fadeInTime);

        //Getting the canvas renderer reference and makes it invisible
        this.thisCanvas = GetComponent<CanvasRenderer>();
        this.thisCanvas.SetAlpha(0);
    }

    // Update is called once per frame
    private void Update()
    {
        //Skips the splash screen if the player clicks or presses space or escape
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2) )
        {
            this.isSkipping = true;
        }


        //If there's still time left to fade in, fades in
        if (this.fadeInTime > 0 && this.isSkipping == false)
        {
            this.fadeInTime -= Time.deltaTime;
            this.myInterp.AddTime(Time.deltaTime);

            //Changes the alpha of this image to fade in based on the amount of time passed
            this.thisCanvas.SetAlpha(this.myInterp.GetProgress());
        }
        //If fading in is finished, stays on screen
        else if (this.onScreenTime > 0 && this.isSkipping == false)
        {
            this.onScreenTime -= Time.deltaTime;

            //Once it's finished staying on screen regularly, sets the interpolater to the fade out time
            if (this.onScreenTime <= 0)
            {
                this.myInterp.SetDuration(this.fadeOutTime);
                this.myInterp.AddTime(this.fadeOutTime);
            }
        }
        //If it's done staying on screen normally, fades out
        else if (this.fadeOutTime > 0)
        {
            this.fadeOutTime -= Time.deltaTime;
            this.myInterp.AddTime(-Time.deltaTime);

            //Changes the alpha of this image to fade out based on the amount of time passed
            this.thisCanvas.SetAlpha(this.myInterp.GetProgress());

            //Once it's finished fading out, activates the on finish event
            if (this.fadeOutTime <= 0)
            {
                this.eventOnFinish.Invoke();
            }
        }
    }
}
