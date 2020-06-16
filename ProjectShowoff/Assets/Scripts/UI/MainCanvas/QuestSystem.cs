using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestSystem : MonoBehaviour
{
    private BuildingType SelectedBuildingType;
    private int counterValue = 0;
    private int maxCounterValue;
    private float waitTime = 0;
    private string timerValue;
    public Image buildingIcon;
    public TMP_Text taskText;
    public TMP_Text counterText;
    public TMP_Text timerText;
    public Sprite factorySprite;
    public Sprite harborSprite;
    public Sprite mineSprite;
    public Sprite natureSprite;
    public Sprite oilSprite;
    public Sprite solarSprite;
    public Sprite trainSprite;
    [Serializable] 
    public class Quest
    {
        public BuildingType buildingType;
        public int buildHowMany;
        public int maxTimeInSeconds;
        public int waitTimeInSeconds;
    }
    public Quest[] questList;
    void OnEnable()
    {   
        StartCoroutine(QuestQueueSystem());
        BuildingSystem.onBuildingPlaced += OnBuildingPlaced;
    }

    private void OnDisable()
    {
        BuildingSystem.onBuildingPlaced -= OnBuildingPlaced;
    }

    IEnumerator QuestQueueSystem()
    {
        for (int i = 0; i < questList.Length; i++)
        {
            counterValue = 0;
            SelectedBuildingType = questList[i].buildingType;
            maxCounterValue = questList[i].buildHowMany;
            waitTime = questList[i].maxTimeInSeconds;
            if (questList[i].buildingType == BuildingType.Factory)
            {
                buildingIcon.sprite = factorySprite;
            } else if (questList[i].buildingType == BuildingType.Harbor)
            {
                buildingIcon.sprite = harborSprite;
            } else if (questList[i].buildingType == BuildingType.CoalMine)
            {
                buildingIcon.sprite = mineSprite;
            } else if (questList[i].buildingType == BuildingType.NatureReserve)
            {
                buildingIcon.sprite = natureSprite;
            } else if (questList[i].buildingType == BuildingType.OilRig)
            {
                buildingIcon.sprite = oilSprite;
            } else if (questList[i].buildingType == BuildingType.SolarFarm)
            {
                buildingIcon.sprite = solarSprite;
            } else if (questList[i].buildingType == BuildingType.TrainStation)
            {
                buildingIcon.sprite = trainSprite;
            }
            StartCoroutine(TempUpdate());
            QuestInitialization();
            yield return new WaitForSeconds(waitTime);
            if (counterValue < maxCounterValue)
            {
                taskText.text = "Productivity decreased!";
            }
            yield return new WaitForSeconds(questList[i].waitTimeInSeconds);
        }
    }

    public void QuestInitialization()
    {
        taskText.text = "Build " + maxCounterValue + " " + SelectedBuildingType;
        counterText.text = counterValue + "/" + maxCounterValue;
    }
    private void OnBuildingPlaced(BuildingLocation location, BuildingPlacer buildingData, Building building)
    {
        BuildingChecker(buildingData);
    }

    private void BuildingChecker(BuildingPlacer buildingData)
    {
        if (buildingData.buildingType == SelectedBuildingType)
        {
            counterValue += 1;
            counterText.text = counterValue + "/" + maxCounterValue;
            if (counterValue == maxCounterValue)
            {
                taskText.text = "productivity increased!";
            }
        }
    }

    IEnumerator TempUpdate()
    {
        while (waitTime > 0)
        {
            if (waitTime < 10)
            {
                timerValue = "0" + Mathf.Ceil(waitTime);
            }
            else
            {
                timerValue = Mathf.Ceil(waitTime).ToString();
            }

            timerText.text = "0:" + timerValue;
            waitTime -= GameManager.deltaTime;
            yield return null;
        }
    }
}
