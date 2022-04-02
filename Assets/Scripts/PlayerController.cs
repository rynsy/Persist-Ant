using UnityEngine;
using System.Collections;
using UnityEngine.UI;   //Allows us to use UI.
using UnityEngine.SceneManagement;

//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class PlayerController :  MonoBehaviour
{

    [SerializeField] private int speed = 5;
    [SerializeField] private int speedBoostFactor = 2;
    [SerializeField] private float jumpFactor = 5;
    [SerializeField] private float speedBoostDuration = 10f;
    [SerializeField] private float chargeBoostFactor = 1.5f;
    [SerializeField] private float chargeBoostDuration = 0.5f;
    [SerializeField] private float chargeBoostCooldown = 5f;

    private Rigidbody2D rigidBodyComponent;
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
    public AudioClip moveSound1;

    private Animator animator;                  //Used to store a reference to the Player's animator component.

    public bool canJump = true;
    public bool canCharge = true;

    public bool isJumping = false;
    public bool isCharging = false;
    public bool onGround;

    private bool jumpKeyWasPressed;
    private bool chargeKeyWasPressed;
    private bool playerFacingRight = true;

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
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            chargeKeyWasPressed = true;
        }
        horizontalInput = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        if (!isCharging)
        {
            if (horizontalInput > 0)    // Player is facing right
            {
                animator.SetTrigger("playerFacingRight");
                playerFacingRight = true;
            } 
            else if (horizontalInput < 0) // Player is facing left 
            {
                animator.SetTrigger("playerFacingLeft");
                playerFacingRight = false;
            }
            Move(new Vector2(horizontalInput * speed, rigidBodyComponent.velocity.y));
        } 
        else
        {
            Charge();
        }

        if (jumpKeyWasPressed && canJump)
        {
            Jump();                         // This sets jumpKeyPressed to false
        } 
        else
        {
            jumpKeyWasPressed = false;      // Need to be sure this gets reset. probably a cleaner way of writing this
        }

        if (chargeKeyWasPressed && canCharge)
        {
            StartCharge();
        } 
        else
        {
            chargeKeyWasPressed = false;
        }

        if (rigidBodyComponent.position.y < -10)
        {
            GameOver();
        }

        Time += 1;
    }

    private void Move(Vector2 dir)
    {
        rigidBodyComponent.velocity = dir;
        //play move sound
        if (onGround)
        {
            SoundManager.instance.PlaySingleSoundEffect(moveSound1);
        }
    }

    private void Jump()
    {
        jumpKeyWasPressed = false;
        isJumping = true;
        canJump = false;
        onGround = false;
        Move(new Vector2(rigidBodyComponent.velocity.x, transform.up.y * jumpFactor));
    }

    private void StartCharge()
    {
        chargeKeyWasPressed = false;
        canCharge = false;
        isCharging = true;
        Invoke("RemoveChargeBoost", chargeBoostDuration);
        Invoke("RemoveChargeBoostCooldown", chargeBoostCooldown);
    }

    private void Charge()
    {
        if (playerFacingRight)
        {
            Move(new Vector2(transform.right.x * speed * chargeBoostFactor, rigidBodyComponent.velocity.y));
        } else
        {
            Move(new Vector2(-transform.right.x * speed * chargeBoostFactor, rigidBodyComponent.velocity.y));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            canJump = true;
            isJumping = false;
            onGround = true;
        }    
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

    private void RemoveChargeBoost()
    {
        isCharging = false;
    }

    private void RemoveChargeBoostCooldown()
    {
        canCharge = true;
    }

    //Restart reloads the scene when called.
    private void Restart()
    {
        //Load the last scene loaded, in this case Main, the only scene in the game. And we load it in "Single" mode so it replace the existing one
        //and not load all the scene object in the current scene.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    private void GameOver()
    {
        SoundManager.instance.GameOver();
        GameManager.instance.GameOver();
    }
}
