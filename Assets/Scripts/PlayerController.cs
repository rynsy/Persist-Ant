using UnityEngine;
using System.Collections;
using UnityEngine.UI;   //Allows us to use UI.
using UnityEngine.SceneManagement;
using System;

//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class PlayerController :  MonoBehaviour
{
    private const int BOTTOM_OF_WORLD = -20;
    private const int MAX_HEALTH = 3;

    // Our mortal enemy
    [SerializeField] private CombineController combine;
    [SerializeField] private GameObject combineSpawn;
    [SerializeField] private GameObject checkPoint;
    [SerializeField] private GameObject startingPosition;

    // Components
    public Camera playerCamera;
    public Camera cameraPrefab;
    public ParticleSystem dust;
    public ParticleSystem blood;
    public FreeParallax parallaxComponent;
    private Rigidbody2D rigidBodyComponent;
    private Animator animatorComponent;
    private CapsuleCollider2D capsuleCollider;
    
    // All the states
    string[] animationParameters = {"idle", "run", "hurt", "jump", "charge", "die" };

    // HUD
    [SerializeField] private HealthController healthIndicator;

    // Sound
    public AudioClip moveSound1;
    public AudioClip playerHurtSound;
    public AudioClip genericItemSound;
    public AudioClip healthItemSound;
    public AudioClip playerJumpSound;
    public AudioClip playerBounceSound;
    public AudioClip playerChargeSound;

    // Movement Vectors
    private Vector2 moveDir; 

    // Player Parameters
    [SerializeField] private int playerHealth = 3;
    [SerializeField] private int playerSpeed = 5;
    [SerializeField] private int speedBoostFactor = 2;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float chargeForce = 1.5f;
    [SerializeField] private float speedBoostDuration = 10f;
    [SerializeField] private float chargeBoostDuration = 0.5f;
    [SerializeField] private float chargeBoostCooldown = 5f;
    [SerializeField] private float parallaxSpeed;
    [SerializeField] private float chargingMass;

    // Slope/collision resolution parameters
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private float slopeCheckDistance;
    [SerializeField] private float maxSlopeAngle;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private PhysicsMaterial2D noFriction;
    [SerializeField] private PhysicsMaterial2D fullFriction;

    private float slopeDownAngle;
    private float slopeSideAngle;
    private float lastSlopeAngle;
    private float oldGravityScale;
    private float oldMass;

    private Vector2 newVelocity; 
    private Vector2 newForce; 
    private Vector2 capsuleColliderSize; 
    private Vector2 slopeNormalPerp; 

    // Flags
    private bool _facingRight;
    private bool canWalkOnSlope;

    public bool canJump = true;
    public bool canCharge = true;
    public bool canMove = true;

    // Exposed for debugging/toggling animations
    public bool isTakingDamage;        // Hack to make sure hurt animation plays
    public bool isOnSlope;
    public bool isGrounded;
    public bool isDead;
    public bool isJumping;
    public bool isCharging;

    // Time, if we want to track how long it takes to beat a level
    private int _time = 0;
    private DateTime startTime = System.DateTime.Now;
    private DateTime endTime = System.DateTime.Now;
    private float oldCombineSpeed;

    // Variables that need fancy getters/setters to couple variables to actions
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
            healthIndicator.SetHealth(playerHealth);
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
            animatorComponent.SetBool("facingRight", _facingRight);
        }
    }

    void Start()
    {
        // Grab components
        animatorComponent = GetComponent<Animator>();
        rigidBodyComponent = GetComponent<Rigidbody2D>();
        parallaxComponent = GameObject.Find("Background").GetComponentInChildren<FreeParallax>();
        
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        capsuleColliderSize = capsuleCollider.size;

        if(playerCamera == null)
        {
            playerCamera = Camera.current;
        }
        if (GameManager.instance.checkPoint != Vector3.zero)
        {
            transform.position = GameManager.instance.checkPoint;
        } else
        {
            transform.position = startingPosition.transform.position;
        }

        // Set state for game start
        PlayerFacingRight = true;
        moveDir = Vector2.zero;
        animatorComponent.SetBool("idle", true);
    }

    void Restart()
    {
        Health = MAX_HEALTH;
    }


    void Update()
    {
        CheckInput();
    }

    private void FixedUpdate()
    {
        CheckGround();
        SlopeCheck();
        ApplyMovement();
        UpdateCameraPosition();
    }

    private void ApplyMovement()
    {
        bool playMoveSound = false;
        if (canMove)
        {
            if (isGrounded && !isOnSlope && !isJumping)
            {
                newVelocity.Set(playerSpeed * moveDir.x, rigidBodyComponent.velocity.y);
                playMoveSound = true;
            } else if (isGrounded && isOnSlope && canWalkOnSlope && !isJumping)
            {
                newVelocity.Set(playerSpeed * slopeNormalPerp.x * -moveDir.x, playerSpeed * slopeNormalPerp.y * -moveDir.x);
                playMoveSound = true;
            } else if (!isGrounded | isJumping) //else if (!isGrounded | isJumping)
            {
                newVelocity.Set(playerSpeed * moveDir.x, rigidBodyComponent.velocity.y);
                playMoveSound = false;
                SwitchAnimation("jump");
            }
            if (moveDir.x != 0) // Player is moving in some direction
            {
                SwitchAnimation("run");
            } else
            {
                SwitchAnimation("idle");
            }
            playMoveSound = playMoveSound && moveDir.x != 0; //Ensure sount plays when we want and we're moving

            Move(newVelocity, playMoveSound);
        }

        if (rigidBodyComponent.position.y < BOTTOM_OF_WORLD)    //Always check this
        {
            GameOver();
        }

        Time += 1;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Colliding with: " + collision.gameObject.name);

        if (collision.gameObject.tag == "BreakBlock")
        {
            if (isCharging)
            {
                collision.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

                // TODO: Need to be able to set the whole wall to dynamic. I've tried grouping them into one object and doing the above for all children, but no luck.
/*                var children = collision.gameObject.GetComponentsInChildren<Rigidbody2D>();
                foreach(Rigidbody2D child in children)
                {
                    child.bodyType = RigidbodyType2D.Dynamic;
                }
*/

            }

        } else if (collision.gameObject.tag == "BouncePad")
        {
            if ( collision.collider.sharedMaterial != null  
                && collision.collider.sharedMaterial.name == "Full Bounce")
            {
                SoundManager.instance.PlaySingleSoundEffect(playerBounceSound);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered by: " + other.name);

        //Check if the tag of the trigger collided with is Exit.
        if (other.tag == "Enemy")
        {
            if (!isCharging)
            {
                TakeDamage();
            }
        } else if (other.tag == "Spike")
        {
            Health = 0;
            SwitchAnimation("hurt");
            SoundManager.instance.PlaySingleSoundEffect(playerHurtSound);
            blood.Play();
            Die();
//            TakeDamage();
        } else if (other.tag == "Item")
        {
            SoundManager.instance.PlaySingleSoundEffect(genericItemSound);
            Destroy(other.gameObject);
            ApplySpeedBoost();
        }
        else if (other.tag == "HealthItem")
        {
            SoundManager.instance.PlaySingleSoundEffect(healthItemSound);
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

    private void Move(Vector2 dir, bool playWalkSound)
    {
        //play move sound
        if (playWalkSound)
        {
            SoundManager.instance.PlayWalkSound(moveSound1);
            dust.Play();
        } 
        else if (rigidBodyComponent.velocity.x == dir.x)
        {
            SoundManager.instance.StopWalkSound();
            dust.Stop();
        }

        rigidBodyComponent.velocity = dir;
        UpdateCameraPosition();
    }

    private void CheckInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        if (Input.GetButtonDown("Fire1"))
        {
            Charge();
        }
        moveDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        PlayerFacingRight = (moveDir.x == 0 && PlayerFacingRight)
                                || (moveDir.x > 0);
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        if (rigidBodyComponent.velocity.y <= 0.0f)
        {
            isJumping = false;
        }

        if (isGrounded && !isJumping && slopeDownAngle <= maxSlopeAngle)
        {
            canJump = true;
        }

    }
    private void SlopeCheck()
    {
        Vector2 checkPos = transform.position - (Vector3)(new Vector2(0.0f, capsuleColliderSize.y / 2));

        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, whatIsGround);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, whatIsGround);

        if (slopeHitFront)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if (slopeHitBack)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }

    }


    private void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);

        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeDownAngle != lastSlopeAngle)
            {
                isOnSlope = true;
            }
            lastSlopeAngle = slopeDownAngle;

            Debug.DrawRay(hit.point, slopeNormalPerp, Color.blue);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }
        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
            canWalkOnSlope = false;
        }
        else
        {
            canWalkOnSlope = true;
        }
        if (isOnSlope && canWalkOnSlope && moveDir.x == 0.0f)
        {
            rigidBodyComponent.sharedMaterial = fullFriction;
        }
        else
        {
            rigidBodyComponent.sharedMaterial = noFriction;
        }
    }
    private void Jump()
    {
        if (canJump)
        {
            Debug.Log("isJumping = true");
            isJumping = true;
            isGrounded = false;
            canJump = false;
            SwitchAnimation("jump");
            SoundManager.instance.PlaySingleSoundEffect(playerJumpSound);

            newForce.Set(0.0f, jumpForce);
            rigidBodyComponent.AddForce(newForce, ForceMode2D.Impulse);
            Debug.Log("Jumping. New Velocity after force is applied: " + rigidBodyComponent.velocity);
            UpdateCameraPosition();
        }
    }

    private void Charge()
    {
        if (canCharge)
        {
            Debug.Log("Charging");
            canCharge = false;
            isCharging = true;

            newVelocity.Set(0.0f, 0.0f);
            rigidBodyComponent.velocity = newVelocity;
            oldGravityScale = rigidBodyComponent.gravityScale;
            oldMass = rigidBodyComponent.mass;
            rigidBodyComponent.gravityScale = 0;
            rigidBodyComponent.mass = chargingMass;
            if (PlayerFacingRight)
            {
                rigidBodyComponent.AddRelativeForce(Vector2.right * chargeForce * playerSpeed);
            } else
            {
                rigidBodyComponent.AddRelativeForce(-Vector2.right * chargeForce * playerSpeed);
            }

            SoundManager.instance.PlaySingleSoundEffect(playerChargeSound);
            SwitchAnimation("charge");
            Invoke("RemoveChargeBoost", chargeBoostDuration);
            Invoke("RemoveChargeBoostCooldown", chargeBoostCooldown);
            UpdateCameraPosition();
        }
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
        rigidBodyComponent.gravityScale = oldGravityScale;
        rigidBodyComponent.mass = oldMass;
    }

    private void RemoveChargeBoostCooldown()
    {
        canCharge = true;
    }

    private void UpdateParallax()
    {
        if (parallaxComponent != null)
        {
            if(animatorComponent.GetBool("idle"))
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

    private void UpdateCameraPosition()
    { 
        Vector2 pos = rigidBodyComponent.position;
        Vector3 newCameraPos = new Vector3(pos.x, pos.y + 1.5f, -10.0f);
        playerCamera.transform.position = newCameraPos;
        UpdateParallax();
    }

    // Ensure that boolean values that control states are mutually exclusive
    private void SwitchAnimation(string param)
    {

        if (!isGrounded && ((param == "jump") | (param == "charge")))
        {
            if (isCharging) { param = "charge"; }
            if (isJumping) { param = "jump"; }
            UnsetAllAnimations();
            animatorComponent.SetBool(param, true);
            return;
        }

        //if (!isGrounded && (param != "jump"))
        //{
        //    return;
        //}
        //if (isCharging && (param != "charge"))
        //{
        //    return;
        //}
        //if (isDead && param != "die")
        //{
        //    return;
        //}
        //if (isTakingDamage) // hack to get hurt to work
        //{
        //    return;
        //}

        UnsetAllAnimations();
        animatorComponent.SetBool(param, true);
    }

    private void UnsetAllAnimations()
    {
        foreach (string s in animationParameters)
        {
            animatorComponent.SetBool(s, false);
        }

    }

    private void TakeDamage()
    {
        SwitchAnimation("hurt");
        isTakingDamage = true;
        SoundManager.instance.PlaySingleSoundEffect(playerHurtSound);
        blood.Play();
        Health -= 1;
        if (Health <= 0)
        {
            Invoke("Die", 1f);
        }
        if (!isGrounded)
        {
            isTakingDamage = false;
        }
    }

    // Hack to ensure hurt animation completes. Called from animation event
    private void DamageAnimationDone()
    {
        isTakingDamage = false;
        animatorComponent.SetBool("hurt", false);
        animatorComponent.SetBool("idle", true);
    }

    private void Die()
    {
        canMove = false;
        isDead = true;
        SwitchAnimation("die");
        Invoke("GameOver", 2f);                 // TODO: tweak this to allow the death animation to play
    }

    private void GameOver()
    {
        GameManager.instance.GameOver();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
