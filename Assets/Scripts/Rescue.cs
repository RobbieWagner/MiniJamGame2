using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Rescue : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI text;
    [SerializeField] float secondsToSave;

    // Start is called before the first frame update
    void Start()
    {
        text.text = "Time to Rescue:\n" + FormatTimeString(secondsToSave);
    }

    // Update is called once per frame
    void Update()
    {
        secondsToSave -= Time.deltaTime;
        text.text = "Time to Rescue:\n" + FormatTimeString(secondsToSave);

        if(secondsToSave <= 0) SceneManager.LoadScene("Win");
    }

    private string FormatTimeString(float time)
    {
        int timeInt = (int) time;
        int minutes = timeInt/60;
        int seconds = timeInt % 60;

        if(seconds < 10) return minutes.ToString() + ":0" + seconds.ToString();
        return minutes.ToString() + ":" + seconds.ToString();
    }
}
