using UnityEngine;
using System.Collections;
using UnityEngine.UI;   //Allows us to use UI.
using UnityEngine.SceneManagement;

//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class PlayerController :  MonoBehaviour
{

    [SerializeField] private int speed = 5;
    [SerializeField] private int jumpHeight = 5;
    [SerializeField] private int speedBoostFactor = 2;
    [SerializeField] private float speedBoostDuration = 10f;




    private Rigidbody2D rigidBodyComponent;
    private bool jumpKeyWasPressed;
    private float horizontalInput;
    private int _time = 0;
    public int Time
    {
        get
        {
            return _time;
        }
        set
        {
            _time = value;
            levelTime.text = "Time: " + _time;
        }
    }

    public Text levelTime;

    public AudioClip moveSound1;                //1 of 2 Audio clips to play when player moves.
    public AudioClip moveSound2;                //2 of 2 Audio clips to play when player moves.
    public AudioClip gameOverSound;             //Audio clip to play when player dies.

    private Animator animator;                  //Used to store a reference to the Player's animator component.

    //Start overrides the Start function of MovingObject
    protected void Start()
    {
        //Get a component reference to the Player's animator component
        animator = GetComponent<Animator>();
        rigidBodyComponent = GetComponent<Rigidbody2D>();
    }


    //This function is called when the behaviour becomes disabled or inactive.
    private void OnDisable()
    {
        //NOTE: Might not need this
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpKeyWasPressed = true;
        }
        horizontalInput = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        if (horizontalInput > 0)    // Player is facing right
        {
            animator.SetTrigger("playerFacingRight");
        } 
        else if (horizontalInput < 0) // Player is facing left 
        {
            animator.SetTrigger("playerFacingLeft");
        }

        rigidBodyComponent.velocity = new Vector2(horizontalInput * speed, rigidBodyComponent.velocity.y);
        if (jumpKeyWasPressed)
        {
            rigidBodyComponent.AddForce(transform.up * 5f, ForceMode2D.Impulse);
            jumpKeyWasPressed = false;
        }

        if (rigidBodyComponent.position.y < -10)
        {
            GameOver();
        }

        Time += 1;
    }


    //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Check if the tag of the trigger collided with is Exit.
        if (other.tag == "Enemy")
        {
            GameOver();
        } else if (other.tag == "Item")
        {
            Destroy(other.gameObject);
            ApplySpeedBoost();
        }

    }

    private void ApplySpeedBoost()
    {
        speed *= speedBoostFactor;
        Invoke("RemoveSpeedBoost", speedBoostDuration);
    }
    private void RemoveSpeedBoost()
    {
        speed /= speedBoostFactor;
    }



    //Restart reloads the scene when called.
    private void Restart()
    {
        //Load the last scene loaded, in this case Main, the only scene in the game. And we load it in "Single" mode so it replace the existing one
        //and not load all the scene object in the current scene.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }


    //CheckIfGameOver checks if the player is out of food points and if so, ends the game.
    private void GameOver()
    {
        //Call the PlaySingle function of SoundManager and pass it the gameOverSound as the audio clip to play.
        SoundManager.instance.PlaySingleSoundEffect(gameOverSound);

        //Stop the background music.
        SoundManager.instance.StopMusic();

        //Call the GameOver function of GameManager.
        GameManager.instance.GameOver();
    }
}
