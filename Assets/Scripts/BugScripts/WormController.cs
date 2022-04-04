using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WormController : MonoBehaviour
{
    private GameObject player;
    private Animator animatorComponent;
    private Rigidbody2D rigidbodyComponent;
    public AudioClip wormDeathSound; 
    
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
    public void DestroyWorm()
    {
        Destroy(gameObject);
    }
}