using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour 
{
    public AudioSource efxSource;					//Drag a reference to the audio source which will play the sound effects.
    public AudioSource walkSoundSource;					//Drag a reference to the audio source which will play the sound effects.
    public AudioSource musicSource;

    public AudioClip mainMenuMusic;
    public AudioClip chillMusic;
    public AudioClip stressMusicIntro;
    public AudioClip stressMusicMainLoop;
    public AudioClip gameOverSound;


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
        
        musicSource.loop = false;
        musicSource.Stop();
    }

    public bool IsMusicPlaying()
    {
        return musicSource.isPlaying;
    }

    public void PlayMenuMusic()
    {
        musicSource.Stop();
        musicSource.loop = true;
        musicSource.clip = mainMenuMusic;
        musicSource.Play();
    }

    public void PlayChillMusic()
    {
        musicSource.Stop();
        musicSource.loop = true;
        musicSource.clip = chillMusic;
        musicSource.Play();
    }
     
    public void EngageStress()
    {
        StopMusic();
        musicSource.clip = stressMusicIntro;
        musicSource.Play();
        StartCoroutine("PlayStressIntro");
    }

    IEnumerator PlayStressIntro()
    {
        while (musicSource.isPlaying)
        {
            yield return null;
        }
        musicSource.Stop();
        musicSource.loop = true;
        musicSource.clip = stressMusicMainLoop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.loop = false;
        musicSource.Stop();
    }

    public void GameOver()
    {
        StopMusic();
        musicSource.PlayOneShot(gameOverSound);
    }

    public void PlaySingleSoundEffect(AudioClip clip)
    {
        efxSource.PlayOneShot(clip, 1f);
    }

    public void PlayWalkSound(AudioClip clip)
    {
        if (!walkSoundSource.isPlaying)
        {
            walkSoundSource.loop = true;
            walkSoundSource.PlayOneShot(clip, 0.5f);
        }
    }

    public void StopWalkSound()
    {
        walkSoundSource.loop = false;
        walkSoundSource.Stop();
    }

    public void StopSoundEffects()
    {
        efxSource.loop = false;
        efxSource.Stop();
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
