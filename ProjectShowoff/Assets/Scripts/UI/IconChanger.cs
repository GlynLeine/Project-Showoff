using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconChanger : MonoBehaviour
{
    public Sprite airPortSprite;
    public Sprite spaceStationSprite;
    public Sprite trainStationSprite;
    private Image thisImage;
    void Start()
    {
        thisImage = gameObject.GetComponent<Image>();
        BuildingSystem.onBuildingPlaced += OnBuildingPlaced;
    }

    private void OnDisable()
    {
        BuildingSystem.onBuildingPlaced -= OnBuildingPlaced;
    }

    private void OnBuildingPlaced(BuildingLocation location, BuildingPlacer buildingData, Building building)
    {
        if (GameManager.industry >= 30)
        {
            thisImage.sprite = spaceStationSprite;
        } else if (GameManager.industry >= 15)
        {
            thisImage.sprite = airPortSprite;
        }
        else
        {
            thisImage.sprite = trainStationSprite;
        }
    }

}
