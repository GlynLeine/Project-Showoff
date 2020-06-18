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
    private int forLoopInt = 0;
    private string timerValue;
    private string rewardPlusMinus;
    private bool English;
    private string dutchReward;
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
        Pollution,Nature,Happiness,Industry
    }
    public enum BuildOrDestroy
    {
        Build,Destroy
    }
    [Serializable] 
    public class Quest
    {
        public BuildingType buildingType;
        public BuildOrDestroy buildOrDestroy;
        public int buildHowMany;
        public int maxTimeInSeconds;
        public int waitTimeInSeconds;
        [Tooltip("Set to true if reward should get added, false if it should get removed. Punishment simply acts as the reverse")]
        public bool addReward;
        [Tooltip("This will be the punishment as well")]
        public RewardChoice reward;
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
            blockerImage.SetActive(false);
            forLoopInt = i;
            if (questList[i].buildOrDestroy == BuildOrDestroy.Build)
            {
                buildingOrDestroy.sprite = buildSprite;
            }
            else
            {
                buildingOrDestroy.sprite = destroySprite;
                
            }
            counterValue = 0;
            SelectedBuildingType = questList[i].buildingType;
            maxCounterValue = questList[i].buildHowMany;
            waitTime = questList[i].maxTimeInSeconds;
            counterText.text = "X" + (maxCounterValue - counterValue);
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
                buildingIcon.sprite = natureReserveSprite;
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
            if (questList[i].addReward)
            {
                plusMinusImage.sprite = plusSprite;
            }else
            {
                plusMinusImage.sprite = minusSprite;
            }

            if (questList[i].reward == RewardChoice.Pollution)
            {
                rewardImage.sprite = pollutionSprite;
            }else if (questList[i].reward == RewardChoice.Nature)
            {
                rewardImage.sprite = natureSprite;
            }else if (questList[i].reward == RewardChoice.Happiness)
            {
                rewardImage.sprite = creatureSprite;
            }
            StartCoroutine(TempUpdate());
            yield return new WaitForSeconds(waitTime);
            if (counterValue < maxCounterValue)
            {
                QuestDone();
            }
            yield return new WaitForSeconds(questList[i].waitTimeInSeconds);
        }
    }

    public void QuestDone()
    {
        blockerImage.SetActive(true);
        if (counterValue >= maxCounterValue)
        {
            if (questList[forLoopInt].addReward)
            {
                if (English)
                {
                    rewardPlusMinus = "increased";
                }
                else
                {
                    rewardPlusMinus = "is gestegen";
                }
                if (questList[forLoopInt].reward == RewardChoice.Happiness)
                {
                    dutchReward = "Blijdschap";
                    GameManager.happiness += 8;
                }
                else if (questList[forLoopInt].reward == RewardChoice.Nature)
                {
                    dutchReward = "Natuur";
                    GameManager.nature += 50;
                }
                else if (questList[forLoopInt].reward == RewardChoice.Industry){
                 dutchReward = "Technologie";
                 GameManager.industry += 2;
                }
            }
            else
            {
                if (English)
                {
                    rewardPlusMinus = "decreased";
                }
                else
                {
                    rewardPlusMinus = "is gedaald";
                }
                if (questList[forLoopInt].reward == RewardChoice.Pollution)
                {
                    dutchReward = "Vervuiling";
                    GameManager.pollution -= 100;
                }
            }

            if (English)
            {
                blockerText.text = "Success! " + questList[forLoopInt].reward + " " + rewardPlusMinus + "!";
            }
            else
            {
                blockerText.text = "Succes! " + dutchReward + " " + rewardPlusMinus + "!";
            }
        }
        else
        {
            
            if (questList[forLoopInt].addReward)
            {
                if (questList[forLoopInt].reward == RewardChoice.Happiness)
                {
                    dutchReward = "Blijdschap";
                    GameManager.happiness -= 6;
                }
                else if (questList[forLoopInt].reward == RewardChoice.Nature)
                {
                    dutchReward = "Natuur";
                    GameManager.nature -= 25;
                }
                else if (questList[forLoopInt].reward == RewardChoice.Industry){
                 dutchReward = "Technologie";
                 GameManager.industry -= 1;
                }
                if (English)
                {
                    rewardPlusMinus = "decreased";
                }
                else
                {
                    rewardPlusMinus = "is gedaald";
                }
            }
            else
            {
                if (English)
                {
                    rewardPlusMinus = "increased";
                }
                else
                {
                    rewardPlusMinus = "is gestegen";
                }
                if (questList[forLoopInt].reward == RewardChoice.Pollution)
                {
                    dutchReward = "Vervuiling";
                    GameManager.pollution += 100;
                }
            }
            if (English)
            {
                blockerText.text = "Oh no! " + questList[forLoopInt].reward + " " + rewardPlusMinus + "!";
            }
            else
            {
                blockerText.text = "Oh jee! " + dutchReward + " " + rewardPlusMinus + "!";
            }
        }
    }
    private void OnBuildingPlaced(BuildingLocation location, BuildingPlacer buildingData, Building building)
    {
        if (buildingData.buildingType == SelectedBuildingType)
        {
            counterValue += 1;
            counterText.text = "X" + (maxCounterValue - counterValue);
            if (counterValue == maxCounterValue)
            {
                QuestDone();
            }
        }    
    }
    

    IEnumerator TempUpdate()
    {
        bool colorFlashDone = false;
        while (waitTime > 0)
        {
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
            waitTime -= GameManager.deltaTime;
            yield return null;
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
        yield return null;
    }
}