using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WormController : MonoBehaviour
{
    private Animator animatorComponent;
    private Rigidbody2D rigidbodyComponent;
    public AudioClip wormDeathSound; 

    void Start()
    {
        animatorComponent = GetComponent<Animator>();
        rigidbodyComponent = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Call Coroutine to dissolve platform
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.transform.position.y > gameObject.transform.position.y + 0.5f)
            {
                SoundManager.instance.PlaySingleSoundEffect(wormDeathSound);
            }
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player.isCharging)
            {
                rigidbodyComponent.simulated = false;
                SoundManager.instance.PlaySingleSoundEffect(wormDeathSound);
                animatorComponent.SetTrigger("die");
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            //arise worm
            animatorComponent.SetTrigger("wiggle");
        }
    }
    public void DestroyWorm()
    {
        Destroy(gameObject);
    }
}