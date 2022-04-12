using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquishyJumpPlayerController : MonoBehaviour
{
    [Header("Horizontal Movement")]
    public float moveSpeed = 10f;
    public Vector2 direction;
    private bool facingRight = true;

    [Header("Vertical Movement")]
    public float jumpSpeed = 15f;
    public float jumpDelay = 0.25f;
    private float jumpTimer;
    
    [Header("Charge Movement")]
    public float chargeSpeed = 15f;
    public float chargeDelay = 0.25f;
    public float chargingMass = 5;
    public float oldMass;
    private float chargeTimer;
    public bool isCharging = false;

    [Header("Components")]
    public Camera playerCamera;
    public Rigidbody2D rb;
    public Animator animator;
    public LayerMask groundLayer;
    public GameObject characterHolder;
    public FreeParallax parallaxComponent;
    [SerializeField] private float parallaxSpeed;
    public HealthController healthIndicator;
    public GameObject checkPoint;

    [Header("Physics")]
    public float maxSpeed = 7f;
    public float linearDrag = 4f;
    public float gravity = 1f;
    public float fallMultiplier = 5f;
    public float speedBoostFactor = 2f;
    public float speedBoostDuration = 10f;

    [Header("Collision")]
    [SerializeField] private bool onGround = false;
    [SerializeField] private float groundLength = 0.6f;
    [SerializeField] private Vector3 colliderOffset;

    [Header("Player")]
    [SerializeField] private int playerHealth = 3;
    [SerializeField] private ParticleSystem dust;
    [SerializeField] private ParticleSystem blood;

    [Header("Sounds")]
    [SerializeField] private AudioClip moveSound1;
    [SerializeField] private AudioClip playerHurtSound;
    [SerializeField] private AudioClip genericItemSound;
    [SerializeField] private AudioClip healthItemSound;
    [SerializeField] private AudioClip playerJumpSound;
    [SerializeField] private AudioClip playerBounceSound;
    [SerializeField] private AudioClip playerChargeSound;

    [Header("Combine")]
    [SerializeField] private CombineController combine;
    [SerializeField] private GameObject combineSpawn;
    [SerializeField] private float oldCombineSpeed;

    [Header("Gameplay")]
    [SerializeField] private bool canMove = true;
    [SerializeField] private GameObject startingPosition;

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

    private void Start()
    {
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
    }

    // Update is called once per frame
    void Update()
    {
        bool wasOnGround = onGround;
        onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);

        if (!wasOnGround && onGround)
        {
            StartCoroutine(JumpSqueeze(1.25f, 0.8f, 0.05f));
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpTimer = Time.time + jumpDelay;
        }
        if (Input.GetButtonDown("Fire1"))
        {
            chargeTimer = Time.time + chargeDelay;
            isCharging = true;
        }
        animator.SetBool("onGround", onGround);
        if (!isCharging)
        {
            direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
    }
    void FixedUpdate()
    {
        if (canMove)
        {
            moveCharacter(direction.x);
            if (jumpTimer > Time.time && onGround)
            {
                Jump();
            }
            if (chargeTimer > Time.time && isCharging)
            {
                Charge();
            }
            modifyPhysics();
        }
    }
    void moveCharacter(float horizontal)
    {
        rb.AddForce(Vector2.right * horizontal * moveSpeed);

        if ((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight))
        {
            Flip();
        }
        if (Mathf.Abs(rb.velocity.x) > 0)
        {
            SoundManager.instance.PlayWalkSound(moveSound1);
            dust.Play();
        }
        if (Mathf.Abs(rb.velocity.x) <= 0.1f)
        {
            SoundManager.instance.StopWalkSound();
            dust.Stop();
        }
        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
        animator.SetFloat("horizontal", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("vertical", rb.velocity.y);
        UpdateCameraPosition();
    }
    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        SoundManager.instance.PlaySingleSoundEffect(playerJumpSound);
        jumpTimer = 0;
        StartCoroutine(JumpSqueeze(0.5f, 1.2f, 0.1f));
    }
    void Charge()
    {
        isCharging = true;
        oldMass = rb.mass;
        rb.mass = chargingMass;
        rb.velocity = new Vector2(0, rb.velocity.y);
        if (facingRight)
        {
            rb.AddForce(Vector2.right * chargeSpeed * chargingMass, ForceMode2D.Impulse);
        } else
        {
            rb.AddForce(-Vector2.right * chargeSpeed * chargingMass, ForceMode2D.Impulse);
        }
        chargeTimer = 0;
        animator.SetTrigger("charge");
        SoundManager.instance.PlaySingleSoundEffect(playerChargeSound);
        StartCoroutine(JumpSqueeze(1.0f, 0.8f, 0.5f));
        Invoke("DoneCharging", chargeDelay);
    }
    private void DoneCharging()
    {
        rb.mass = oldMass;
        isCharging = false;
    }
    void modifyPhysics()
    {
        bool changingDirections = (direction.x > 0 && rb.velocity.x < 0) || (direction.x < 0 && rb.velocity.x > 0);

        if (onGround)
        {
            if (Mathf.Abs(direction.x) < 0.4f || changingDirections)
            {
                rb.drag = linearDrag;
            }
            else
            {
                rb.drag = 0f;
            }
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = gravity;
            rb.drag = linearDrag * 0.15f;
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = gravity * fallMultiplier;
            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.gravityScale = gravity * (fallMultiplier / 2);
            }
        }
    }
    void Flip()
    {
        facingRight = !facingRight;
        animator.SetBool("facingRight", facingRight);
        transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
    }
    IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds)
    {
        Vector3 originalSize = Vector3.one;
        Vector3 newSize = new Vector3(xSqueeze, ySqueeze, originalSize.z);
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            characterHolder.transform.localScale = Vector3.Lerp(originalSize, newSize, t);
            yield return null;
        }
        t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            characterHolder.transform.localScale = Vector3.Lerp(newSize, originalSize, t);
            yield return null;
        }

    }
    private void TakeDamage()
    {
        animator.SetTrigger("hurt");
        SoundManager.instance.PlaySingleSoundEffect(playerHurtSound);
        blood.Play();
        Health -= 1;
        if (Health <= 0)
        {
            Invoke("Die", 1f);
        }
    }
    private void UpdateCameraPosition()
    { 
        Vector2 pos = rb.position;
        Vector3 newCameraPos = new Vector3(pos.x, pos.y + 1.5f, -10.0f);
        playerCamera.transform.position = newCameraPos;
        UpdateParallax();
    }
    private void UpdateParallax()
    {
        if (parallaxComponent != null)
        {
            if (animator.GetFloat("horizontal") == 0.0f)
            {
                parallaxComponent.Speed = 0.0f;
            } else
            {
                if (animator.GetFloat("horizontal") < 0)
                {
                    parallaxComponent.Speed = -parallaxSpeed;
                } else
                {
                    parallaxComponent.Speed = parallaxSpeed;
                }
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Colliding with: " + collision.gameObject.name);

        if (collision.gameObject.tag == "BreakBlock")
        {
            if (isCharging)
            {
                collision.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

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
            animator.SetTrigger("hurt");
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
    private void ApplySpeedBoost()
    {
        maxSpeed *= speedBoostFactor;
        Invoke("RemoveSpeedBoost", speedBoostDuration);
    }
    private void RemoveSpeedBoost()
    {
        maxSpeed /= speedBoostFactor;
    }
    private void Die()
    {
        canMove = false;
        animator.SetTrigger("die");
        Invoke("GameOver", 2f);                 // TODO: tweak this to allow the death animation to play
    }
    private void GameOver()
    {
        GameManager.instance.GameOver();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);
    }
}