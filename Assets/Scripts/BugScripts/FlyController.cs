using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyController : MonoBehaviour
{
    private float counter;
    private const float damp = 0.001f;


    public float flySpeed = 0.5f;
    public float bobFactor = 1f;

    // Start is called before the first frame update
    void Start()
    {
        counter = Random.Range(0, 2 * Mathf.PI);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        transform.position = new Vector2( transform.position.x + (damp * -1 * flySpeed) , transform.position.y + (bobFactor * Mathf.Cos(counter)));
        counter += 0.1f;
        counter = counter % (2 * Mathf.PI);
    }
}
