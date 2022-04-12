using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SnailController : MonoBehaviour
{
    private Animator animatorComponent;
    private Rigidbody2D rigidbodyComponent;
    public AudioClip snailDeathSound; 
    public ParticleSystem blood;

    public float bugSpeed = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        animatorComponent = GetComponent<Animator>();
        rigidbodyComponent = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        rigidbodyComponent.velocity = new Vector2(-transform.right.x * bugSpeed, rigidbodyComponent.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "BreakBlock")
        {
            Kill();
        }
        // Call Coroutine to dissolve platform
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.transform.position.y > gameObject.transform.position.y + 0.5f)
            {
                SoundManager.instance.PlaySingleSoundEffect(snailDeathSound);
            }
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player.isCharging)
            {
                rigidbodyComponent.simulated = false;
                SoundManager.instance.PlaySingleSoundEffect(snailDeathSound);
                Kill();
            }
        } else if (collision.gameObject.tag == "Combine")
        {
            rigidbodyComponent.simulated = false;
            SoundManager.instance.PlaySingleSoundEffect(snailDeathSound);
            Kill();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Combine")
        {
            Kill();
        }
    }

    public void Kill()
    {
        animatorComponent.SetTrigger("snailDead");
        SoundManager.instance.PlaySingleSoundEffect(snailDeathSound);
        blood.Play();
    }

    public void DestroySnail()
    {
        Destroy(gameObject);
    }
}