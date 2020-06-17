using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCloudEffect : MonoBehaviour
{
    public GameObject cloudEffectPrefab;
    public float effectTime = 5f;

    public static System.Action onEffectFinish;

    private void OnEnable()
    {
        BuildingSystem.onBuildingPlaced += OnBuildingPlaced;
    }

    private void OnDisable()
    {
        BuildingSystem.onBuildingPlaced -= OnBuildingPlaced;
    }

    private IEnumerator DestroyEffect(GameObject effectObject)
    {
        yield return new WaitForSeconds(effectTime);
        onEffectFinish?.Invoke();
        Destroy(effectObject);
    }

    private void OnBuildingPlaced(BuildingLocation location, BuildingPlacer buildingData, Building building)
    {
        GameObject effectObject = Instantiate(cloudEffectPrefab, building.transform);
        effectObject.transform.localPosition = Vector3.up*0.07f;
        StartCoroutine(DestroyEffect(effectObject));
    }
}
