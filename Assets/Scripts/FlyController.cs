using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyController : MonoBehaviour
{
    private float counter = 0f;
    private Rigidbody2D rigidbody;

    public int flySpeed = 2;

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
        rigidbody.velocity = new Vector2(-transform.right.x * flySpeed , Mathf.Cos(counter));
        counter += 1;
        counter = counter % (2 * Mathf.PI);
    }
}
