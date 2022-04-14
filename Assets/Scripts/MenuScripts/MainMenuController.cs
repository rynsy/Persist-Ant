using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public AudioClip buttonPressSound;
    public bool buttonClicked;


    private void Awake()
    {
        SoundManager.instance.PlayMenuMusic();
        buttonClicked = false;
    }

    public void PlayButtonAction()
    {
        if (!buttonClicked)
        {
            buttonClicked = true;
            Debug.Log("PLAY");
            SoundManager.instance.StopMusic();
            SoundManager.instance.PlaySingleSoundEffect(buttonPressSound);
            StartCoroutine(Load("Intro", buttonPressSound.length));
        }
    }
    public void LevelSelectButtonAction()
    {
        if (!buttonClicked)
        {
            buttonClicked = true;
            Debug.Log("LEVEL SELECT");
            SoundManager.instance.StopMusic();
            SceneManager.LoadScene("LevelSelect");
        }
    }
    public void CreditsButtonAction()
    {
        if (!buttonClicked)
        {
            buttonClicked = true;
            Debug.Log("CREDITS");
            SoundManager.instance.StopMusic();
            SoundManager.instance.Credits();
            SceneManager.LoadScene("Credits");
        }
    }
  
    IEnumerator Load(string scene, float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("LOADED");
        if (scene == "Intro")
        {
            GameManager.instance.StartIntro();
        } else
        {
            SceneManager.LoadScene(scene);
        }
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
