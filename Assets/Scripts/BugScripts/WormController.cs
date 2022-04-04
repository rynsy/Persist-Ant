using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WormController : MonoBehaviour
{
    private GameObject player;
    private Animator animatorComponent;
    private Rigidbody2D rigidbodyComponent;
    public AudioClip wormDeathSound; 
    public ParticleSystem blood;
    
    public float motionTriggerRadius;

    void Start()
    {
        player = GameObject.Find("Player");
        animatorComponent = GetComponent<Animator>();
        rigidbodyComponent = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        if ((player.transform.position - transform.position).sqrMagnitude < (Mathf.Pow(motionTriggerRadius, 2)))
        {
            animatorComponent.SetTrigger("wiggle");
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // Call Coroutine to dissolve platform
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.transform.position.y > gameObject.transform.position.y + 0.5f)
            {
                SoundManager.instance.PlaySingleSoundEffect(wormDeathSound);
                animatorComponent.SetTrigger("die");
                blood.Play();
            }
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            if (player.isCharging)
            {
                Kill();
            }
        } 
        if (other.gameObject.tag == "Combine")
        {
            Debug.Log("combine worm");
            Kill();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Combine")
        {
            Debug.Log("combine worm");
            Kill();
        }
    }


    public void Kill()
    {
        rigidbodyComponent.simulated = false;
        SoundManager.instance.PlaySingleSoundEffect(wormDeathSound);
        animatorComponent.SetTrigger("die");
        blood.Play();
    }

    public void DestroyWorm()
    {
        Destroy(gameObject);
    }
}