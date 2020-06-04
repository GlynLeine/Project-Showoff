using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    //drag all UI elements in
    public GameObject buildings;
    public GameObject destroy;
    public GameObject zoomSlider;
    public GameObject announcerBox;
    public GameObject timeImages;
    public GameObject reset;
    public GameObject tutorialArrow;
    public GameObject tutorialHand;

    private float animationSpeed = 1; //reset at end of every animation, makes animations move faster as they go on
    //tutorial step bools so we can check where the player is
    private bool tutorialRotationStep;
    private bool tutorialZoomStep;
    private bool tutorialBuildStep;
    private bool gameHasStarted;
    private float tutorialDelaySeconds = 2.5f;
    public bool tutorialSkip;

    void Start()
    {
        StartCoroutine(NewsCasterAnimationStart());
        StartCoroutine(HandAnimation());
        GameManager.paused = true;
        if (tutorialSkip)
        {
            //basically sets the delay of the animations to 0 nd just throws em all in at the start
            tutorialDelaySeconds = 0;
            tutorialBuildStep = true;
            tutorialZoomStep = true;
            tutorialRotationStep = true;
            StartCoroutine(BuildingAnimationStart());
            StartCoroutine(TimerAnimationStart());
            StartCoroutine(ResetAnimationStart());
            StartCoroutine(DestroyAnimationStart());
            StartCoroutine(SliderAnimationStart());
        }
    }
    void Update()
    {
        if (!tutorialRotationStep)
        {
            if (InputRedirect.pressed)
            {
                tutorialArrow.SetActive(false);
                tutorialHand.SetActive(false);
                tutorialRotationStep = true;
                StartCoroutine(SliderAnimationStart());
            }
        }
    }

    public void SliderTutorialChange()
    {
        if (!tutorialZoomStep)
        {
            tutorialZoomStep = true;
            StartCoroutine(BuildingAnimationStart());
        }
    }

    public void BuildingTutorialButton()
    {
        if (!tutorialBuildStep)
        {
            tutorialBuildStep = true;
            StartCoroutine(TimerAnimationStart());
            StartCoroutine(ResetAnimationStart());
            StartCoroutine(DestroyAnimationStart());
        }
    }

    IEnumerator SliderAnimationStart()
    {
        yield return new WaitForSeconds(tutorialDelaySeconds);
        while (zoomSlider.transform.position.x > 1790)
        {
            Vector3 sliderAnimationPosition;
            sliderAnimationPosition = zoomSlider.transform.position;
            animationSpeed += 0.1f;
            sliderAnimationPosition.x -= 3.6f * animationSpeed;
            if (sliderAnimationPosition.x <= 1790)
            {
                animationSpeed = 1;
                tutorialRotationStep = true;
                sliderAnimationPosition.x = 1790;
                zoomSlider.transform.position = sliderAnimationPosition;
            }
            else
            {
                zoomSlider.transform.position = sliderAnimationPosition;   
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator BuildingAnimationStart()
    {
        yield return new WaitForSeconds(tutorialDelaySeconds);
        while (buildings.transform.position.y < 125)
        {
            Vector3 buildingAnimationPosition;
            buildingAnimationPosition = buildings.transform.position;
            animationSpeed += 0.1f;
            buildingAnimationPosition.y += 3.6f * animationSpeed;
            if (buildingAnimationPosition.y >= 125)
            {
                animationSpeed = 1;
                buildingAnimationPosition.y = 125;
                buildings.transform.position = buildingAnimationPosition;  
            }
            else
            {
                buildings.transform.position = buildingAnimationPosition;   
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator DestroyAnimationStart()
    {
        yield return new WaitForSeconds(tutorialDelaySeconds*8);
        while (destroy.transform.position.x < 135)
        {
            Vector3 destroyAnimationPosition;
            destroyAnimationPosition = destroy.transform.position;
            animationSpeed += 0.1f;
            destroyAnimationPosition.x += 3.6f * animationSpeed;
            if (destroyAnimationPosition.x >= 135)
            {
                animationSpeed = 1;
                destroyAnimationPosition.x = 135;
                destroy.transform.position = destroyAnimationPosition;
            }
            else
            {
                destroy.transform.position = destroyAnimationPosition;   
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator TimerAnimationStart()
    {
        yield return new WaitForSeconds(tutorialDelaySeconds);
        while (timeImages.transform.position.y > 990)
        {
            Vector3 timeAnimationPosition;
            timeAnimationPosition = timeImages.transform.position;
            animationSpeed += 0.1f;
            timeAnimationPosition.y -= 3.6f * animationSpeed;
            if (timeAnimationPosition.y <= 990)
            {
                animationSpeed = 1;
                GameManager.paused = false;
                timeAnimationPosition.y = 990;
                timeImages.transform.position = timeAnimationPosition;
            }
            else
            {
                timeImages.transform.position = timeAnimationPosition;   
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator ResetAnimationStart()
    {
        yield return new WaitForSeconds(tutorialDelaySeconds);
        while(reset.transform.position.x > 1830)
        {
            Vector3 resetAnimationPosition;
            resetAnimationPosition = reset.transform.position;
            animationSpeed += 0.1f;
            resetAnimationPosition.x -= 3.6f * animationSpeed;
            if (resetAnimationPosition.x <= 1830)
            {
                animationSpeed = 1;
                resetAnimationPosition.x = 1830;
                reset.transform.position = resetAnimationPosition;
            }
            else
            {
                reset.transform.position = resetAnimationPosition;   
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator NewsCasterAnimationStart()
    {
        while(announcerBox.transform.position.x < 135)
        {
            Vector3 newsCasterAnimationPosition;
            newsCasterAnimationPosition = announcerBox.transform.position;
            animationSpeed += 0.1f;
            newsCasterAnimationPosition.x += 3.6f * animationSpeed;
            if (newsCasterAnimationPosition.x >= 135)
            {
                animationSpeed = 1;
            }
            else
            {
                announcerBox.transform.position = newsCasterAnimationPosition;   
            }
            yield return new WaitForSeconds(0.01f);
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
                tutorialHand.transform.Rotate(0, 0, -0.6f);
                yield return new WaitForSeconds(0.01f);
                if (tutorialHand.transform.rotation.z <= -0.6f)
                {
                    tutorialHand.transform.rotation = Quaternion.Euler(0, 0, 15);
                }
            }
        }
    }
}