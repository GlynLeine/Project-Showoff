using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLineSaver : MonoBehaviour
{
    //spring
    public static float _springPollution;
    public static float _springWaterLevel;
    public static float _springBuildings;
    public static float _springNature;
    //summer
    public static float _summerPollution;
    public static float _summerWaterLevel;
    public static float _summerBuildings;
    public static float _summerNature;
    //autumn
    public static float _autumnPollution;
    public static float _autumnWaterLevel;
    public static float _autumnBuildings;
    public static float _autumnNature;
    //winter
    public static float _winterPollution;
    public static float _winterWaterLevel;
    public static float _winterBuildings;
    public static float _winterNature;
    
    void Update()
    {
        if (GameManager.time <= 75)
        {
            _springPollution = GameManager.pollution;
            _springWaterLevel = GameManager.waterLevel;
            _springBuildings = GameManager.buildingsPlaced;
            _springNature = GameManager.nature;
        }
        else if (GameManager.time <= 150)
        {
            _summerPollution = GameManager.pollution;
            _summerWaterLevel = GameManager.waterLevel;
            _summerBuildings = GameManager.buildingsPlaced;
            _summerNature = GameManager.nature;
        }
        else if(GameManager.time <= 225)
        {
            _autumnPollution = GameManager.pollution;
            _autumnWaterLevel = GameManager.waterLevel;
            _autumnBuildings = GameManager.buildingsPlaced;
            _autumnNature = GameManager.nature;
        }
        else if(GameManager.time <= 300)
        {
            _winterPollution = GameManager.pollution;
            _winterWaterLevel = GameManager.waterLevel;
            _winterBuildings = GameManager.buildingsPlaced;
            _winterNature = GameManager.nature;
        }
    }
}
