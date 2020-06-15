using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{
    void Start()
    {
        Debug.Log(BuildingSystem.onBuildingPlaced);
    }
    IEnumerator QuestQueueSystem()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
        }
    }
    private void OnBuildingPlaced(BuildingLocation location, BuildingPlacer buildingData, Building building)
    {
    }

    IEnumerator TemporaryUpdate()
    {
        yield return null;
    }
}
