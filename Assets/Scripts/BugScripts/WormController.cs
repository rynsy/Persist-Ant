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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "BreakBlock")
        {
            Kill();
        }
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.transform.position.y > gameObject.transform.position.y + 0.5f)
            {
                SoundManager.instance.PlaySingleSoundEffect(wormDeathSound);
                animatorComponent.SetTrigger("die");
                blood.Play();
            }
            PlayerManager player = collision.gameObject.GetComponent<PlayerManager>();
            if (player.isCharging)
            {
                Kill();
            }
        } 
        if (collision.gameObject.tag == "Combine")
        {
            Debug.Log("combine worm");
            Kill();
        }
        if (collision.gameObject.tag == "Combine")
        {
            Debug.Log("combine worm");
            Kill();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.transform.position.y > gameObject.transform.position.y + 0.5f)
            {
                SoundManager.instance.PlaySingleSoundEffect(wormDeathSound);
                animatorComponent.SetTrigger("die");
                blood.Play();
            }
            PlayerManager player = collision.gameObject.GetComponent<PlayerManager>();
            if (player.isCharging)
            {
                Kill();
            }
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