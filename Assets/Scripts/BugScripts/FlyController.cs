using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyController : MonoBehaviour
{
    private float counter;
    private const float damp = 0.001f;
    private Animator animatorComponent;
    private Rigidbody2D rigidbodyComponent;
    public AudioClip flyDeathSound; 


    public float flySpeed = 0.5f;
    public float bobFactor = 1f;

    // Start is called before the first frame update
    void Start()
    {
        animatorComponent = GetComponent<Animator>();
        rigidbodyComponent = GetComponent<Rigidbody2D>();
        counter = Random.Range(0, 2 * Mathf.PI);
    }
    
    private void FixedUpdate()
    {
        transform.position = new Vector2(transform.position.x + (damp * -1 * flySpeed), transform.position.y + (bobFactor * Mathf.Cos(counter)));
        counter += 0.1f;
        counter = counter % (2 * Mathf.PI);
    }

    // Update is called once per frame
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Call Coroutine to dissolve platform
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.transform.position.y > gameObject.transform.position.y + 0.5f)
            {
                SoundManager.instance.PlaySingleSoundEffect(flyDeathSound);
            }
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player.isCharging)
            {
                rigidbodyComponent.simulated = false;
                SoundManager.instance.PlaySingleSoundEffect(flyDeathSound);
                animatorComponent.SetTrigger("FlyDead");
            }
        } else if (collision.gameObject.tag == "Combine")
        {
            rigidbodyComponent.simulated = false;
            SoundManager.instance.PlaySingleSoundEffect(flyDeathSound);
            animatorComponent.SetTrigger("FlyDead"); //TODO: May need to change this
        }
    }

    public void DestroyFly()
    {
        Destroy(gameObject);
    }
}