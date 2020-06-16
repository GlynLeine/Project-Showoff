using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    //drag all UI elements in
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
        GameManager.paused = true;
        StartCoroutine(NewsCasterAnimationStart());
        StartCoroutine(HandAnimation());
        StartCoroutine(TemporaryUpdate());
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
    public void SliderTutorialChange()
    {
        if (!tutorialZoomStep)
        {
            tutorialZoomStep = true;
        }
    }

    public void BuildingTutorialButton()
    {
        if (!tutorialBuildStep)
        {
            GameManager.paused = false;
            tutorialBuildStep = true;
            StartCoroutine(TimerAnimationStart());
            StartCoroutine(ResetAnimationStart());
        }
    }

    IEnumerator TemporaryUpdate()
    {
        while (!tutorialRotationStep)
        {
            if (InputRedirect.pressed)
            {
                tutorialArrow.SetActive(false);
                tutorialHand.SetActive(false);
                tutorialRotationStep = true;
                StartCoroutine(SliderAnimationStart());
            }
            yield return null;
        }
    }
    //shouldve used anchored positions instead but it works now
    IEnumerator SliderAnimationStart()
    {
        yield return new WaitForSeconds(tutorialDelaySeconds);
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