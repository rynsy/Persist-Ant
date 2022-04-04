using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public AudioClip buttonPressSound;

    private void Awake()
    {
        SoundManager.instance.PlayMenuMusic();
    }

    public void PlayButtonAction()
    {
        Debug.Log("PLAY");
        SoundManager.instance.StopMusic();
        SoundManager.instance.PlaySingleSoundEffect(buttonPressSound);
        StartCoroutine(Load("Intro", buttonPressSound.length));
    }
    public void LevelSelectButtonAction()
    {
        Debug.Log("LEVEL SELECT");
        SoundManager.instance.StopMusic();
        SoundManager.instance.PlaySingleSoundEffect(buttonPressSound);
    }
    public void CreditsButtonAction()
    {
        Debug.Log("CREDITS");
        SoundManager.instance.StopMusic();
        SoundManager.instance.PlaySingleSoundEffect(buttonPressSound);
    }
  
    IEnumerator Load(string scene, float delay)
    {
        yield return new WaitForSeconds(delay/2);
        Debug.Log("LOADED");
        GameManager.instance.StartIntro();
    }    

    public void Win()
    {
        //TODO: fancy stuff
    }

    public void Restart()
    {
        SoundManager.instance.PlayChillMusic();
        GameManager.instance.Restart();
    }

    public void GameOver()
    {
        GameObject.Find("GameOverScreen").SetActive(true);
    } 

}
