using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadybugController : MonoBehaviour
{
    private Rigidbody2D rigidbodyComponent;

    public float bugSpeed = 1f;

    private void FixedUpdate()
    {
        rigidbodyComponent.velocity = new Vector2(transform.right.x * bugSpeed , rigidbodyComponent.velocity.y);
    }
}
