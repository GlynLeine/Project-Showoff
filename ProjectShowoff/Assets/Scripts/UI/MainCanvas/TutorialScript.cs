using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;

public class TutorialScript : MonoBehaviour
{
    //drag all UI elements in + anything else we might need to alter or turn off
    public GameObject zoomSlider;
    public GameObject announcerBox;
    public GameObject timeImages;
    public GameObject reset;
    public GameObject tutorialArrow;
    public GameObject tutorialHand;
    public TMP_Text questBoxTextTMP;
    public GameObject questSystem;
    public GameObject ruralBuildings;
    public GameObject coastalBuildings;
    public Image questBoxImage;
    private Slider slider;
    private float animationSpeed = 1; //reset at end of every animation, makes animations move faster as they go on
    //tutorial step bools so we can check where the player is
    private bool tutorialRotationStep;
    private bool tutorialZoomStep;
    private bool tutorialBuildStep;
    private bool gameHasStarted;
    private bool ifHarborBuilt;
    private bool ifOilRigBuilt;
    private bool tutorialBuildingCheckStep;
    private bool tutorialDestroyStep;
    
    private float tutorialDelaySeconds = 2.5f;//how long you need to wait before you start a tutorial step, potentially redundant
    public bool tutorialSkip;//if active, start does all the tutorial steps right away
    private bool English;//if true, text in english, if false, text is in dutch
    private int buildingCount;//for the tutorial steps, how many buildings have you placed yet?
    private int coastalCount;
    private int ruralCount;
    private float timer;
    private string[] coastalBuildingsArray = {"Harbor","Oil rig"};
    private string[] ruralBuildingsArray = {"Factory","Nature reserve","Coal mine","Train station","Solar farm"};
    private string[] englishCoastalBuildingsArray = {"harbor","oil rig"};
    private string[] englishRuralBuildingsArray = {"factory","nature reserve","coal mine","train station","solar farm"};
    private string[] dutchCoastalBuildingsArray = {"haven","boorplatform"};
    private string[] dutchRuralBuildingsArray = {"fabriek","natuurgebied","koolmijn","trein station","zonnepaneel"};
    
