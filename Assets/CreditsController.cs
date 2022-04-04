using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsController : MonoBehaviour
{
    private void Start()
    {
        SoundManager.instance.Credits();
    }
}