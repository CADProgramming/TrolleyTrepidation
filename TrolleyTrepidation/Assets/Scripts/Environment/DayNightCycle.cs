using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float time;
    public float fullDayLength;
    public float startTime = 0.4f;
    private float timeRate;

    public Vector3 midDay;

    public GameObject carPark;

    [Header("Sun")]
    public Light sun;
    public Gradient sunColour;
    public AnimationCurve sunIntensity;

    [Header("Moon")]
    public Light moon;
    public Gradient moonColour;
    public AnimationCurve moonIntensity;

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensity;
    public AnimationCurve reflectionIntensity;

    private void Start()
    {
        timeRate = 1.0f / fullDayLength;
        time = startTime;
    }

    private void Update()
    {
        ModifyLightingSettings();
        ToggleSunAndMoon();
        ToggleSceneLighting();
    }

    private void ToggleSceneLighting()
    {
        if (sun.intensity < 0.65f)
        {
            foreach (Transform child in carPark.transform)
            {
                if (child.tag == "Lamp")
                {
                    child.GetChild(0).GetChild(0).gameObject.SetActive(true);
                }
            }
        }
        else if (sun.intensity > 0.65f)
        {
            foreach (Transform child in carPark.transform)
            {
                if (child.tag == "Lamp")
                {
                    child.GetChild(0).GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }

    private void ToggleSunAndMoon()
    {
        if (sun.intensity == 0 && sun.gameObject.activeInHierarchy)
        {
            sun.gameObject.SetActive(false);
        }
        else if (sun.intensity > 0 && !sun.gameObject.activeInHierarchy)
        {
            sun.gameObject.SetActive(true);
        }

        if (moon.intensity == 0 && moon.gameObject.activeInHierarchy)
        {
            moon.gameObject.SetActive(false);
        }
        else if (moon.intensity > 0 && !moon.gameObject.activeInHierarchy)
        {
            moon.gameObject.SetActive(true);
        }
    }

    private void ModifyLightingSettings()
    {
        time += timeRate * Time.deltaTime;
        if (time >= 1.0f)
        {
            time = 0;
        }

        sun.transform.eulerAngles = (time - 0.25f) * midDay * 4.0f;
        moon.transform.eulerAngles = (time - 0.75f) * midDay * 4.0f;

        sun.intensity = sunIntensity.Evaluate(time);
        moon.intensity = moonIntensity.Evaluate(time);

        sun.color = sunColour.Evaluate(time);
        moon.color = moonColour.Evaluate(time);

        RenderSettings.ambientIntensity = lightingIntensity.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensity.Evaluate(time);
    }
}
