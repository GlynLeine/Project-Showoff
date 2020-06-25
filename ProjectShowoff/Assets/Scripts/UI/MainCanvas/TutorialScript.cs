using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
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
    public Image iconImage;
    public GameObject questSystem;
    public GameObject ruralBuildings;
    public GameObject coastalBuildings;
    public Image questBoxImage;
    public Button destroyButton;
    public BuildingSystem buildingSystem;
    public Image infoIcon;
    private Slider slider;
    private float animationSpeed = 1; //reset at end of every animation, makes animations move faster as they go on
    //tutorial step bools so we can check where the player is
    private bool tutorialRotationStep;
    private bool tutorialZoomStep;
    public bool tutorialBuildStep;//this is an old bool that doesnt do anything anymore and im only really using it to make the editor easier to look at
    private bool tutorialBuildingCheckStep;
    private bool tutorialDestroyStep;
    private bool tutorialBoxFlash;

    public Sprite zoomIcon;
    public Sprite creatureHead;
    public Sprite buildingIcon;
    public Sprite destroyIcon;
    public Sprite factorySprite;
    public Sprite harborSprite;
    public Sprite mineSprite;
    public Sprite natureReserveSprite;
    public Sprite oilSprite;
    public Sprite solarSprite;
    public Sprite trainSprite;
    public Sprite emptyButton;
    
    private float tutorialDelaySeconds = 2.5f;//how long you need to wait before you start a tutorial step, potentially redundant
    public bool tutorialSkip;//if active, start does all the tutorial steps right away
    private bool English;//if true, text in english, if false, text is in dutch
    private int buildingCount;//for the tutorial steps, how many buildings have you placed yet?
    private int coastalCount;
    private int ruralCount;
    private bool infoIconPlaying;
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
        GameManager.paused = true;
        StartCoroutine(NewsCasterAnimationStart());
        StartCoroutine(HandAnimation());
        if (tutorialSkip)
        {
            //basically sets the delay of the animations to 0 nd just throws em all in at the start
            tutorialDelaySeconds = 0;
            tutorialZoomStep = true;
            tutorialRotationStep = true;
            GameManager.paused = false;
            tutorialBuildingCheckStep = true;
            tutorialDestroyStep = true;
            StartCoroutine(QuestChanger());
            StartCoroutine(TimerAnimationStart());
            StartCoroutine(ResetAnimationStart());
            StartCoroutine(SliderAnimationStart());
            buildingSystem.enableNewContinents = true;
            buildingSystem.enableCoasts = true;
            buildingSystem.enableStatsAndDestroy = true;
            BuildingSystem.onBuildingPlaced -= OnBuildingPlaced;

        }
    }

    private void OnDisable()
    {
        BuildingSystem.onBuildingPlaced -= OnBuildingPlaced;
    }

    //this function gets called when ui slider disabled the main canvas, its the first thing that gets called
    public void OnEnable()
    {
        StartCoroutine(QuestBoxFlash());
        if (LanguageSelector.LanguageSelected == LanguageSelector.LanguageSelectorSelected.English)
        {
            English = true;
        }
        if (!tutorialSkip)
        {
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
            iconImage.sprite = factorySprite;
        }

        //StartCoroutine(QuestBoxFlash());
        //basically set all the stuff that needs to be set, set the starting text, etc
    }
    //this gets called when a building is placed 
    private void OnBuildingPlaced(BuildingLocation location, BuildingPlacer buildingData, Building building)
    {
        //if youve destroyed a building, activate zoom step, else, check what building has just been placed to see what step theyre on
        if (tutorialDestroyStep)
        {
            if (English)
            {
                questBoxTextTMP.text = "Zoom out and look around to find a new place to build!";
            }
            else
            {
                questBoxTextTMP.text = "Zoom uit en kijk rond om een nieuwe plek te vinden!";
            }
            slider.onValueChanged.AddListener(SliderTutorialChange);
            StartCoroutine(SliderAnimationStart());
            iconImage.sprite = zoomIcon;
            BuildingSystem.onBuildingPlaced -= OnBuildingPlaced;
        }
        else if (buildingData.buildingType == BuildingType.Factory)
        {
            {
                StartCoroutine(BuildingNatureReserveWaiter(ruralBuildingsArray[1]));
                if (English)
                {
                    questBoxTextTMP.text = "Now place a " + englishRuralBuildingsArray[1] + " to balance the pollution!";
                }
                else
                {
                    questBoxTextTMP.text = "Plaats nu een " + dutchRuralBuildingsArray[1] + " om vervuiling te stoppen!";
                }
                iconImage.sprite = natureReserveSprite;
                StartCoroutine(QuestBoxFlash());
            }
        }
        else if (buildingData.buildingType == BuildingType.NatureReserve)
        {
            StartCoroutine(BuildingNatureReserveWaiter(ruralBuildingsArray[2]));
            StartCoroutine(BuildingHarborWaiter(""));
            if (English)
            {
                questBoxTextTMP.text = "Nice! Try placing a " + englishRuralBuildingsArray[2] + " this time!";
            }
            else
            {
                questBoxTextTMP.text = "Netjes! Probeer nu eens een " + dutchRuralBuildingsArray[2] + " te bouwen.";
            }
            iconImage.sprite = mineSprite;
            StartCoroutine(QuestBoxFlash());
        }
        else if (buildingData.buildingType == BuildingType.CoalMine)
        {
            StartCoroutine(BuildingNatureReserveWaiter(ruralBuildingsArray[3]));
            if (English)
            {
                questBoxTextTMP.text = "That smells! Try putting down a " + englishRuralBuildingsArray[3] + ".";
            }
            else
            {
                questBoxTextTMP.text = "Dat stinkt! Probeer eens een " + dutchRuralBuildingsArray[3] + " te plaatsen.";
            }
            iconImage.sprite = trainSprite;
            StartCoroutine(QuestBoxFlash());
        }
        else if (buildingData.buildingType == BuildingType.TrainStation)
        {
            StartCoroutine(BuildingNatureReserveWaiter(ruralBuildingsArray[4]));
            if (English)
            {
                questBoxTextTMP.text = "Good! Now put down a " + englishRuralBuildingsArray[4] + "!";
            }
            else
            {
                questBoxTextTMP.text = "Heel goed! Kun je ook een " + dutchRuralBuildingsArray[4] + " plaatsen?";
            }
            iconImage.sprite = solarSprite;
            StartCoroutine(QuestBoxFlash());
        }
        else if (buildingData.buildingType == BuildingType.SolarFarm)
        {
            buildingSystem.enableCoasts = true;
            StartCoroutine(BuildingHarborWaiter(coastalBuildingsArray[0]));
            if (English)
            {
                questBoxTextTMP.text = "We also have coastal buildings, place a " + englishCoastalBuildingsArray[0] + ".";
            }
            else
            { 
                questBoxTextTMP.text = "We hebben ook gebouwen voor de kust, bouw een " + dutchCoastalBuildingsArray[0] + ".";
            }
            iconImage.sprite = harborSprite;
            StartCoroutine(QuestBoxFlash());
        }
        else if (buildingData.buildingType == BuildingType.Harbor)
        {
            StartCoroutine(BuildingHarborWaiter(coastalBuildingsArray[1]));
            if (English)
            {
                questBoxTextTMP.text = "Almost done! Try placing a "+ englishCoastalBuildingsArray[1] + ".";
            }
            else
            {
                questBoxTextTMP.text = "Bijna klaar! Plaats nu een " + dutchCoastalBuildingsArray[1] + ".";
            }
            iconImage.sprite = oilSprite;
            StartCoroutine(QuestBoxFlash());
        }
        else if (buildingData.buildingType == BuildingType.OilRig)
        {
            if (English)
            {
                questBoxTextTMP.text = "Press on an existing building now!";
            }
            else
            {
                questBoxTextTMP.text = "Klik op een bestaand gebouw om meer informatie te zien!";
            }
            tutorialBuildStep = true;
            buildingSystem.enableStatsAndDestroy = true;
            iconImage.sprite = buildingIcon;
            StartCoroutine(QuestBoxFlash());
            StartCoroutine(BuildingActivationWaiter());
        }
    }

    //this function gets called in the script that pops up the building data, its the first step post-building buidlings
    public void BuildingCheckTutorial()
    {
        if (!tutorialBuildingCheckStep)
        {
            if (English)
            {
                questBoxTextTMP.text = "Press the icon on the right to destroy the building!";
            }
            else
            {
                questBoxTextTMP.text = "Klik op de knop rechts om het gebouw te vernietigen!";
            }
            StartCoroutine(QuestBoxFlash());
            tutorialBuildingCheckStep = true;
            iconImage.sprite = destroyIcon;
            destroyButton.onClick.AddListener(OnButtonPress);
        }
    }

    //gets called when you press the destroy button aka when you destroy a building
    public void OnButtonPress()
    {
        if (English)
        {
            questBoxTextTMP.text = "Place a new building on the destroyed spot now!";
        }
        else
        {
            questBoxTextTMP.text = "Plaats nu een nieuw gebouw op de oude plek!";
        }
        StartCoroutine(QuestBoxFlash());
        tutorialDestroyStep = true;
        iconImage.sprite = buildingIcon;
        destroyButton.onClick.RemoveListener(OnButtonPress);
    }
    
    //gets called when you touch the slider, aka when you zoom
    public void SliderTutorialChange(float value)
    {
        if (!tutorialZoomStep)
        {
            tutorialZoomStep = true;
            GameManager.paused = false;
            //basically this is the end of the tutorial, so it activates all the coroutines to move everything else into place and activate the rest of the game
            StartCoroutine(TimerAnimationStart());
            StartCoroutine(zoomWaiter());
            //zoomwaiter = the text change, we wait one second before we confirm that you've done the tutorial, to give you a moment to zoom in and out
            StartCoroutine(ResetAnimationStart());
            buildingSystem.enableNewContinents = true;
            tutorialZoomStep = true;
        }
        slider.onValueChanged.RemoveListener(SliderTutorialChange);
    }
    
    //does what it says on the tin, called when you finish the tutorial to change the tutorial box to the quest system
    IEnumerator QuestChanger()
    {
        yield return new WaitForSeconds(6);
        questBoxTextTMP.gameObject.SetActive(false);
        iconImage.gameObject.SetActive(false);
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
            if (child.gameObject.name == name)
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }
    //same as above, just for activating everything once the building tutorial section is done
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
    //the zoom text disappeared just a bit too quickly, so we wait one sec before changing it
    IEnumerator zoomWaiter()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(QuestBoxFlash());
        //flash is here because it should flash when it changes
        if (English)
        {
            questBoxTextTMP.text = "At the top you can see the timer, both as a countdown and in the seasons! When it runs out, the game ends.";
        }
        else
        {
            questBoxTextTMP.text = "Aan de bovenkant zie je de timer! Aftellend en als seizoenen. Zodra het op nul staat eindigt het spel!";
        }
        iconImage.sprite = emptyButton;
        StartCoroutine(HelpForgotten());
    }

    IEnumerator HelpForgotten()
    {
        yield return new WaitForSeconds(6);
        if (English)
        {
            questBoxTextTMP.text = "If you need any refreshers on the buildings, just click on the info on the left! This pauses the game.";
        }
        else
        {
            questBoxTextTMP.text = "Als je iets vergeten bent, klik dan op de info knop aan de linkerkant! Dit pauzeert ook het spel.";
        }
        StartCoroutine(InfoIconFlash());
        StartCoroutine(PostTimer());
    }
    IEnumerator PostTimer()
    {
        yield return new WaitForSeconds(10);
        if (English)
        {
            questBoxTextTMP.text = "We also have more tasks! Just build or destroy the building mentioned, and we'll give you a reward!";
        }
        else
        {
            questBoxTextTMP.text = "We hebben nog meer taken! Bouw of vernietig de gebouwen die we laten zien, en je krijgt een beloning!";
        }
        StartCoroutine(QuestChanger());
    }
    IEnumerator InfoIconFlash()
    {
        if (!infoIconPlaying)
        {
            infoIconPlaying = true;
            while (timer < 0.5)
            {
                Color color = infoIcon.color;
                color.b -= 1f * Time.deltaTime;
                color.g -= 1f * Time.deltaTime;
                infoIcon.color = color;
                timer += Time.deltaTime;
                yield return null;
            }
            while (timer < 1)
            {
                Color color = infoIcon.color;
                color.b += 1f * Time.deltaTime;
                color.g += 1f * Time.deltaTime;
                infoIcon.color = color;
                timer += Time.deltaTime;
                yield return null;
            }
            while (timer < 1.5)
            {
                Color color = infoIcon.color;
                color.b -= 1f * Time.deltaTime;
                color.g -= 1f * Time.deltaTime;
                infoIcon.color = color;
                timer += Time.deltaTime;
                yield return null;
            }
            while (timer < 2)
            {
                Color color = infoIcon.color;
                color.b += 1f * Time.deltaTime;
                color.g += 1f * Time.deltaTime;
                infoIcon.color = color;
                timer += Time.deltaTime;
                yield return null;
            }
            while (timer < 2.5)
            {
                Color color = infoIcon.color;
                color.b -= 1f * Time.deltaTime;
                color.g -= 1f * Time.deltaTime;
                infoIcon.color = color;
                timer += Time.deltaTime;
                yield return null;
            }
            while (timer < 3)
            {
                Color color = infoIcon.color;
                color.b += 1f * Time.deltaTime;
                color.g += 1f * Time.deltaTime;
                infoIcon.color = color;
                timer += Time.deltaTime;
                yield return null;
            }
            while (timer < 3.5)
            {
                Color color = infoIcon.color;
                color.b -= 1f * Time.deltaTime;
                color.g -= 1f * Time.deltaTime;
                infoIcon.color = color;
                timer += Time.deltaTime;
                yield return null;
            }
            while (timer < 4)
            {
                Color color = infoIcon.color;
                color.b += 1f * Time.deltaTime;
                color.g += 1f * Time.deltaTime;
                infoIcon.color = color;
                timer += Time.deltaTime;
                yield return null;
            }
            infoIcon.color = Color.white;
            timer = 0;
            infoIconPlaying = false;
        }
    }
    //makes the tutorial box flash on new tutorial steps, assuming its not currently flashing
    IEnumerator QuestBoxFlash()
    {
        if (!tutorialBoxFlash)
        {
            tutorialBoxFlash = true;
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
            timer = 0;
            tutorialBoxFlash = false;
        }
    }
    //everything below here is things moving into place
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