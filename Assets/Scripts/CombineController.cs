using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineController : MonoBehaviour
{
    public float combineSpeed = 3f;
    public float damp = 0.001f;

    private void FixedUpdate()
    {
        transform.position = new Vector2(transform.position.x + (damp *  combineSpeed), transform.position.y );
    }
}