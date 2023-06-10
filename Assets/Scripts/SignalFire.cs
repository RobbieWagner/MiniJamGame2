using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SignalFire : MonoBehaviour
{

    public static SignalFire Instance {get; private set;}

    [SerializeField] Slider fireLife;
    [SerializeField] float lossMultiplier = 1f;
    [SerializeField] float replenishmentValue = 30f;

    [SerializeField] float secondsToExtinguish;

    bool isPlayerNear;

    [SerializeField] AudioSource fireSound;

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

        isPlayerNear = false;

        fireLife.minValue = 0;
        fireLife.maxValue = secondsToExtinguish;
        fireLife.value = secondsToExtinguish;
    }

    void Update()
    {
        fireLife.value -= Time.deltaTime;

        if(fireLife.value == 0) SceneManager.LoadScene("FireOutGameOver");
    }

    private void OnInteract()
    {
        if(isPlayerNear && GameStats.Instance.Fuel > 0 && fireLife.value < fireLife.maxValue - replenishmentValue/4)
        {
            GameStats.Instance.Fuel--;
            fireLife.value += replenishmentValue;
            if(!fireSound.isPlaying)fireSound.Play();
        }
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        if(other.gameObject.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.gameObject.CompareTag("Player"))
        {
            isPlayerNear = false;
            fireSound.Stop();
        }
    }
}
