using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStats : MonoBehaviour
{
    public static GameStats Instance {get; private set;}

    public int Fuel 
    {
        get {return fuel;}
        set
        {
            fuel = value;
            if(OnFuelSet != null) OnFuelSet(fuel);
        }
    }
    private int fuel;

    public delegate void OnFuelSetDelegate(int fuel);
    public event OnFuelSetDelegate OnFuelSet;


    public TextMeshProUGUI fuelAmountText;

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

        OnFuelSet += UpdateFuelText;
    }

    private void UpdateFuelText(int fuel)
    {
        fuelAmountText.text = "Branches:\n" + fuel.ToString();
    }
}
