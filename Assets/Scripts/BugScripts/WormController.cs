using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WormController : MonoBehaviour
{
    private Animator animatorComponent;
    private Rigidbody2D rigidbodyComponent;

    void Start()
    {
        animatorComponent = GetComponent<Animator>();
        rigidbodyComponent = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Destroy(rigidbodyComponent);
            animatorComponent.SetTrigger("die");
        }
    }
    public void DestroyWorm()
    {
        Destroy(gameObject);
    }

    public void WiggleWorm()
    {
        animatorComponent.SetTrigger("wiggle");
    }
}