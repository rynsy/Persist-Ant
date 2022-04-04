using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineController : MonoBehaviour
{
    public AudioClip startCombineSound; 
    public AudioClip gameloopCombineSound; 
    public AudioClip combineGrindBugSound; 

    public float combineSpeed = 1f;
    public float damp = 0.001f;

    private void FixedUpdate()
    {
        transform.position = new Vector2(transform.position.x + (damp *  combineSpeed), transform.position.y );
    }
}