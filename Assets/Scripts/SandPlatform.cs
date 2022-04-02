using UnityEngine;
using System.Collections;

public class SandPlatform : MonoBehaviour
{
    public AudioClip dissolveSound1; 
    public AudioClip dissolveSound2;

    public float dissolveTime;    

    void Awake ()
    {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Call Coroutine to dissolve platform
    }

    public void Dissolve()
    {
        // We want this platform to dissolve in around the time it takes to play the clip/animation
        SoundManager.instance.RandomizeSfx (dissolveSound1, dissolveSound2); 
    }
}
