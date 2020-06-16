using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScript : MonoBehaviour
{
    //drag all UI elements in + anything else we might need to alter or turn off
    public GameObject zoomSlider;
    public GameObject announcerBox;
    public GameObject timeImages;
    public GameObject reset;
    public GameObject tutorialArrow;
    public GameObject tutorialHand;
    public TMP_Text questBoxText;
    public GameObject questSystem;
    public GameObject ruralBuildings;
    private Slider slider;
    private float animationSpeed = 1; //reset at end of every animation, makes animations move faster as they go on
    //tutorial step bools so we can check where the player is
    private bool tutorialRotationStep;
    private bool tutorialZoomStep;
    private bool tutorialBuildStep;
    private bool gameHasStarted;
    private float tutorialDelaySeconds = 2.5f;
    public bool tutorialSkip;
    public bool English;
    private int buildingCount;

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
    public void tutorialStart()
    {
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
            questBoxText.text = "Click on the purple dome and place a factory there!";
        }
        else
        {
            questBoxText.text = "Klik op de paarse cirkel en plaats daar een fabriek!";
        }
    }
    //this gets called when a building is placed, we have tutorialized the first 4 buildings so far 
    private void OnBuildingPlaced(BuildingLocation location, BuildingPlacer buildingData, Building building)
    {
        buildingCount += 1;
        if (buildingCount == 1)
        {
            StartCoroutine(BuildingNatureReserveWaiter());
            if (English)
            {
                questBoxText.text = "Good job! Now place a nature reserve to balance the pollution!";
            }
            else
            {
                questBoxText.text = "Goed gedaan! Plaats nu een natuurgebied om de vervuiling te stoppen!";
            }   
        }
        else if (buildingCount == 2)
        {
            StartCoroutine(BuildingActivationWaiter());
            if (English)
            {
                questBoxText.text = "Nice! There is one spot left, why don't you try something new?";
            }
            else
            {
                questBoxText.text = "Netjes! Er is nog een plek over, waarom probeer je niet iets nieuws?";
            }  
        }
        else if (buildingCount == 3)
        {
            if (English)
            {
                questBoxText.text = "Wow! The island is full! Try zooming out and finding a new spot!";
            }
            else
            {
                questBoxText.text = "Wow! Het hele eiland is vol! Probeer is uit te zoomen en een nieuwe plek te vinden!";
            }
            StartCoroutine(SliderAnimationStart());
        }
        else if (buildingCount == 4)
        {
            if (English)
            {
                questBoxText.text = "Good job, you can place what you want now, but you can help us more!.";
            }
            else
            {
                questBoxText.text = "Goed gedaan, je kan nu plaatsen wat je wil, maar we zouden het erg waarderen als je ons helpt.";
            }
            StartCoroutine(QuestChanger());
        }
        else if (buildingCount >= 5)
        {
            BuildingSystem.onBuildingPlaced -= OnBuildingPlaced;
        }
    }
    public void SliderTutorialChange(float value)
    {
        slider.onValueChanged.RemoveListener(SliderTutorialChange);
        tutorialZoomStep = true;
        GameManager.paused = false;
        tutorialBuildStep = true;
        StartCoroutine(TimerAnimationStart());
        StartCoroutine(ResetAnimationStart());
    }

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

    IEnumerator QuestChanger()
    {
        yield return new WaitForSeconds(2);
        questBoxText.gameObject.SetActive(false);
        questSystem.SetActive(true);
    }
    IEnumerator BuildingNatureReserveWaiter()
    {
        yield return new WaitForSeconds(1);
        foreach (Transform child in ruralBuildings.transform)
        {
            if (child.gameObject.name != "Nature reserve")
            {
                child.gameObject.SetActive(false);
            }
            else
            {
                child.gameObject.SetActive(true);
            }
        }
    }
    IEnumerator BuildingActivationWaiter()
    {
        yield return new WaitForSeconds(1);
        foreach (Transform child in ruralBuildings.transform)
        {
            child.gameObject.SetActive(true);
        }
    }
    //shouldve used anchored positions instead but it works now
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