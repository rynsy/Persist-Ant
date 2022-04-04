using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadybugController : MonoBehaviour
{
    private Animator animatorComponent;
    private Rigidbody2D rigidbodyComponent;
    public AudioClip ladybugDeathSound; 
    public ParticleSystem blood;

    public float bugSpeed = 0f;

    private void Start()
    {
        animatorComponent = GetComponent<Animator>();
        rigidbodyComponent = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        //rigidbodyComponent.velocity = new Vector2(transform.right.x * bugSpeed , rigidbodyComponent.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Combine")
        {
            Debug.Log("COMBINE TOUCHED LADYBUG");
            rigidbodyComponent.simulated = false;
            SoundManager.instance.PlaySingleSoundEffect(ladybugDeathSound);
            animatorComponent.SetTrigger("ladybugDead"); //TODO: May need to change this
            blood.Play();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Combine")
        {
            Debug.Log("COMBINE TOUCHED LADYBUG");
            rigidbodyComponent.simulated = false;
            SoundManager.instance.PlaySingleSoundEffect(ladybugDeathSound);
            animatorComponent.SetTrigger("ladybugDead"); //TODO: May need to change this
            blood.Play();
        }
    }
}
