using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailController : MonoBehaviour
{
    private Rigidbody2D rigidbodyComponent;

    public float bugSpeed = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        rigidbodyComponent = GetComponent<Rigidbody2D>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        rigidbodyComponent.velocity = new Vector2(-transform.right.x * bugSpeed , rigidbodyComponent.velocity.y);
    }
}
