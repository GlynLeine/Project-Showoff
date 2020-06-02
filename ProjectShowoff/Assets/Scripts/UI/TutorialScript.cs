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

    private float animationSpeed = 1; //reset at end of every animation, makes animations move faster as they go on
    private bool tutorialPlayed = false; //set to true once all animations have played
    //animation bools, set to true to play - can only be played once
    public bool buildingAnimation = false;
    public bool destroyAnimation = true;
    public bool sliderAnimation = false;
    public bool newsCasterAnimation = false;
    public bool timeImagesAnimation = false;
    public bool resetAnimation = false;
    //tutorial step bools
    public bool tutorialRotationStep = false;
    public bool tutorialZoomStep = false;
    public bool tutorialBuildStep = false;
    void Update()
    {
        if (!tutorialPlayed)
        {
            if (!tutorialRotationStep)
            {
                if (InputRedirect.pressed)
                {
                    sliderAnimation = true;
                }
                if (sliderAnimation)
                {
                    Vector3 sliderAnimationPosition;
                    sliderAnimationPosition = zoomSlider.transform.position;
                    animationSpeed += 0.01f;
                    sliderAnimationPosition.x -= 360 * Time.deltaTime * animationSpeed;
                    if (sliderAnimationPosition.x <= 1790)
                    {
                        animationSpeed = 1;
                        sliderAnimation = false;
                        tutorialRotationStep = false;
                        sliderAnimationPosition.x = 1790;
                        zoomSlider.transform.position = sliderAnimationPosition;
                    }
                    else
                    {
                        zoomSlider.transform.position = sliderAnimationPosition;   
                    }
                }
            }
            if (buildingAnimation)
            {
                Vector3 buildingAnimationPosition;
                buildingAnimationPosition = buildings.transform.position;
                animationSpeed += 0.01f;
                buildingAnimationPosition.y += 360 * Time.deltaTime * animationSpeed;
                if (buildingAnimationPosition.y >= 125)
                {
                    animationSpeed = 1;
                    buildingAnimation = false;
                    tutorialZoomStep = true;
                    buildingAnimationPosition.y = 125;
                    buildings.transform.position = buildingAnimationPosition;  
                }
                else
                {
                    buildings.transform.position = buildingAnimationPosition;   
                }
            }
            if (destroyAnimation)
            {
                Vector3 destroyAnimationPosition;
                destroyAnimationPosition = destroy.transform.position;
                animationSpeed += 0.01f;
                destroyAnimationPosition.x += 360 * Time.deltaTime * animationSpeed;
                if (destroyAnimationPosition.x >= 135)
                {
                    animationSpeed = 1;
                    destroyAnimation = false;
                    destroyAnimationPosition.x = 135;
                    destroy.transform.position = destroyAnimationPosition;  
                }
                else
                {
                    destroy.transform.position = destroyAnimationPosition;   
                }
            }
            if (newsCasterAnimation)
            {
                /*Vector3 newsCasterAnimationPosition;
                newsCasterAnimationPosition = announcerBox.transform.position;
                animationSpeed += 0.01f;
                newsCasterAnimationPosition.x -= 360 * Time.deltaTime * animationSpeed;
                if (newsCasterAnimationPosition.x <= 1790)
                {
                    animationSpeed = 1;
                    newsCasterAnimation = false;
                }
                else
                {
                    announcerBox.transform.position = newsCasterAnimationPosition;   
                }*/
            }
            if (timeImagesAnimation)
            {
                Vector3 timeAnimationPosition;
                timeAnimationPosition = timeImages.transform.position;
                animationSpeed += 0.01f;
                timeAnimationPosition.y -= 360 * Time.deltaTime * animationSpeed;
                if (timeAnimationPosition.y <= 990)
                {
                    animationSpeed = 1;
                    timeImagesAnimation = false;
                    timeAnimationPosition.y = 990;
                    timeImages.transform.position = timeAnimationPosition;
                }
                else
                {
                    timeImages.transform.position = timeAnimationPosition;   
                }
            }
            if (resetAnimation)
            {
                Vector3 resetAnimationPosition;
                resetAnimationPosition = reset.transform.position;
                animationSpeed += 0.01f;
                resetAnimationPosition.x -= 360 * Time.deltaTime * animationSpeed;
                if (resetAnimationPosition.x <= 1830)
                {
                    animationSpeed = 1;
                    resetAnimation = false;
                    resetAnimationPosition.x = 1830;
                    reset.transform.position = resetAnimationPosition;
                }
                else
                {
                    reset.transform.position = resetAnimationPosition;   
                }
            }
        }
    }

    public void SliderTutorialChange()
    {
        if (!tutorialZoomStep)
        {
            buildingAnimation = true;   
        }
    }

    public void BuildingTutorialButton()
    {
        if (!tutorialBuildStep)
        {
            timeImagesAnimation = true;
            resetAnimation = true;
        }
    }
}