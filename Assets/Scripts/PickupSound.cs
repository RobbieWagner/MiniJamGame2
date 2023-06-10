using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSound : MonoBehaviour
{
    [SerializeField] AudioSource pickupSound;

    public static PickupSound Instance {get; private set;}

    private void Awake() 
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    public void PlaySound()
    {
        pickupSound.Play();
    }
}
