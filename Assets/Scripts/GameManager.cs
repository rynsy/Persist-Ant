using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

using System.Collections.Generic;		//Allows us to use Lists. 
using UnityEngine.UI;					//Allows us to use UI.

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;						//Time to wait before starting level, in seconds.
    public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.
    
    private Text levelText;									//Text to display current level number.
    private Text levelTime;									//Current Time
    private GameObject levelImage;							//Image to block out level as levels are being set up, background for levelText.
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
        //While doingSetup is true the player can't move, prevent player from moving while title card is up.
        doingSetup = true;
        //Call the HideLevelImage function with a delay in seconds of levelStartDelay.

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelTime = GameObject.Find("LevelTime").GetComponent<Text>();

        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);
    }
    
    
    //Hides black image used between levels
    void HideLevelImage()
    {
        //Disable the levelImage gameObject.
        levelImage.SetActive(false);
        //Set doingSetup to false allowing player to move again.
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
        //Set levelText to display number of levels passed and game over message
        levelText.text = "You fuckin died";
        //Enable black background image gameObject.
        levelImage.SetActive(true);
        //Disable this GameManager.
        enabled = false;
    }
    
}
