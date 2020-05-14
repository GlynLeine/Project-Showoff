using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSystem : MonoBehaviour
{
    public GameObject characterPrefab;
    public Transform planet;
    public float wanderRange;
    public float walkSpeed;
    public float minWanderTime;
    public float maxWanderTime;
    public float travelChance;

    public void SpawnCharacter(BuildingLocation location)
    {
        Transform parent = planet.Find("Characters");
        if(parent == null)
        {
            parent = new GameObject("Characters").transform;
            parent.parent = planet;
            parent.localPosition = Vector3.zero;
            parent.localRotation = Quaternion.identity;
        }

        Vector2 offset = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(0f, wanderRange);
        Vector3 position = location.transform.position;
        position += location.transform.forward * offset.x + location.transform.right * offset.y;
        GameObject charObject = Instantiate(characterPrefab, position, location.transform.rotation, parent);

        Character character = charObject.GetComponent<Character>();
        character.location = location;
        character.wanderRange = wanderRange;
        character.walkSpeed = walkSpeed;
        character.travelChance = travelChance;
        character.planet = planet;
        character.minWanderTime = minWanderTime;
        character.maxWanderTime = maxWanderTime;
    }
}
