using UnityEngine;

public class FlyController : MonoBehaviour
{
    private float counter;
    private const float damp = 0.001f;

    public ParticleSystem blood;

    private GameObject player;
    private Animator animatorComponent;
    private Rigidbody2D rigidbodyComponent;
    public AudioClip flyDeathSound;

    public float motionTriggerRadius;
    public float flySpeed = 0.5f;
    public float bobFactor = 1f;
    // ok
    // Start is called before the first frame update
    void Start()
    {
        animatorComponent = GetComponent<Animator>();
        rigidbodyComponent = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        counter = Random.Range(0, 2 * Mathf.PI);
    }
    
    private void FixedUpdate()
    {
        if ((player.transform.position - transform.position).sqrMagnitude < (Mathf.Pow(motionTriggerRadius, 2)))
        {
            transform.position = new Vector2(transform.position.x + (damp * -1 * flySpeed), transform.position.y + (bobFactor * Mathf.Cos(counter)));
            counter += 0.1f;
            counter = counter % (2 * Mathf.PI);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Call Coroutine to dissolve platform
        if (collision.gameObject.tag == "BreakBlock")
        {
            Kill();
        }
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Fly has touched player");
            if (collision.gameObject.transform.position.y > gameObject.transform.position.y + 0.5f)
            {
                SoundManager.instance.PlaySingleSoundEffect(flyDeathSound);
                animatorComponent.SetTrigger("FlyDead");
                blood.Play();
            }
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player.isCharging)
            {
                Kill();
            }
        } else if (collision.gameObject.tag == "Combine")
        {
            Kill();
        }
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Fly has touched player");
            if (collision.gameObject.transform.position.y > gameObject.transform.position.y + 0.5f)
            {
                SoundManager.instance.PlaySingleSoundEffect(flyDeathSound);
                animatorComponent.SetTrigger("FlyDead");
                blood.Play();
            }
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player.isCharging)
            {
                Kill();
            }
        }
    }

    public void Kill()
    {
        rigidbodyComponent.simulated = false;
        SoundManager.instance.PlaySingleSoundEffect(flyDeathSound);
        animatorComponent.SetTrigger("FlyDead");
        blood.Play();
    }

    public void DestroyFly()
    {
        Destroy(gameObject);
    }
}