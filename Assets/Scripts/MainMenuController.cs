using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public AudioClip buttonPressSound;

    public void PlayButtonAction()
    {
        Debug.Log("PLAY");
        SoundManager.instance.StopMusic();
        SoundManager.instance.PlaySingleSoundEffect(buttonPressSound);
        StartCoroutine(Load("DebugScene", buttonPressSound.length));
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
        SceneManager.LoadScene(sceneName: scene);
    }    

}
