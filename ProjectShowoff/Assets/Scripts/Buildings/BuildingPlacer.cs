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
        if (system.PlaceBuilding(this))
        {
            GameManager.coolDown += cooldownEffect;
            locked = true;
            StartCoroutine(Unlock());
        }
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
