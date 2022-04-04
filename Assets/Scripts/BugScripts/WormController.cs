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


    private void OnTriggerEnter2D(Collider2D other)
    {
        // Call Coroutine to dissolve platform
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.transform.position.y > gameObject.transform.position.y + 0.5f)
            {
                SoundManager.instance.PlaySingleSoundEffect(wormDeathSound);
            }
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
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