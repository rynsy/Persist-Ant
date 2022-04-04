using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootysmackController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Start the coroutine we define below named ExampleCoroutine.
        StartCoroutine(GoToStartMenu());
    }

    IEnumerator GoToStartMenu()
    {
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene(sceneName: "StartMenu");
    }
}
