using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadybugController : MonoBehaviour
{
    private Animator animatorComponent;
    private Rigidbody2D rigidbodyComponent;
    public AudioClip ladybugDeathSound; 

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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Call Coroutine to dissolve platform
        if (collision.gameObject.tag == "Combine")
        {
            rigidbodyComponent.simulated = false;
            SoundManager.instance.PlaySingleSoundEffect(ladybugDeathSound);
            animatorComponent.SetTrigger("ladybugDead"); //TODO: May need to change this
        }
    }
}
