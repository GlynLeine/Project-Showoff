﻿using System.Collections;
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
            GameManager.paused = false;
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
            GameManager.paused = false;
            tutorialBuildStep = true;
            StartCoroutine(TimerAnimationStart());
            StartCoroutine(ResetAnimationStart());
            StartCoroutine(DestroyAnimationStart());
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
            sliderAnimationPosition.x -= 3.6f * animationSpeed;
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
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator BuildingAnimationStart()
    {
        yield return new WaitForSeconds(tutorialDelaySeconds);
        while (buildings.transform.localPosition.y < -420)
        {
            Vector3 buildingAnimationPosition;
            buildingAnimationPosition = buildings.transform.localPosition;
            animationSpeed += 0.1f;
            Debug.Log(buildings.transform.localPosition);
            buildingAnimationPosition.y += 3.6f * animationSpeed;
            if (buildingAnimationPosition.y >= -420)
            {
                animationSpeed = 1;
                buildingAnimationPosition.y = -420;
                buildings.transform.localPosition = buildingAnimationPosition;  
            }
            else
            {
                buildings.transform.localPosition = buildingAnimationPosition;   
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator DestroyAnimationStart()
    {
        yield return new WaitForSeconds(tutorialDelaySeconds*8);
        while (destroy.transform.localPosition.x < -810)
        {
            Vector3 destroyAnimationPosition;
            destroyAnimationPosition = destroy.transform.localPosition;
            animationSpeed += 0.1f;
            destroyAnimationPosition.x += 3.6f * animationSpeed;
            if (destroyAnimationPosition.x >= -810)
            {
                animationSpeed = 1;
                destroyAnimationPosition.x = -810;
                destroy.transform.localPosition = destroyAnimationPosition;
            }
            else
            {
                destroy.transform.localPosition = destroyAnimationPosition;   
            }
            yield return new WaitForSeconds(0.01f);
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
            timeAnimationPosition.y -= 3.6f * animationSpeed;
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
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator ResetAnimationStart()
    {
        yield return new WaitForSeconds(tutorialDelaySeconds);
        while(reset.transform.localPosition.x > 800)
        {
            Vector3 resetAnimationPosition;
            resetAnimationPosition = reset.transform.localPosition;
            animationSpeed += 0.1f;
            resetAnimationPosition.x -= 3.6f * animationSpeed;
            if (resetAnimationPosition.x <= 800)
            {
                animationSpeed = 1;
                resetAnimationPosition.x = 800;
                reset.transform.localPosition = resetAnimationPosition;
            }
            else
            {
                reset.transform.localPosition = resetAnimationPosition;   
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator NewsCasterAnimationStart()
    {
        while(announcerBox.transform.localPosition.x < -810)
        {
            Vector3 newsCasterAnimationPosition;
            newsCasterAnimationPosition = announcerBox.transform.localPosition;
            animationSpeed += 0.1f;
            newsCasterAnimationPosition.x += 3.6f * animationSpeed;
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