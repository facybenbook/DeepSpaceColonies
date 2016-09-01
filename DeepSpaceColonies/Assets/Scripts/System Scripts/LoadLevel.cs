/****************************************************
Project:    Deep Space Colonies
Engine:     Unity v5.3.1
Author:     Mitchell Regan
Date:       July 2016
Description:
    - Component that should be attached to the same game object as GlobalData so that it persists
    - Component that allows Unity UI Buttons to load new levels
 ****************************************************/
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    //Loads the level with the given name
    public void LoadLevelByName(string levelName_)
    {
        SceneManager.LoadScene(levelName_);
    }


    //Loads the level with the given scene index
    public void LoadLevelByIndex(int levelIndex_)
    {
        SceneManager.LoadScene(levelIndex_);
    }
}