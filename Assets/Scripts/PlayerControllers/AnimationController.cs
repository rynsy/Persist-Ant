using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    private PlayerManager playerManager;
    string[] animationParameters = {"idle", "run", "hurt", "jump", "charge", "die" };

    [Header("Player  Parameters")]
    public GameObject characterHolder;
    private Vector2 capsuleColliderSize; 
    
    [Header("Sounds")]
    public AudioClip moveSound1;
    public AudioClip genericItemSound;
    public AudioClip healthItemSound;
    public AudioClip playerHurtSound;
    public AudioClip playerJumpSound;
    public AudioClip playerBounceSound;
    public AudioClip playerChargeSound;
    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();        
        animator.SetBool("run", false);
    }
    
    public void SwitchAnimation(string param)
    {
        switch(param)
        {
            case "jump":
                animator.SetTrigger("jump");
                break;
            case "charge":
                animator.SetTrigger("charge");
                break;
            case "hurt":
                animator.SetTrigger("hurt");
                break;
            case "idle":
                animator.SetBool("run", false);
                break;
            case "run":
                animator.SetBool("run", true);
                break;
            case "die":
                animator.SetBool("dead", true);
                break;
            default:
                animator.SetBool("run", false);
                break;
        }
    }
    void Flip()
    {
        playerManager.facingRight = !playerManager.facingRight;
        transform.rotation = Quaternion.Euler(0, playerManager.facingRight ? 0 : 180, 0);
    }

    public void Jump()
    {
        SwitchAnimation("jump");
        SoundManager.instance.PlaySingleSoundEffect(playerJumpSound);
        StartCoroutine(JumpSqueeze(0.7f, 1.2f, 0.1f)); //TODO: Tweak
    }

    public void Bounce()
    {
        SoundManager.instance.PlaySingleSoundEffect(playerBounceSound);
    }
    
    public void Charge()
    {
        SoundManager.instance.PlaySingleSoundEffect(playerChargeSound);
        SwitchAnimation("charge");
        StartCoroutine(JumpSqueeze(1.0f, 0.8f, 0.5f));
    }
    
    public void Ground()
    {
        animator.SetBool("onGround", playerManager.isGrounded);
    }

    public void Hurt()
    {
        SwitchAnimation("hurt");
        SoundManager.instance.PlaySingleSoundEffect(playerHurtSound);
    }
    
    public IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds)
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
    
    // Hack to ensure hurt animation completes. Called from animation event
    public void DamageAnimationDone()
    {
        playerManager.isTakingDamage = false;
        animator.SetBool("hurt", false);
        animator.SetBool("idle", true);
    }
}
