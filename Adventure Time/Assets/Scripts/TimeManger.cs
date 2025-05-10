using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class TimeManger : MonoBehaviour
{

    public static event Action<bool> OnNightStateChanged;
    public static event Action OnTimeStop;
    public static event Action OnTimeResume; 

    public Volume ppv;

    public float tick; // Increasing the tick, increases second rate
    public float baseTick;
    public float seconds=0;
    public int mins=0;
    public int hours=0;
    public int days = 1;
    public int timeofDawn = 7;
    public int timeofSunset = 22;

    public bool isLights =false;
    
    public List<GameObject> lights = new List<GameObject>();
    public Image MinImage;
    public Image HourImage;

    private static TimeManger instance;

    private static float hourDeg = 1f / 12f * 360f;
    private static float minDeg = 1f / 60f * 360f;
    private float minRot = 0;
    private float hourRot=0;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ppv = GetComponent<Volume>();
        baseTick = tick;
        minRot = (mins +15) * minDeg;
        hourRot = (hours +1)  * hourDeg;
        UpdateMin(minRot);
        UpdateHour(hourRot);
        //lights[0].SetActive(false);
        isLights = false;
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
            minRot += minDeg;
            if (minRot >= 360)
                minRot -= 360;
            UpdateMin(minRot);
        }

        if (mins >= 60) //60 min = 1 hr
        {
            mins = 0;
            hours += 1;
            hourRot += hourDeg;
            if (hourRot >= 360)
                hourRot -= 360;
            UpdateHour(hourRot);
        }

        if (hours >= 24) //24 hr = 1 day
        {
            hours = 0;
            days += 1;
            PlayerCampfire.Instance.ResetCampfireCount();
        }
        ControlPPV();
    }

    public void ControlPPV() 
    {

        if (hours >= (timeofSunset-1) && hours < timeofSunset) 
        {
            ppv.weight = (float)mins / 60;
            if (isLights == false) 
            {
                if (mins > 45)
                {
                    CleanUpNullLights();
                    foreach (var light in lights)
                    {
                        light.SetActive(true);
                    }
                    isLights = true;
                    OnNightStateChanged?.Invoke(isLights);
                }
                 
            }
        }


        if (hours >= (timeofDawn-1) && hours < timeofDawn)
        {
           
            ppv.weight = 1 - (float)mins / 60;

            
            if (isLights == true)
            {
                if (mins > 45)
                {
                    CleanUpNullLights();
                    foreach (var light in lights)
                    {
                        light.SetActive(false);
                    }
                    isLights = false;
                    OnNightStateChanged?.Invoke(isLights);
                }
                 
            }
        }
    }

    public void UpdateMin(float rot)
    {
        MinImage.transform.rotation = Quaternion.Euler(0, 0, -rot);
    }

    public void UpdateHour(float rot)
    {
        HourImage.transform.rotation = Quaternion.Euler(0, 0, -rot);
    }

    public static TimeManger GetInstance ()
    {
        return instance;
    }

    public void RegisterNewLight(GameObject light)
    {
        lights.Add(light);
        if (isLights)
            light.SetActive(true);
        else
            light.SetActive(false);
    }

    public bool IsNight()
    {
        return isLights;
    }

    public void CleanUpNullLights()
    {
        lights.RemoveAll(light => light == null);
    }

    public void TriggerTimeStop()
    {
        tick = 0;
        OnTimeStop?.Invoke();
    }

    public void TriggerTimeResume()
    {
        tick = baseTick;
        OnTimeResume?.Invoke();
    }
}
