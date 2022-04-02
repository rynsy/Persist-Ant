using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadybugController : MonoBehaviour
{
    private Rigidbody2D rigidbody;

    public int bugSpeed = 2;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        rigidbody.velocity = new Vector2(transform.right.x * bugSpeed , rigidbody.velocity.y);
    }
}
