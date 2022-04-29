using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerLocomotion : MonoBehaviour
{
    private const int BOTTOM_OF_WORLD = -20;
    
    private PlayerManager playerManager;
    private AnimationController animationController;
    private Rigidbody2D rigidbody;
    private InputManager inputManager;
    
    [Header("Slope/Collision Parameters")]
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private float slopeCheckDistance;
    [SerializeField] private float maxSlopeAngle;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private PhysicsMaterial2D noFriction;
    [SerializeField] private PhysicsMaterial2D fullFriction;
    [SerializeField] private CapsuleCollider2D capsuleCollider;
    
    private float slopeDownAngle;
    private float slopeSideAngle;
    private float lastSlopeAngle;
    private Vector2 slopeNormalPerp; 
    
    [Header("Player Movement Parameters")]
    [SerializeField] private int playerHealth = 3;
    [SerializeField] private float playerSpeed = 5;
    [SerializeField] private float playerMaxSpeed = 50;

    [SerializeField] private float speedBoostFactor = 2;
    [SerializeField] private float speedBoostDuration = 10f;

    [Header("Player Jumping Parameters")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float playerMaxJumpSpeed = 50;


    [Header("Player Charging Parameters")]
    [SerializeField] private float chargeForce = 1.5f;
    [SerializeField] private float chargingMass;
    [SerializeField] private float chargeBoostDuration = 0.5f;
    [SerializeField] private float chargeBoostCooldown = 5f;
    
    private float oldGravityScale;
    private float oldMass;

    private Vector2 newVelocity; 
    private Vector2 newForce;
    private Vector2 capsuleColliderSize;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        animationController = GetComponent<AnimationController>();
        rigidbody = GetComponent<Rigidbody2D>();
        inputManager = GetComponent<InputManager>();
        
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        capsuleColliderSize = capsuleCollider.size;
    }

    void FixedUpdate()
    {
        StartCoroutine("CheckGround");
        SlopeCheck();
        ApplyMovement();
    }

    #region Movement

    public void Move(Vector2 dir, bool playWalkSound)
    {
        //play move sound
        if (playWalkSound)
        {
            playerManager.Move();
        } 
        else if (rigidbody.velocity.x == dir.x)
        {
            playerManager.StopMoving();
        }

        var x = Mathf.Clamp(dir.x, -playerMaxSpeed, playerMaxSpeed);
        var y = Mathf.Clamp(dir.y, -playerMaxJumpSpeed, playerMaxJumpSpeed);
        rigidbody.velocity = new Vector2(x, y);
    }

    public void Bounce()
    {
        // TODO: Custom bounce physics. 
        animationController.Bounce();
    }
    
    public void Jump()
    {
        if (playerManager.canJump)
        {
            Debug.Log("isJumping = true");
            playerManager.isJumping = true;
            playerManager.isGrounded = false;
            playerManager.canJump = false;
            animationController.Jump();
            newForce.Set(0.0f, jumpForce);
            rigidbody.AddForce(newForce, ForceMode2D.Impulse);
            Debug.Log("Jumping. New Velocity after force is applied: " + rigidbody.velocity);
        }
    }

    public void Charge()
    {
        if (playerManager.canCharge)
        {
            Debug.Log("Charging");
            playerManager.canCharge = false;
            playerManager.isCharging = true;

            newVelocity.Set(0.0f, 0.0f);
            rigidbody.velocity = newVelocity;
            oldGravityScale = rigidbody.gravityScale;
            oldMass = rigidbody.mass;
            rigidbody.gravityScale = 0;
            rigidbody.mass = chargingMass;
            if (playerManager.facingRight)
            {
                rigidbody.AddRelativeForce(Vector2.right * chargeForce * playerSpeed);
            } else
            {
                rigidbody.AddRelativeForce(-Vector2.right * chargeForce * playerSpeed);
            }

            animationController.Charge();
            Invoke("RemoveChargeBoost", chargeBoostDuration);
            Invoke("RemoveChargeBoostCooldown", chargeBoostCooldown);
        }
    }

    public void ApplySpeedBoost()
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
        playerManager.isCharging = false;
        rigidbody.gravityScale = oldGravityScale;
        rigidbody.mass = oldMass;
    }

    private void RemoveChargeBoostCooldown()
    {
        playerManager.canCharge = true;
    }
    
    private void ApplyMovement()
    {
        bool playMoveSound = false;
        if (playerManager.canMove)
        {
            if (playerManager.isGrounded && !playerManager.isOnSlope && !playerManager.isJumping)
            {
                newVelocity.Set(playerSpeed * inputManager.moveDir.x, rigidbody.velocity.y);
                playMoveSound = true;
            } else if (playerManager.isGrounded && playerManager.isOnSlope && playerManager.canWalkOnSlope && !playerManager.isJumping)
            {
                newVelocity.Set(playerSpeed * slopeNormalPerp.x * -inputManager.moveDir.x, playerSpeed * slopeNormalPerp.y * -inputManager.moveDir.x);
                playMoveSound = true;
            } else if (!playerManager.isGrounded || playerManager.isJumping)
            {
                newVelocity.Set(playerSpeed * inputManager.moveDir.x, rigidbody.velocity.y);
                playMoveSound = false;
                animationController.SwitchAnimation("jump");
            }
            if (inputManager.moveDir.x != 0) // Player is moving in some direction
            {
                animationController.SwitchAnimation("run");
            } else
            {
                animationController.SwitchAnimation("idle");
            }
            playMoveSound = playMoveSound && inputManager.moveDir.x != 0; //Ensure sound plays when we want and we're moving

            Move(newVelocity, playMoveSound);
            playerManager.PlayTime += 1; // Don't want to increase this after death
        }

        if (rigidbody.position.y < BOTTOM_OF_WORLD)    //Always check this
        {
            GameManager.instance.GameOver();
        }
    }
    #endregion 

    #region GroundAndSlope

    IEnumerator  CheckGround()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            bool wasOnGround = playerManager.isGrounded;
            playerManager.isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
            animationController.Ground();
            if (!wasOnGround && playerManager.isGrounded)
            {
                StartCoroutine(animationController.JumpSqueeze(1.25f, 0.8f, 0.05f)); // Landing squish
            }
            if (rigidbody.velocity.y <= 0.0f)
            {
                playerManager.isJumping = false;
            }

            if (playerManager.isGrounded && !playerManager.isJumping && slopeDownAngle <= maxSlopeAngle)
            {
                playerManager.canJump = true;
            }
            yield return new WaitForSeconds(0.05f);
        }
        yield return null;
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
            playerManager.isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if (slopeHitBack)
        {
            playerManager.isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0.0f;
            playerManager.isOnSlope = false;
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
                playerManager.isOnSlope = true;
            }
            lastSlopeAngle = slopeDownAngle;

            Debug.DrawRay(hit.point, slopeNormalPerp, Color.blue);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }
        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
            playerManager.canWalkOnSlope = false;
        }
        else
        {
            playerManager.canWalkOnSlope = true;
        }
        if (playerManager.isOnSlope && playerManager.canWalkOnSlope && inputManager.moveDir.x == 0.0f)
        {
            rigidbody.sharedMaterial = fullFriction;
        }
        else
        {
            rigidbody.sharedMaterial = noFriction;
        }
    }
    #endregion
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
