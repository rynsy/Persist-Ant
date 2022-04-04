using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public int health = 3;
    [SerializeField] GameObject health1;
    [SerializeField] GameObject health2;
    [SerializeField] GameObject health3;
    public void SetHealth(int h)
    {
        Debug.Log("IN SETHEALTH");
        if (h < 3)
        {
            health3.SetActive(false);
        } else
        {
            health3.SetActive(true);
        }
        if (h < 2)
        {
            health2.SetActive(false);
        } else
        {
            health2.SetActive(true);
        }
        if (h < 1)
        {
            health1.SetActive(false);
        } else
        {
            health1.SetActive(true);
        }
        
    }
    private void AddIconToView()
    {

    }
}
