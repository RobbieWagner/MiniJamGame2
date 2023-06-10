using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FiberPickUp : MonoBehaviour
{

    private bool canPickUp;

    private void Awake()
    {
        canPickUp = false;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.CompareTag("Player"))
        {
            canPickUp = true;
        }    
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.gameObject.CompareTag("Player"))
        {
            canPickUp = false;
        }        
    }

    private void OnInteract()
    {
        if(canPickUp)
        {
            GameStats.Instance.Fibers++;
            PickupSound.Instance.PlaySound();
            Destroy(this.gameObject);
        }
    }
}
