using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

using System.Collections.Generic;		//Allows us to use Lists. 
using UnityEngine.UI;					//Allows us to use UI.

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 0f;                      //TODO: Fix	//Time to wait before starting level, in seconds.
    public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.

    private bool doingSetup = true;							//Boolean to check if we're setting up board, prevent Player from moving during setup.
    
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);	
        }
        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
        InitGame();
    }

    
    //Initializes the game for each level.
    void InitGame()
    {
        doingSetup = true;
        Invoke("StartLevel", levelStartDelay);
    }
   
    void StartLevel()
    {
        SoundManager.instance.EngageStress();
        HideLevelImage();
    }
    
    //Hides black image used between levels
    void HideLevelImage()
    {
        doingSetup = false;
    }
    
    //Update is called every frame.
    void Update()
    {
        //doingSetup are not currently true.
        if(doingSetup)
        {
            return;
        }
        // NOTE: Don't know that we'll need to do much here
    }
    
    
    //GameOver is called when the player reaches 0 food points
    public void GameOver()
    {
        enabled = false;
    }
    
}
