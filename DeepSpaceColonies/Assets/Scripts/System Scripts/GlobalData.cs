/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Component to be attached to a game object to make it persist between scenes
    - Sets the seed for Random functions
    - Can quit the game safely
 ****************************************************/
using UnityEngine;
using System.Collections;

public class GlobalData : MonoBehaviour
{
    //A reference to the object that stores data between scenes
    public static GameObject GlobalDataRef;

    //String that the player can use to set a seed for Random
    public string seed;

    //Bool that determines if the player is currently typing
    public static bool isTyping = false;



    //Function called on Initialization
    private void Awake()
    {
        //If there isn't already a static reference to this global data object, creates a new one
        if(GlobalDataRef == null)
        {
            DontDestroyOnLoad(this.gameObject);
            GlobalDataRef = this.gameObject;
        }
        //Otherwise we already have a global data object and we can't make a new one
        else if(GlobalDataRef != this.gameObject)
        {
            Destroy(gameObject);
        }

        //Sets the seed
        this.SetSeed(this.seed);
    }


    //Externally sets the random seed based on the string put in
    public void SetSeed(string seed_)
    {
        //Doesn't set the seed if the string is empty
        if (seed_ == "")
            return;

        //Converts the characters in the seed_ string to an int that becomes the seed
        int seedVal = 0;

        foreach(char c in seed_)
        {
            seedVal += c.GetHashCode();
        }

        Random.seed = seedVal;
    }


    //Externally tells the application to close
    public void QuitGame()
    {
        Application.Quit();
    }

    
    //Externally registers that the player is typing so that keyboard hotkeys don't trigger
    public void StartTyping()
    {
        isTyping = true;
    }


    //Externally registers that the player is finished typing so that keyboard hotkeys can trigger again
    public void DoneTyping()
    {
        isTyping = false;
    }
}