    void Start()
    {
        BuildingSystem.onBuildingPlaced += OnBuildingPlaced;
        slider = zoomSlider.GetComponent<Slider>();
        slider.onValueChanged.AddListener(SliderTutorialChange);
        GameManager.paused = true;
        StartCoroutine(NewsCasterAnimationStart());
        StartCoroutine(HandAnimation());
        if (tutorialSkip)
        {
            //basically sets the delay of the animations to 0 nd just throws em all in at the start
            tutorialDelaySeconds = 0;
            tutorialBuildStep = true;
            tutorialZoomStep = true;
            tutorialRotationStep = true;
            GameManager.paused = false;
            StartCoroutine(TimerAnimationStart());
            StartCoroutine(ResetAnimationStart());
            StartCoroutine(SliderAnimationStart());
            StartCoroutine(HandAnimation());
        }
    }
    //this function gets called when ui slider disabled the main canvas, its the first thing that gets called
    public void OnEnable()
    {
        StartCoroutine(QuestBoxFlash());
        if (LanguageSelector.LanguageSelected == LanguageSelector.LanguageSelectorSelected.English)
        {
            English = true;
        }
        tutorialArrow.SetActive(false);
        tutorialHand.SetActive(false);
        tutorialRotationStep = true;
        foreach (Transform child in ruralBuildings.transform)
        {
            if (child.gameObject.name != "Factory")
            {
                child.gameObject.SetActive(false);
            }
        }

        if (English)
        {
            questBoxTextTMP.text = "Click on the purple dome and place a factory there!";
        } else
        {
            questBoxTextTMP.text = "Klik op de paarse cirkel en plaats daar een fabriek!";
        }

        //StartCoroutine(QuestBoxFlash());
        //basically set all the stuff that needs to be set, set the starting text, etc
    }
    //this gets called when a building is placed 
    private void OnBuildingPlaced(BuildingLocation location, BuildingPlacer buildingData, Building building)
    {
        buildingCount += 1;
        if (buildingCount == 1)
        {
            ruralCount += 1;
            StartCoroutine(BuildingNatureReserveWaiter("Nature reserve"));
            if (English)
            {
                questBoxTextTMP.text = "Good job! Now place a " + englishRuralBuildingsArray[ruralCount] + " to balance the pollution!";
            }
            else
            {
                questBoxTextTMP.text = "Goed gedaan! Plaats nu een " + dutchRuralBuildingsArray[ruralCount] + " om de vervuiling te stoppen!";
            }   
        }
        else if (buildingCount == 2)
        {
            ruralCount += 1;
            StartCoroutine(BuildingNatureReserveWaiter("Coal mine"));
            StartCoroutine(BuildingHarborWaiter("Harbor"));
            if (English)
            {
                questBoxTextTMP.text = "Nice! Try placing a " + englishRuralBuildingsArray[ruralCount] + " this time!";
            }
            else
            {
                questBoxTextTMP.text = "Netjes! Probeer nu is een " + dutchRuralBuildingsArray[ruralCount] + " te plaatsen.";
            }  
        }
        else if (buildingCount == 3)
        {
            if (buildingData.locationType == LocationType.Coastal)
            {
                if (coastalCount < 1)
                {
                    coastalCount += 1;
                }
                StartCoroutine(BuildingHarborWaiter("Oil rig"));
            }
            else if (buildingData.locationType == LocationType.Rural)
            {
                if (ruralCount < 4)
                {
                    ruralCount += 1;
                }
                StartCoroutine(BuildingNatureReserveWaiter("Train station"));
            }
            if (English)
                {
                    questBoxTextTMP.text = "Wow! That smells! Try putting down a " + englishRuralBuildingsArray[ruralCount] + ".";
                }
                else
                {
                    questBoxTextTMP.text = "Wow! Dat stinkt! Probeer is een " + dutchRuralBuildingsArray[ruralCount] + " te plaatsen.";
                }
        }
        else if (buildingCount == 4)
        {
            
            if (buildingData.locationType == LocationType.Coastal)
            {
                if (coastalCount < 1)
                {
                    coastalCount += 1;
                }
                StartCoroutine(BuildingHarborWaiter("Oil rig"));
            }
            else if (buildingData.locationType == LocationType.Rural)
            {
                if (ruralCount < 4)
                {
                    ruralCount += 1;
                }
                StartCoroutine(BuildingNatureReserveWaiter(ruralBuildingsArray[ruralCount]));
            }
            if (English)
            {
                questBoxTextTMP.text = "Good! Now put down a " + englishRuralBuildingsArray[ruralCount] + "!";
            }
            else
            {
                questBoxTextTMP.text = "Goed! Probeer nu is een " + dutchRuralBuildingsArray[ruralCount] + " te plaatsen.";
            }
        }
        else if (buildingCount == 5)
        {
            
            if (buildingData.locationType == LocationType.Coastal)
            {
                if (coastalCount < 1)
                {
                    coastalCount += 1;
                }
                StartCoroutine(BuildingHarborWaiter("Oil rig"));
            }
            else if (buildingData.locationType == LocationType.Rural)
            {
                if (ruralCount < 4)
                {
                    ruralCount += 1;
                }
                StartCoroutine(BuildingNatureReserveWaiter(ruralBuildingsArray[ruralCount]));
            }

            if (coastalCount > 0)
            {
                if (English)
                {
                    questBoxTextTMP.text = "Wow! You already know about coastal buildings? Place a " + englishRuralBuildingsArray[ruralCount] + " instead!";
                }
                else
                {
                    questBoxTextTMP.text = "Wow! Je weet al wat kust gebouwen zijn? Plaats dan maar een " + dutchRuralBuildingsArray[ruralCount] + ".";
                }
            }
            else
            {
                if (English)
                {
                    questBoxTextTMP.text = "We also have coastal buildings, place a " + englishCoastalBuildingsArray[coastalCount] + ".";
                }
                else
                {
                    questBoxTextTMP.text = "We hebben ook kust gebouwen, plaats een " + dutchCoastalBuildingsArray[coastalCount] + ".";
                }
            }
        }
        else if (buildingCount == 6)
        {
            
            if (buildingData.locationType == LocationType.Coastal)
            {
                if (coastalCount < 1)
                {
                    coastalCount += 1;
                }
                StartCoroutine(BuildingHarborWaiter("Oil rig"));
            }
            else if (buildingData.locationType == LocationType.Rural)
            {
                if (ruralCount < 4)
                {
                    ruralCount += 1;
                }
                StartCoroutine(BuildingNatureReserveWaiter(ruralBuildingsArray[ruralCount]));
            }
            if (coastalCount > 0)
            {
                if (English)
                {
                    questBoxTextTMP.text = "Good! Now put down a " + englishRuralBuildingsArray[ruralCount] + "!";
                }
                else
                {
                    questBoxTextTMP.text = "Goed! Probeer nu eens een " + dutchRuralBuildingsArray[ruralCount] + " te plaatsen.";
                }
            }
            else
            {
                if (English)
                {
                    questBoxTextTMP.text = "Almost done! Try placing a "+ englishCoastalBuildingsArray[coastalCount] + ".";
                }
                else
                {
                    questBoxTextTMP.text = "Bijna klaar! Plaats nu eens een " + dutchCoastalBuildingsArray[coastalCount] + ".";
                }
            }
        }
        else if (buildingCount == 7)
        {
            if (English)
            {
                questBoxTextTMP.text = "Press on an existing building now!";
            }
            else
            {
                questBoxTextTMP.text = "Klik nu op een bestaand gebouw!";
            }
            StartCoroutine(SliderAnimationStart());
            StartCoroutine(BuildingActivationWaiter());
        }
        else if (buildingCount >= 8)
        {
            if (English)
            {
                questBoxTextTMP.text = "Good job, you can place what you want now, but you can help us more!.";
            }
            else
            {
                questBoxTextTMP.text = "Goed gedaan, je kan nu doen wat je wilt, maar je kan ooks ons helpen!";
            }
            tutorialBuildingCheckStep = true;
            tutorialDestroyStep = true;
            StartCoroutine(QuestChanger());
            BuildingSystem.onBuildingPlaced -= OnBuildingPlaced;
        }
    }
    //on slider change, this gets called, but since it only needs to be called when the user touches the slider for the first time it also unsubs from the slider
    public void SliderTutorialChange(float value)
    {
        slider.onValueChanged.RemoveListener(SliderTutorialChange);
        tutorialZoomStep = true;
        GameManager.paused = false;
        tutorialBuildStep = true;
        StartCoroutine(TimerAnimationStart());
        StartCoroutine(ResetAnimationStart());
    }
    //gets called when you press a button, keeping it here for now if i need that again so i dont need to go through the hassle of adding all the buttons again
    public void BuildingTutorialButton()
    {
        /*if (!tutorialBuildStep)
        {
            GameManager.paused = false;
            tutorialBuildStep = true;
            StartCoroutine(TimerAnimationStart());
            StartCoroutine(ResetAnimationStart());
        }*/
    }

