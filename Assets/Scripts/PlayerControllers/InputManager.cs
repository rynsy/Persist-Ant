using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    
    public Vector2 moveDir; 
    private PlayerManager playerManager;
    private PlayerLocomotion playerLocomotion;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            playerLocomotion.Jump();
        }
        if (Input.GetButtonDown("Fire1"))
        {
            playerLocomotion.Charge();
        }
        moveDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (playerManager.canMove)
        {
            playerManager.facingRight = (moveDir.x == 0 && playerManager.facingRight)
                                    || (moveDir.x > 0);
        }
    }

}
