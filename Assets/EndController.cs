using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndController : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        SoundManager.instance.GameEnd();
    }
}
