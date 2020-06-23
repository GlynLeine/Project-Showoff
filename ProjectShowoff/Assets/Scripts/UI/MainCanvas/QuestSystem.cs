using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestSystem : MonoBehaviour
{
    private int counterValue = 0;
    private float waitTime = 0;
    private int currentQuest = 0;
    private string timerValue;
    private string rewardPlusMinus;
    private bool English;
    private string dutchReward;
    public GameObject questTutorial;
    public Image questBoxImage;
    public GameObject blockerImage;
    public TMP_Text blockerText;
    public TMP_Text objectives;
    public Image buildingOrDestroy;
    public TMP_Text counterText;
    public Image buildingIcon;
    public TMP_Text forText;
    public Image plusMinusImage;
    public Image rewardImage;
    public TMP_Text timerText;
    public Sprite buildSprite;
    public Sprite destroySprite;
    public Sprite pollutionSprite;
    public Sprite natureSprite;
    public Sprite creatureSprite;
    public Sprite factorySprite;
    public Sprite harborSprite;
    public Sprite mineSprite;
    public Sprite natureReserveSprite;
    public Sprite oilSprite;
    public Sprite solarSprite;
    public Sprite trainSprite;
    public Sprite plusSprite;
    public Sprite minusSprite;

    public enum RewardChoice
    {
        Pollution, Nature, Happiness, Industry, Tutorial
    }
    public enum BuildOrDestroy
    {
        Build, Destroy
    }
    [Serializable]
    public class Quest
    {
        public BuildingType buildingType;
        public bool build;
        public int requiredAmount;
        public int maxTimeInSeconds;
        public int waitTimeInSeconds;
        [Tooltip("Set to true if reward should get added, false if it should get removed. Punishment simply acts as the reverse")]
        public bool addReward;
        [Tooltip("This will be the punishment as well")]
        public RewardChoice reward;
        [HideInInspector] public bool done;
    }
    public Quest[] questList;

    void OnEnable()
    {
        if (LanguageSelector.LanguageSelected == LanguageSelector.LanguageSelectorSelected.English)
        {
            English = true;
        }
        if (English)
        {
            forText.text = "for";
            objectives.text = "Objectives";
        }
        else
        {
            forText.text = "voor";
            objectives.text = "Taken";
        }
        StartCoroutine(QuestQueueSystem());
    }

    private void OnDisable()
    {
        BuildingSystem.onBuildingPlaced -= OnBuildingPlaced;
    }

    IEnumerator QuestQueueSystem()
    {
        for (int i = 0; i < questList.Length; i++)
        {
            questList[i].done = false;
            blockerImage.SetActive(false);
            currentQuest = i;

            counterValue = 0;
            waitTime = questList[i].maxTimeInSeconds;
            counterText.text = "X" + questList[i].requiredAmount;

            if (questList[i].build)
                BuildingSystem.onBuildingPlaced += OnBuildingPlaced;
            else
                BuildingSystem.onBuildingDestroyed += OnBuildingDestroyed;

            switch (questList[i].buildingType)
            {
                case BuildingType.Factory:
                    buildingIcon.sprite = factorySprite;
                    break;
                case BuildingType.Harbor:
                    buildingIcon.sprite = harborSprite;
                    break;
                case BuildingType.CoalMine:
                    buildingIcon.sprite = mineSprite;
                    break;
                case BuildingType.NatureReserve:
                    buildingIcon.sprite = natureReserveSprite;
                    break;
                case BuildingType.OilRig:
                    buildingIcon.sprite = oilSprite;
                    break;
                case BuildingType.SolarFarm:
                    buildingIcon.sprite = solarSprite;
                    break;
                case BuildingType.TrainStation:
                    buildingIcon.sprite = trainSprite;
                    break;
                default:
                    break;
            }

            switch (questList[i].reward)
            {
                case RewardChoice.Pollution:
                    rewardImage.sprite = pollutionSprite;
                    break;
                case RewardChoice.Nature:
                    rewardImage.sprite = natureSprite;
                    break;
                case RewardChoice.Happiness:
                    rewardImage.sprite = creatureSprite;
                    break;
                case RewardChoice.Industry:
                    rewardImage.sprite = buildSprite;
                    break;
                case RewardChoice.Tutorial:
                    rewardImage.sprite = creatureSprite;
                    break;
            }

            buildingOrDestroy.sprite = questList[i].build ? buildSprite : destroySprite;
            plusMinusImage.sprite = questList[i].addReward ? plusSprite : minusSprite;

            StartCoroutine(QuestBoxFlash());

            bool colorFlashDone = false;
            while (waitTime > 0)
            {
                waitTime -= GameManager.deltaTime;

                if (waitTime < 10 && waitTime > 9)
                {
                    if (!colorFlashDone)
                    {
                        StartCoroutine(TextColorFlash());
                        colorFlashDone = true;
                    }
                }
                if (waitTime <= 9)
                {
                    timerValue = "0" + Mathf.Ceil(waitTime);
                }
                else
                {
                    timerValue = Mathf.Ceil(waitTime).ToString();
                }

                timerText.text = "Time Remaining " + timerValue;
                yield return null;
            }

            QuestDone(currentQuest, false);

            yield return new WaitForSeconds(questList[i].waitTimeInSeconds);
        }
    }

    public void QuestDone(int questIndex, bool success)
    {
        if (questList[questIndex].done)
            return;

        waitTime = 0;
        BuildingSystem.onBuildingPlaced -= OnBuildingPlaced;
        BuildingSystem.onBuildingDestroyed -= OnBuildingDestroyed;

        questList[questIndex].done = true;

        blockerText.fontSize = 36;
        blockerImage.SetActive(true);

        if (questList[questIndex].reward == RewardChoice.Tutorial)
        {
            questTutorial.SetActive(false);
            blockerText.fontSize = 28;

            blockerText.text = English ? "Good job! And you can see back how your planet is doing in these bars!" :
                "Goed gedaan! En hoe je planeet het doet kan je in deze balken terugzien!";

            GameManager.happiness += 6;
            return;
        }

        if (questList[questIndex].addReward ^ !success)
            rewardPlusMinus = English ? "increased" : "is gestegen";
        else
            rewardPlusMinus = English ? "decreased" : "is gedaald";

        switch (questList[questIndex].reward)
        {
            case RewardChoice.Happiness:
                dutchReward = "Blijdschap";
                GameManager.happiness += success ? 8 : -6;
                break;
            case RewardChoice.Nature:
                dutchReward = "Natuur";
                GameManager.nature += success ? 50 : -25;
                break;
            case RewardChoice.Industry:
                dutchReward = "Technologie";
                GameManager.industry += success ? 2 : -1;
                break;
            case RewardChoice.Pollution:
                dutchReward = "Vervuiling";
                GameManager.pollution += success ? 100 : -100;
                break;
            default:
                break;
        }

        if (English)
            blockerText.text = (success ? "Success! " : "Oh no! ") + questList[questIndex].reward + " " + rewardPlusMinus + "!";
        else
            blockerText.text = (success ? "Succes! " : "Oh jee! ") + dutchReward + " " + rewardPlusMinus + "!";

    }
    private void OnBuildingPlaced(BuildingLocation location, BuildingPlacer buildingData, Building building)
    {
        if (buildingData.buildingType == questList[currentQuest].buildingType)
        {
            counterValue += 1;
            counterText.text = "X" + (questList[currentQuest].requiredAmount - counterValue);
            if (counterValue >= questList[currentQuest].requiredAmount)
            {
                QuestDone(currentQuest, true);
            }
        }
    }

    private void OnBuildingDestroyed(BuildingLocation location, BuildingPlacer buildingData, Building building)
    {
        if (building.buildingType == questList[currentQuest].buildingType)
        {
            counterValue += 1;
            counterText.text = "X" + (questList[currentQuest].requiredAmount - counterValue);
            if (counterValue >= questList[currentQuest].requiredAmount)
            {
                QuestDone(currentQuest, true);
            }
        }
    }

    private IEnumerator TextColorFlash()
    {
        float timer = 0;
        while (timer < 0.5)
        {
            Color color = timerText.color;
            color.b -= 1f * Time.deltaTime;
            color.g -= 1f * Time.deltaTime;
            timerText.color = color;
            timer += Time.deltaTime;
            yield return null;
        }
        while (timer < 1)
        {
            Color color = timerText.color;
            color.b += 1f * Time.deltaTime;
            color.g += 1f * Time.deltaTime;
            timerText.color = color;
            timer += Time.deltaTime;
            yield return null;
        }
        while (timer < 1.5)
        {
            Color color = timerText.color;
            color.b -= 1f * Time.deltaTime;
            color.g -= 1f * Time.deltaTime;
            timerText.color = color;
            timer += Time.deltaTime;
            yield return null;
        }
        while (timer < 2)
        {
            Color color = timerText.color;
            color.b += 1f * Time.deltaTime;
            color.g += 1f * Time.deltaTime;
            timerText.color = color;
            timer += Time.deltaTime;
            yield return null;
        }
        while (timer < 2.5)
        {
            Color color = timerText.color;
            color.b -= 1f * Time.deltaTime;
            color.g -= 1f * Time.deltaTime;
            timerText.color = color;
            timer += Time.deltaTime;
            yield return null;
        }
        while (timer < 3)
        {
            Color color = timerText.color;
            color.b += 1f * Time.deltaTime;
            color.g += 1f * Time.deltaTime;
            timerText.color = color;
            timer += Time.deltaTime;
            yield return null;
        }
        while (timer < 3.5)
        {
            Color color = timerText.color;
            color.b -= 1f * Time.deltaTime;
            color.g -= 1f * Time.deltaTime;
            timerText.color = color;
            timer += Time.deltaTime;
            yield return null;
        }
        while (timer < 4)
        {
            Color color = timerText.color;
            color.b += 1f * Time.deltaTime;
            color.g += 1f * Time.deltaTime;
            timerText.color = color;
            timer += Time.deltaTime;
            yield return null;
        }
        timerText.color = Color.white;
        yield return null;
    }
    IEnumerator QuestBoxFlash()
    {
        float timer = 0;
        while (timer < 0.5)
        {
            Color color = questBoxImage.color;
            color.b -= 1f * Time.deltaTime;
            questBoxImage.color = color;
            timer += Time.deltaTime;
            yield return null;
        }
        while (timer < 1)
        {
            Color color = questBoxImage.color;
            color.b += 1f * Time.deltaTime;
            questBoxImage.color = color;
            timer += Time.deltaTime;
            yield return null;
        }
        while (timer < 1.5)
        {
            Color color = questBoxImage.color;
            color.b -= 1f * Time.deltaTime;
            questBoxImage.color = color;
            timer += Time.deltaTime;
            yield return null;
        }
        while (timer < 2)
        {
            Color color = questBoxImage.color;
            color.b += 1f * Time.deltaTime;
            questBoxImage.color = color;
            timer += Time.deltaTime;
            yield return null;
        }
        while (timer < 2.5)
        {
            Color color = questBoxImage.color;
            color.b -= 1f * Time.deltaTime;
            questBoxImage.color = color;
            timer += Time.deltaTime;
            yield return null;
        }
        while (timer < 3)
        {
            Color color = questBoxImage.color;
            color.b += 1f * Time.deltaTime;
            questBoxImage.color = color;
            timer += Time.deltaTime;
            yield return null;
        }
        questBoxImage.color = Color.white;
        yield return null; ;
    }
}