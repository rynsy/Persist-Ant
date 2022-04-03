using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rigidbody;
    public float timeToDie;

    public float bugSpeed = 3f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        rigidbody.velocity = new Vector2(transform.right.x * bugSpeed, rigidbody.velocity.y);
    }
}