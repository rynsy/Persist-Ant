using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroController : MonoBehaviour
{

    private int imageCounter = 0;
    [SerializeField] private GameObject[] introImages;
    [SerializeField] public float transitionDelay;

    private void Awake()
    {
        SoundManager.instance.PlayChillMusic();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DisplayImages());
    }

    IEnumerator DisplayImages()
    {
        while (imageCounter < introImages.Length)
        {
            introImages[imageCounter].SetActive(true);
            imageCounter += 1;
            if (imageCounter == 3)
            {
                SoundManager.instance.StartCombine();
            }
            yield return new WaitForSeconds(transitionDelay);
        }
            yield return new WaitForSeconds(1.8f);
        GameManager.instance.StartLevel1();         
    }
}
