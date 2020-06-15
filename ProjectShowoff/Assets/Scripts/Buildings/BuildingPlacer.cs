using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class BuildingPlacer : MonoBehaviour
{
    public LocationType locationType;

    public BuildingType buildingType;
    public Button button;

    public float natureEffect;
    public float pollutionEffect;
    public float industryEffect;

    public float cooldownEffect;

    private static float timeBuffer;

    private static bool locked;

    private BuildingSystem system;

    private void Start()
    {
        locked = false;
        system = FindObjectOfType<BuildingSystem>();
    }

    public void Place()
    {
        system.buildUI.GetType().GetMethod(locationType.ToString() + "Stop").Invoke(system.buildUI, new object[] { });
        system.PlaceBuilding(this);
    }

    IEnumerator Unlock()
    {
        yield return null;
        while (locked)
        {
            timeBuffer += GameManager.deltaTime;
            if (timeBuffer >= GameManager.coolDown)
            {
                timeBuffer -= GameManager.coolDown;
                locked = false;
            }
            yield return null;
        }

    }

    private void Update()
    {
        button.interactable = !locked;
    }
}
