using UnityEngine;
using System.Collections;
using UnityEngine.UI;   //Allows us to use UI.
using UnityEngine.SceneManagement;
using System;

//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class PlayerController :  MonoBehaviour
{
    public Camera playerCamera;
    public ParticleSystem dust;

    [SerializeField] private int playerHealth = 3;
    [SerializeField] private int playerSpeed = 5;
    [SerializeField] private int speedBoostFactor = 2;
    [SerializeField] private float jumpFactor = 5f;
    [SerializeField] private float bouncePadJumpFactor = 10f;
    [SerializeField] private float speedBoostDuration = 10f;
    [SerializeField] private float chargeBoostFactor = 1.5f;
    [SerializeField] private float chargeBoostDuration = 0.5f;
    [SerializeField] private float chargeBoostCooldown = 5f;
    [SerializeField] private float parallaxSpeed;

    public FreeParallax parallaxComponent;
    private Rigidbody2D rigidBodyComponent;

    private Vector2 moveDir; 
    
    public Text levelTimeText;
    public Text playerHealthText;
    public AudioClip moveSound1;
    public AudioClip playerHurtSound;
    public AudioClip genericItemSound;
    public AudioClip healthItemSound;
    public AudioClip playerJumpSound;
    public AudioClip playerBounceSound;
    public AudioClip playerChargeSound;

    private Animator animator;                  //Used to store a reference to the Player's animator component.
    string[] animationParameters = {"idle", "run", "hurt", "jump", "charge", "die" };

    public bool canJump = true;
    public bool canCharge = true;
    public bool canMove = true;
    private Vector2 chargeDir; 

    public bool isDead = false;
    public bool isJumping = false;
    public bool isCharging = false;
    public bool isTakingDamage = false;
    public bool onGround;

    private bool jumpKeyWasPressed;
    private bool chargeKeyWasPressed;
    private bool _facingRight;
    
    private int _time = 0;
   // private DateTime startTime = System.DateTime.Now;
   // private DateTime endTime = System.DateTime.Now;       //Nice-to-have, time since start. For timing playthroughs

    public int Time
    {
        get
        {
            return _time;
        }
        set
        {
            _time = value;
            //TODO: remove time
        }
    }

    public int Health
    {
        get
        {
            return playerHealth;
        }
        set
        {
            playerHealth = value;
            UpdateHealthDisplay();
        }
    }
    public bool PlayerFacingRight
    {
        get
        {
            return _facingRight;
        }
        set
        {
            _facingRight = value;
            animator.SetBool("facingRight", _facingRight);
        }
    }

    //Start overrides the Start function of MovingObject
    protected void Start()
    {
        //Get a component reference to the Player's animator component
        animator = GetComponent<Animator>();
        rigidBodyComponent = GetComponent<Rigidbody2D>();
        parallaxComponent = GameObject.Find("Background").GetComponentInChildren<FreeParallax>();

        if (playerCamera == null)
        {
            playerCamera = GetComponent<Camera>();
        }


        PlayerFacingRight = true;
        moveDir = Vector2.zero;
        animator.SetBool("idle", true);
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
        moveDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }


    private void FixedUpdate()
    {
        if (canMove)
        {
            if (!isCharging)
            {
                PlayerFacingRight = (moveDir.x == 0 && PlayerFacingRight)
                                        || (moveDir.x > 0);
                if (moveDir.x != 0) // Player is moving in some direction
                {
                    SwitchAnimation("run");
                }
                else
                {
                    SwitchAnimation("idle");
                }
                Move(new Vector2(moveDir.x * playerSpeed, rigidBodyComponent.velocity.y));
            }
            else
            {
                Charge();
            }

            if (jumpKeyWasPressed && canJump)
            {
                Jump(jumpFactor);                         // This sets jumpKeyPressed to false
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
        }
        if (rigidBodyComponent.position.y < -10)
        {
            GameOver();
        }

        Time += 1;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            canJump = true;
            isJumping = false;
            onGround = true;
            SwitchAnimation("idle");            //TODO: Need to experiment with this
        } else if (collision.gameObject.tag == "BouncePad")
        {
            Jump(bouncePadJumpFactor);
            SoundManager.instance.PlaySingleSoundEffect(playerBounceSound);
        }
    }

    //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
    private void OnTriggerEnter2D(Collider2D other)
    {

        Debug.Log("Colliding with: " + other.name); 
        // Raycast down to see if standing on "something"

        //Check if the tag of the trigger collided with is Exit.
        if (other.tag == "Enemy")
        {
            TakeDamage();
        } else if (other.tag == "Item")
        {
            Destroy(other.gameObject);
            SoundManager.instance.PlaySingleSoundEffect(genericItemSound);
            ApplySpeedBoost();
        }
        else if (other.tag == "HealthItem")
        {
            SoundManager.instance.PlaySingleSoundEffect(healthItemSound);
            Destroy(other.gameObject);
            Health += 1;
        }
    }


    private void Move(Vector2 dir)
    {
        //play move sound
        if (onGround && rigidBodyComponent.velocity.x != dir.x)
        {
            SoundManager.instance.PlayWalkSound(moveSound1);
            dust.Play();
        } 
        else if (rigidBodyComponent.velocity.x == dir.x)
        {
            SoundManager.instance.StopWalkSound();
            dust.Stop();
        }

        UpdateParallax();
        rigidBodyComponent.velocity = dir;
        playerCamera.transform.position = new Vector3(rigidBodyComponent.position.x, (float)(rigidBodyComponent.position.y + 3.5), -10);
    }

    private void Jump(float j)
    {
        jumpKeyWasPressed = false;
        isJumping = true;
        canJump = false;
        onGround = false;
        SwitchAnimation("jump");
        SoundManager.instance.PlaySingleSoundEffect(playerJumpSound);
        Move(new Vector2(rigidBodyComponent.velocity.x, transform.up.y * j));
    }

    private void StartCharge()
    {
        chargeKeyWasPressed = false;
        canCharge = false;
        isCharging = true;
        chargeDir = moveDir;
        SoundManager.instance.PlaySingleSoundEffect(playerChargeSound); //TODO: this may need to go into StartCharge
        Invoke("RemoveChargeBoost", chargeBoostDuration);
        Invoke("RemoveChargeBoostCooldown", chargeBoostCooldown);
    }

    private void Charge()
    {
        SwitchAnimation("charge");
        Move(new Vector2(chargeDir.x * playerSpeed * chargeBoostFactor, chargeDir.y * playerSpeed * chargeBoostFactor));
    }
    private void ApplySpeedBoost()
    {
        playerSpeed *= speedBoostFactor;
        Invoke("RemoveSpeedBoost", speedBoostDuration);
    }
    private void RemoveSpeedBoost()
    {
        playerSpeed /= speedBoostFactor;
    }

    private void RemoveChargeBoost()
    {
        isCharging = false;
    }

    private void RemoveChargeBoostCooldown()
    {
        canCharge = true;
    }

    private void UpdateParallax()
    {
        if (parallaxComponent != null)
        {
            if(animator.GetBool("idle"))
            {
                parallaxComponent.Speed = 0.0f;
            } else
            {
                if (PlayerFacingRight)
                {
                    parallaxComponent.Speed = -parallaxSpeed;
                } else
                {
                    parallaxComponent.Speed = parallaxSpeed;
                }
            }
        }
    }

    private void UpdateHealthDisplay()
    {
        //TODO: Change the health display to have current value of Health
    }

    private void SwitchAnimation(string param)
    {
        if (isJumping && param != "jump")
        {
            return;
        }
        if (isDead && param != "die")
        {
            return;
        }
        if (isTakingDamage)
        {
            return;
        }

        foreach (string s in animationParameters)
        {
            animator.SetBool(s, false);
        }

        animator.SetBool(param, true);
    }


    private void TakeDamage()
    {
        SwitchAnimation("hurt");
        isTakingDamage = true;
        SoundManager.instance.PlaySingleSoundEffect(playerHurtSound);
        Health -= 1;
        if (Health <= 0)
        {
            Invoke("Die", 1f);
        }
    }

    private void DamageAnimationDone()
    {
        isTakingDamage = false;
        animator.SetBool("hurt", false);
        animator.SetBool("idle", true);
    }

    private void Die()
    {
        canMove = false;
        isDead = true;
        SwitchAnimation("die");
        Invoke("GameOver", 2f);                 // TODO: tweak this to allow the death animation to play
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
