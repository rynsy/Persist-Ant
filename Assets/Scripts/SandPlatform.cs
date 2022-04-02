using UnityEngine;
using System.Collections;

public class SandPlatform : MonoBehaviour
{
    public AudioClip dissolveSound1; 
    public AudioClip dissolveSound2;

    public int currentSprite = 0;
    private SpriteRenderer spriteRenderer;
    public Sprite[] dissolveSprites;

    public float dissolveTime;    
    public float dissolveDelay;    

    void Awake ()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Call Coroutine to dissolve platform
        if (collision.gameObject.tag == "Player")
        {
            SoundManager.instance.RandomizeSfx (dissolveSound1, dissolveSound2); 
            InvokeRepeating("Dissolve", 0f, 0.5f);
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
