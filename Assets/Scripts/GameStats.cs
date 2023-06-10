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
    private int fuel = 0;

    public delegate void OnFuelSetDelegate(int fuel);
    public event OnFuelSetDelegate OnFuelSet;

        public int Fibers 
    {
        get {return fibers;}
        set
        {
            fibers = value;
            if(OnFibersSet != null) OnFibersSet(fibers);
        }
    }
    private int fibers = 0;

    public delegate void OnFibersSetDelegate(int fuel);
    public event OnFibersSetDelegate OnFibersSet;

    public TextMeshProUGUI fuelAmountText;
    public TextMeshProUGUI fiberAmountText;

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
        OnFibersSet += UpdateFibersText;
    }

    private void UpdateFuelText(int fuel)
    {
        fuelAmountText.text = "Branches:\n" + fuel.ToString();
    }

    private void UpdateFibersText(int fibers)
    {
        fiberAmountText.text = "Plant Fibers:\n" + fibers.ToString();
    }
}
