using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TimeManger : MonoBehaviour
{

    public Volume ppv;

    public float tick; // Increasing the tick, increases second rate
    public float seconds=0;
    public int mins=0;
    public int hours=0;
    public int days = 1;
    public bool isLights =false;
    public GameObject[] lights;


    void Start()
    {
        if (ppv == null)
        {
            Debug.Log("»щем компонент Volume на текущем объекте...");
            ppv = GetComponent<Volume>();
        }

        if (ppv == null)
        {
            Debug.LogError("Volume не найден!");
        }
        else
        {
            Debug.Log("Volume успешно найден.");
        }
    }

    void FixedUpdate()
    {
        CalcTime();
        //DisplayTime();
    }

    public void CalcTime()
    {
        seconds += Time.fixedDeltaTime *tick;
        if (seconds >= 60) // 60 sec = 1 min
        {
            seconds = 0;
            mins += 1;
        }

        if (mins >= 60) //60 min = 1 hr
        {
            mins = 0;
            hours += 1;
        }

        if (hours >= 24) //24 hr = 1 day
        {
            hours = 0;
            days += 1;
        }
        ControlPPV();
    }

    public void ControlPPV() // used to adjust the post processing slider.
    {
        //ppv.weight = 0;
        if (hours >= 21 && hours < 22) // dusk at 21:00 / 9pm    -   until 22:00 / 10pm
        {
            ppv.weight = (float)mins / 60; // since dusk is 1 hr, we just divide the mins by 60 which will slowly increase from 0 - 1 

            if (isLights == false) // if lights havent been turned on
            {
                if (mins > 45) // wait until pretty dark
                {
                    for (int i = 0; i < lights.Length; i++)
                    {
                        lights[i].SetActive(true); // turn them all on
                    }
                    isLights = true;
                }
            }
        }


        if (hours >= 6 && hours < 7) // Dawn at 6:00 / 6am    -   until 7:00 / 7am
        {
            ppv.weight = 1 - (float)mins / 60; // we minus 1 because we want it to go from 1 - 0
             
            if (isLights == true) // if lights are on
            {
                if (mins > 45) // wait until pretty bright
                {
                    for (int i = 0; i < lights.Length; i++)
                    {
                        lights[i].SetActive(false); // shut them off
                    }
                    isLights = false;
                }
            }
        }
    }
}
