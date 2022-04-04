using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro3 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Start the coroutine we define below named ExampleCoroutine.
        StartCoroutine(ExampleCoroutine());
    }

    IEnumerator ExampleCoroutine()
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(3);

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
        SceneManager.LoadScene(sceneName: "_Dustin_LevelDesign");
    }

    // Update is called once per frame
    void Update()
    {

        //SceneManager.LoadScene(sceneName: "Put the name of the scene here");
    }
}
