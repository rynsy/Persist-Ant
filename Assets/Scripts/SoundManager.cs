using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour 
{
    public AudioSource efxSource;					//Drag a reference to the audio source which will play the sound effects.
    public AudioSource chillMusicSource;					//Drag a reference to the audio source which will play the music.
    public AudioSource stressfulMusicSource;					//Drag a reference to the audio source which will play the music.
    public AudioSource gameoverMusicSource;					//Drag a reference to the audio source which will play the music.

    public static SoundManager instance = null;		//Allows other scripts to call functions from SoundManager.				
    public float lowPitchRange = .95f;				//The lowest a sound effect will be randomly pitched.
    public float highPitchRange = 1.05f;			//The highest a sound effect will be randomly pitched.
    
    
    void Awake ()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy (gameObject);
        }
        DontDestroyOnLoad (gameObject);
        
        gameoverMusicSource.loop = false;
        gameoverMusicSource.Stop();
    }
     
    public void EngageStress()
    {
        chillMusicSource.loop = false;
        chillMusicSource.Stop();
        stressfulMusicSource.loop = true;
        stressfulMusicSource.Play();
    }

    public void StopMusic()
    {
        chillMusicSource.loop = false;
        chillMusicSource.Stop();

        stressfulMusicSource.loop = false;
        stressfulMusicSource.Stop();
    }

    public void GameOver()
    {
        StopMusic();
        gameoverMusicSource.loop = false;
        gameoverMusicSource.Play();
    }

    public void PlaySingleSoundEffect(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play ();
    }
    
    //RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
    public void RandomizeSfx (params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        efxSource.pitch = randomPitch;
        efxSource.clip = clips[randomIndex];
        efxSource.Play();
    }
}
