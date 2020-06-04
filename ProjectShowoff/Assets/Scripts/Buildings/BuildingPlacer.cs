using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class BuildingPlacer : MonoBehaviour
{
    public LocationType locationType;

    public BuildingType buildingType;
    public Button button;

    public float environmentEffect;
    public float pollutionEffect;
    public float industryEffect;

    public float cooldown;
    public float cooldownEffect;

    private float timeBuffer;

    private bool locked
    {
        get
        {
            return !button.interactable;
        }
        set
        {
            button.interactable = !value;
        }
    }

    private BuildingSystem system;

    private void Start()
    {
        system = FindObjectOfType<BuildingSystem>();
    }

    public void Place()
    {
        if (system.PlaceBuilding(this))
        {
            cooldown += cooldownEffect;
            locked = true;
        }
    }

    private void Update()
    {
        if (locked)
        {
            timeBuffer += Time.deltaTime;
            if (timeBuffer >= cooldown)
            {
                timeBuffer -= cooldown;
                locked = false;
            }
        }
    }
}
