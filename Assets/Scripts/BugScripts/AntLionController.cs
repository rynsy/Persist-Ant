using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntLionController : MonoBehaviour
{
    public AudioClip cronchPlayerSound; 
    public AudioClip squishSound; 
    public ParticleSystem blood;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //SoundManager.instance.PlaySingleSoundEffect(cronchPlayerSound);
        } else if (collision.gameObject.tag == "Combine")
        {
            Debug.Log("Combine touched antlion");
            blood.Play();
            Destroy(gameObject, 1f);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Combine")
        {
            Debug.Log("COMBINE TOUCHED LADYBUG");
            SoundManager.instance.PlaySingleSoundEffect(squishSound);
            blood.Play();
        }
    }
}
