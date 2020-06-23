using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayingStatsScript : MonoBehaviour
{
    public RectTransform happinessFill;
    public RectTransform pollutionFill;
    public GameObject industryImage;
    

    public float happiness;
    public float pollution;

    private void OnValidate()
    {
        SetHappiness(happiness);
        SetPollution(pollution);
    }

    public void SetIndustry(bool value){
        if (value)
        {
            industryImage.SetActive(true);
        }
        else
        {
            industryImage.SetActive(false);
        }
    }
    public void SetHappiness(float value)
    {
        happiness = Mathf.Clamp(value, -1f, 1f);
        happinessFill.offsetMin = new Vector2(GameManager.lerp(160, 0, GameManager.smoothstep(0, -1, happiness)), 0);
        happinessFill.offsetMax = new Vector2(GameManager.lerp(-160, 0, GameManager.smoothstep(0, 1, happiness)), 0);
    }

    public void SetPollution(float value)
    {
        pollution = Mathf.Clamp(value, -1f, 1f);
        pollutionFill.offsetMin = new Vector2(GameManager.lerp(160, 0, GameManager.smoothstep(0, -1, pollution)), 0);
        pollutionFill.offsetMax = new Vector2(GameManager.lerp(-160, 0, GameManager.smoothstep(0, 1, pollution)), 0);
    }
}
