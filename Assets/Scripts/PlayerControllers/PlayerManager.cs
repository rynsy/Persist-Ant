using UnityEngine;
using System.Collections;
using UnityEngine.UI;   //Allows us to use UI.
using UnityEngine.SceneManagement;
using System;

//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class PlayerManager :  MonoBehaviour
{
    private const int MAX_HEALTH = 3;

    // Our mortal enemy
    [Header("Level Parameters")]
    [SerializeField] private CombineController combine;
    [SerializeField] private GameObject combineSpawn;
    [SerializeField] private GameObject checkPoint;
    [SerializeField] private GameObject startingPosition;
    [SerializeField] private HealthController healthIndicator;

    [Header("Components")]
    [SerializeField] public ParticleSystem dust;
    [SerializeField] public ParticleSystem blood;
    [SerializeField] private Rigidbody2D rigidBodyComponent;
    
    
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerLocomotion playerLocomotion;
    [SerializeField] private AnimationController animationController;
    
    // Flags
    [Header("Booleans")]
    public bool facingRight;
    public bool canWalkOnSlope;

    public bool canJump = true;
    public bool canCharge = true;
    public bool canMove = true;

    // Exposed for debugging/toggling animations
    public bool isTakingDamage = false;        // Hack to make sure hurt animation plays
    public bool isOnSlope = false;
    public bool isGrounded = true;
    public bool isDead = false;
    public bool isJumping = false;
    public bool isCharging = false;

    // Time, if we want to track how long it takes to beat a level
    private float oldCombineSpeed;
    
    [SerializeField]
    public int playerHealth = 3;
    public int Health
    {
        get
        {
            return playerHealth;
        }
        set
        {
            playerHealth = value;
            healthIndicator.SetHealth(playerHealth);
        }
    }
    private float _time = 0;
    private DateTime startTime = System.DateTime.Now;
    private DateTime endTime = System.DateTime.Now;
    public float PlayTime
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


    void Start()
    {
        // Grab components
        rigidBodyComponent = GetComponent<Rigidbody2D>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        inputManager = GetComponent<InputManager>();
        
        if (GameManager.instance.checkPoint != Vector3.zero)
        {
            transform.position = GameManager.instance.checkPoint;
        } else
        {
            transform.position = startingPosition.transform.position;
        }

        // Set state for game start
        facingRight = true;

    }

    void Restart()
    {
        Health = MAX_HEALTH;
    }

    void Update()
    {
    
    }

    private void FixedUpdate()
    {
        PlayTime += Time.deltaTime;
    }


    public void Move() 
    {
        SoundManager.instance.PlayWalkSound(animationController.moveSound1);
        dust.Play();
    }

    public void StopMoving() 
    {
       SoundManager.instance.StopWalkSound();
       dust.Stop();
    }

    private void TakeDamage()
    {
        isTakingDamage = true;
        animationController.Hurt();
        blood.Play();
        Health -= 1;
        if (Health <= 0)
        {
            Invoke("Die", 0.5f);
        }
        if (!isGrounded)
        {
            isTakingDamage = false;
        }
    }
    private void Die()
    {
        canMove = false;
        isDead = true;
        blood.Play();
        dust.Stop();
        rigidBodyComponent.bodyType = RigidbodyType2D.Static;
        animationController.SwitchAnimation("die");
        Invoke("GameOver", 2f);                 // TODO: tweak this to allow the death animation to play
    }

    private void GameOver()
    {
        GameManager.instance.GameOver();
    }

    #region CollisionAndTrigger

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionEnterOrStay2D(collision);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionEnterOrStay2D(collision);
    }

    private void OnCollisionEnterOrStay2D(Collision2D collision)
    {
        Debug.Log("Colliding with: " + collision.gameObject.name);

        if (collision.gameObject.tag == "BreakBlock")
        {
            if (isCharging)
            {
                collision.gameObject.GetComponent<Rigidbody2D>().drag = 0;
            }

        } else if (collision.gameObject.tag == "BouncePad")
        {
            if ( collision.collider.sharedMaterial != null  
                && collision.collider.sharedMaterial.name == "Full Bounce")
            {
                playerLocomotion.Bounce();
            }
        } else if (collision.gameObject.tag == "Enemy")
        {
            ContactPoint2D contact = collision.GetContact(0);
            if (!isCharging && contact.normal != Vector2.up)
            {
                playerLocomotion.Move(new Vector2(contact.normal.x * 15f, 20f), false);
                TakeDamage();
            }
        } else if (collision.gameObject.tag == "Spike")
        {
            Health = 0;
            Die();
        } 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnTriggerEnterOrStay2D(other);
    }
    private void OnTriggerStay2D(Collider2D other) 
    {
        OnTriggerEnterOrStay2D(other);
    }

    private void OnTriggerEnterOrStay2D(Collider2D other)
    {
        Debug.Log("Triggered by: " + other.name);

        //Check if the tag of the trigger collided with is Exit.
        if (other.tag == "Item")
        {
            SoundManager.instance.PlaySingleSoundEffect(animationController.genericItemSound);
            Destroy(other.gameObject);
            playerLocomotion.ApplySpeedBoost();
        }
        else if (other.tag == "HealthItem")
        {
            SoundManager.instance.PlaySingleSoundEffect(animationController.healthItemSound);
            Destroy(other.gameObject);
            Health += 1;
        } else if (other.tag == "Combine")
        {
            Health = 0;
            TakeDamage();
        } else if (other.tag == "CombineTriggerStop")
        {
            oldCombineSpeed = combine.combineSpeed;
            combine.combineSpeed = 0;
        } else if (other.tag == "CombineTriggerStart")
        {
            combine.transform.position = combineSpawn.transform.position;
            combine.combineSpeed = 1;
        } else if (other.tag == "End")
        {
            Debug.Log("We won!");
            GameManager.instance.WinGame();
        } else if (other.tag == "Checkpoint")
        {
            GameManager.instance.checkPoint = other.gameObject.transform.position;
        }
    }
    #endregion
    
}
