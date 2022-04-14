using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

using System.Collections.Generic;		//Allows us to use Lists. 
using UnityEngine.UI;					//Allows us to use UI.

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 0f;                      //TODO: Fix	//Time to wait before starting level, in seconds.
    public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.


    public Camera cameraPrefab;
    public GameObject levelPrefab;
    public PlayerController playerPrefab;

    public Vector3 checkPoint;

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
    }

    //Initializes the game for each level.
    public void InitGame()
    {
        doingSetup = true;
        Debug.Log("Setting up the scene");
        StartLevel1();
        doingSetup = false;
    }

    public void StartMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void StartCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void StartIntro()
    {
        SceneManager.LoadScene("Intro");
    }

    public void StartLevel1()
    {
        SoundManager.instance.EngageStress();
        SceneManager.LoadScene("Level1");
    }
    
    //Hides black image used between levels
    
    //Update is called every frame.
    void Update()
    {
        //doingSetup are not currently true.
        if(doingSetup)
        {
            return;
        }
    }
   
    public void Restart()
    {
        /* If we want respawns to start the music after the game loop intro use the code below
        doingSetup = true;
        Debug.Log("Setting up the scene");
        SoundManager.instance.PlayMainGameLoop();
        SceneManager.LoadScene("Level1");
        doingSetup = false;
        */
        InitGame();
    }

    //GameOver is called when the player reaches 0 food points
    public void GameOver()
    {
        // Load GameOVerScene
        SceneManager.LoadScene("GameOver");
    }

    public void WinGame()
    {
        SceneManager.LoadScene("End");
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("StartMenu");
    }

}
