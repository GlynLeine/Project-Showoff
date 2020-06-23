using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconChangerOnEnable : MonoBehaviour
{
    public Sprite airPortSprite;
    public Sprite spaceStationSprite;
    public Sprite trainStationSprite;
    private Image thisImage;
    void Start()
    {
        thisImage = gameObject.GetComponent<Image>();
    }

    private void OnEnable()
    {
        if (GameManager.industry >= 30)
        {
            thisImage.sprite = spaceStationSprite;
        } else if (GameManager.industry >= 15)
        {
            thisImage.sprite = airPortSprite;
        }
        else
        {
            thisImage.sprite = trainStationSprite;
        }
    }

}
