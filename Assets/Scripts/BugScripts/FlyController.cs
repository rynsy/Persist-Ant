using UnityEngine;

public class FlyController : MonoBehaviour
{
    private float counter;
    private const float damp = 0.001f;

    private GameObject player;
    private Animator animatorComponent;
    private Rigidbody2D rigidbodyComponent;
    public AudioClip flyDeathSound;

    public float motionTriggerRadius;
    public float flySpeed = 0.5f;
    public float bobFactor = 1f;

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

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Call Coroutine to dissolve platform
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Fly has touched player");
            if (other.gameObject.transform.position.y > gameObject.transform.position.y + 0.5f)
            {
                SoundManager.instance.PlaySingleSoundEffect(flyDeathSound);
                animatorComponent.SetTrigger("FlyDead");
            }
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            if (player.isCharging)
            {
                rigidbodyComponent.simulated = false;
                SoundManager.instance.PlaySingleSoundEffect(flyDeathSound);
                animatorComponent.SetTrigger("FlyDead");
            }
        } else if (other.gameObject.tag == "Combine")
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