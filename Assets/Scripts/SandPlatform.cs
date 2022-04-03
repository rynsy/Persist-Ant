using UnityEngine;
using System.Collections;

public class SandPlatform : MonoBehaviour
{
    public AudioClip dissolveSound1; 
    public AudioClip dissolveSound2;

    public int currentSprite = 0;
    private SpriteRenderer spriteRenderer;
    public Sprite[] dissolveSprites;

    public float dissolveTime = 0.12f;
    public float dissolveDelay = 0f;    

    void Awake ()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Call Coroutine to dissolve platform
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.transform.position.y > gameObject.transform.position.y + 0.5f)
            {
                SoundManager.instance.RandomizeSfx(dissolveSound1, dissolveSound2);
                InvokeRepeating("Dissolve", dissolveDelay, dissolveTime);
            }
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player.isCharging)
            {
                Destroy(gameObject); 
            }
        }
    }

    public void Dissolve()
    {
        if ( currentSprite > dissolveSprites.Length - 1 )
        {
            Destroy(gameObject);
            return;
        }
        spriteRenderer.sprite = dissolveSprites[currentSprite];
        currentSprite += 1;
    }
}
