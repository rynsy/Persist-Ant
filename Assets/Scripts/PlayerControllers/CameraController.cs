using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] public FreeParallax parallaxComponent;
    [SerializeField] private float parallaxSpeed;
    [SerializeField] private Animator animatorComponent;
    [SerializeField] private PlayerManager playerManager;
    
    [SerializeField]
    protected Transform trackingTarget;
    [SerializeField]
    float xOffset;
    [SerializeField]
    float yOffset;
    
    private void Awake()
    {
        parallaxComponent = GameObject.Find("Background").GetComponentInChildren<FreeParallax>();
    }

    private void Update()
    {
       UpdateCameraPosition();
    }

    private void UpdateParallax()
    {
        if (parallaxComponent != null)
        {
            if(animatorComponent.GetBool("idle"))
            {
                parallaxComponent.Speed = 0.0f;
            } else
            {
                if (playerManager.facingRight)
                {
                    parallaxComponent.Speed = -parallaxSpeed;
                } else
                {
                    parallaxComponent.Speed = parallaxSpeed;
                }
            }
        }
    }

    private void UpdateCameraPosition()
    { 
        transform.position = new Vector3(trackingTarget.position.x + xOffset,
            trackingTarget.position.y + yOffset, transform.position.z);
        UpdateParallax();
    }
}
