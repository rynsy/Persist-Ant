using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WormController : MonoBehaviour
{
    private Animator animatorComponent;
    private Rigidbody2D rigidbodyComponent;

    void Start()
    {
        animatorComponent = GetComponent<Animator>();
        rigidbodyComponent = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

    }
    public void DestroyWorm()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Call Coroutine to dissolve platform
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.transform.position.y > gameObject.transform.position.y + 0.5f)
            {
                //SoundManager.instance.PlaySingleSoundEffect(snailDeathSound);
                Debug.Log("Worm touch");
            }
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player.isCharging)
            {
                rigidbodyComponent.simulated = false;
                animatorComponent.SetTrigger("die");
                Debug.Log("Worm charge");
            }
        }
    }

    public void WiggleWorm()
    {
        animatorComponent.SetTrigger("wiggle");
    }
}