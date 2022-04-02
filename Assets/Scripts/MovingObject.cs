using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour
{
    public float moveTime = 0.001f;			//Time it will take object to move, in seconds.
    public LayerMask blockingLayer;			//Layer on which collision will be checked.
    
    private BoxCollider2D boxCollider; 		//The BoxCollider2D component attached to this object.
    private Rigidbody2D rb2D;				//The Rigidbody2D component attached to this object.
    private float inverseMoveTime;			//Used to make movement more efficient.
    private bool isMoving;					//Is the object currently moving.
    
    
    //Protected, virtual functions can be overridden by inheriting classes.
    protected virtual void Start ()
    {
        boxCollider = GetComponent <BoxCollider2D> ();
        rb2D = GetComponent <Rigidbody2D> ();
        inverseMoveTime = 1f / moveTime;
    }
    
    
    //Move returns true if it is able to move and false if not. 
    //Move takes parameters for x direction, y direction and a RaycastHit2D to check collision.
    protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2 (xDir, yDir);
        
        boxCollider.enabled = false;
        //Cast a line from start point to end point checking collision on blockingLayer.
        hit = Physics2D.Linecast (start, end, blockingLayer);
        //Re-enable boxCollider after linecast
        boxCollider.enabled = true;
        
        //Check if nothing was hit and that the object isn't already moving.
        if(hit.transform == null && !isMoving)
        {
            return true;
        }
        
        return false;
    }
    
    
    
    //The virtual keyword means AttemptMove can be overridden by inheriting classes using the override keyword.
    //AttemptMove takes a generic parameter T to specify the type of component we expect our unit to interact with if blocked (Player for Enemies, Wall for Player).
    protected virtual void AttemptMove <T> (int xDir, int yDir)
        where T : Component
    {
        //Hit will store whatever our linecast hits when Move is called.
        RaycastHit2D hit;
        
        //Set canMove to true if Move was successful, false if failed.
        bool canMove = Move (xDir, yDir, out hit);
        
        //Check if nothing was hit by linecast
        if(hit.transform == null)
            //If nothing was hit, return and don't execute further code.
            return;
        
        //Get a component reference to the component of type T attached to the object that was hit
        T hitComponent = hit.transform.GetComponent <T> ();
        
        //If canMove is false and hitComponent is not equal to null, meaning MovingObject is blocked and has hit something it can interact with.
        if(!canMove && hitComponent != null)
            
            //Call the OnCantMove function and pass it hitComponent as a parameter.
            OnCantMove (hitComponent);
    }
    
    
    //The abstract modifier indicates that the thing being modified has a missing or incomplete implementation.
    //OnCantMove will be overriden by functions in the inheriting classes.
    protected abstract void OnCantMove <T> (T component)
        where T : Component;
}
