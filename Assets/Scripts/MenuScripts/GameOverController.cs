using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverController : MonoBehaviour
{
    private void Start()
    {
        SoundManager.instance.GameOver();
    }
}
