using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntLionController : MonoBehaviour
{
    public AudioClip cronchPlayerSound; 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Call Coroutine to dissolve platform
        if (collision.gameObject.tag == "Player")
        {
            SoundManager.instance.PlaySingleSoundEffect(cronchPlayerSound);
        }
    }
}
