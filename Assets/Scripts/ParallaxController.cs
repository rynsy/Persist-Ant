using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{

    public FreeParallax parallax;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (parallax != null)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                parallax.Speed = 15.0f;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                parallax.Speed = -15.0f;
            }
            else
            {
                parallax.Speed = 0.0f;
            }
        }
    }
}