    public void BuildingCheckTutorial()
    {
        if (!tutorialBuildingCheckStep)
        {
            if (English)
            {
                questBoxTextTMP.text = "Press the icon on the right to destroy the building!”";
            }
            else
            {
                questBoxTextTMP.text = "Klik op de rechtse knop om het gebouw te vernietigen!";
            }
            tutorialBuildingCheckStep = true;
            StartCoroutine(FindNewLocationWaiter());
        }
    }

    IEnumerator FindNewLocationWaiter()
    {
        yield return new WaitForSeconds(6);
        if (!tutorialDestroyStep)
        {
            if (English)
            {
                questBoxTextTMP.text = "Zoom out and look around to find a new place to build!";
            }
            else
            {
                questBoxTextTMP.text = "Zoom uit en kijk rond om een nieuwe plek te vinden om te bouwen!";
            }
            tutorialDestroyStep = true;
        }
    }
    //does what it says on the tin, called when you finish the tutorial to change the tutorial box to the quest system
    IEnumerator QuestChanger()
    {
        yield return new WaitForSeconds(6);
        questBoxTextTMP.gameObject.SetActive(false);
        questSystem.SetActive(true);
    }
    //waits one second before deactivating everything but the game object with the name, so it doesnt happen on screen and ppl dont notice
    IEnumerator BuildingNatureReserveWaiter(string name)
    {
        yield return new WaitForSeconds(1);
        foreach (Transform child in ruralBuildings.transform)
        {
            if (child.gameObject.name != name)
            {
                child.gameObject.SetActive(false);
            }
            else
            {
                child.gameObject.SetActive(true);
            }
        }
    }
    IEnumerator BuildingHarborWaiter(string name)
    {
        yield return new WaitForSeconds(1);
        foreach (Transform child in coastalBuildings.transform)
        {
            if (child.gameObject.name != name)
            {
                child.gameObject.SetActive(false);
            }
            else
            {
                child.gameObject.SetActive(true);
            }
        }
    }
    //same as above, just for activating everything
    IEnumerator BuildingActivationWaiter()
    {
        yield return new WaitForSeconds(1);
        foreach (Transform child in coastalBuildings.transform)
        {
            child.gameObject.SetActive(true);
        }
        foreach (Transform child in ruralBuildings.transform)
        {
            child.gameObject.SetActive(true);
        }
    }
    //making it glow at the start - doesnt work rn? just does it instantly
    IEnumerator QuestBoxFlash()
    {
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
        while (timer < 3.5)
        {
            Color color = questBoxImage.color;
            color.b -= 1f * Time.deltaTime;
            questBoxImage.color = color;
            timer += Time.deltaTime;
            yield return null;
        }
        while (timer < 4)
        {
            Color color = questBoxImage.color;
            color.b += 1f * Time.deltaTime;
            questBoxImage.color = color;
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
    }
    //everything below here is things moving
    IEnumerator SliderAnimationStart()
    {
        while (zoomSlider.transform.localPosition.x > 810)
        {
            Vector3 sliderAnimationPosition = zoomSlider.transform.localPosition;
            animationSpeed += 0.1f;
            sliderAnimationPosition.x -= 360f * animationSpeed * Time.deltaTime;
            if (sliderAnimationPosition.x <= 810)
            {
                animationSpeed = 1;
                tutorialRotationStep = true;
                sliderAnimationPosition.x = 810;
                zoomSlider.transform.localPosition = sliderAnimationPosition;
            }
            else
            {
                zoomSlider.transform.localPosition = sliderAnimationPosition;   
            }
            yield return null;
        }
    }
    IEnumerator TimerAnimationStart()
    {
        yield return new WaitForSeconds(tutorialDelaySeconds);
        while (timeImages.transform.localPosition.y > 470)
        {
            Vector3 timeAnimationPosition;
            timeAnimationPosition = timeImages.transform.localPosition;
            animationSpeed += 0.1f;
            timeAnimationPosition.y -= 360f * animationSpeed * Time.deltaTime;
            if (timeAnimationPosition.y <= 470)
            {
                animationSpeed = 1;
                timeAnimationPosition.y = 470;
                timeImages.transform.localPosition = timeAnimationPosition;
            }
            else
            {
                timeImages.transform.localPosition = timeAnimationPosition;   
            }
            yield return null;
        }
    }
    IEnumerator ResetAnimationStart()
    {
        yield return new WaitForSeconds(tutorialDelaySeconds);
        while (reset.transform.localPosition.x < -810)
        {
            Vector3 destroyAnimationPosition;
            destroyAnimationPosition = reset.transform.localPosition;
            animationSpeed += 0.1f;
            destroyAnimationPosition.x += 360f * animationSpeed * Time.deltaTime;
            if (destroyAnimationPosition.x >= -810)
            {
                animationSpeed = 1;
                destroyAnimationPosition.x = -810;
                reset.transform.localPosition = destroyAnimationPosition;
            }
            else
            {
                reset.transform.localPosition = destroyAnimationPosition;   
            }
            yield return null;
        }
    }
    IEnumerator NewsCasterAnimationStart()
    {
        while(announcerBox.transform.localPosition.x < -810)
        {
            Vector3 newsCasterAnimationPosition;
            newsCasterAnimationPosition = announcerBox.transform.localPosition;
            animationSpeed += 0.1f;
            newsCasterAnimationPosition.x += 360f * animationSpeed * Time.deltaTime;
            if (newsCasterAnimationPosition.x >= -810)
            {
                animationSpeed = 1;
                newsCasterAnimationPosition.x = -810;
                announcerBox.transform.localPosition = newsCasterAnimationPosition; 
            }
            else
            {
                announcerBox.transform.localPosition = newsCasterAnimationPosition;   
            }
            yield return null;
        }
    }
    //animation of the hand popping in, if youve completed the first tutorial step it doesnt activate at all potentially doesnt do anything anymore with the new tutorial
    IEnumerator HandAnimation()
    {
        yield return new WaitForSeconds(10);
        if(!tutorialRotationStep){
            tutorialArrow.SetActive(true);
            tutorialHand.SetActive(true);
            while (tutorialHand.transform.rotation.z > -0.6f)
            {
                tutorialHand.transform.Rotate(0, 0, -60*Time.deltaTime);
                yield return null;
                if (tutorialHand.transform.rotation.z <= -0.6f)
                {
                    tutorialHand.transform.rotation = Quaternion.Euler(0, 0, 15);
                }
            }
        }
    }
}