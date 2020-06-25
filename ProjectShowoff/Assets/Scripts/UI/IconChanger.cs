using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconChanger : MonoBehaviour
{
    public Sprite airPortSprite;
    public Sprite spaceStationSprite;
    public Sprite trainStationSprite;
    public Image thisImage;
    void Start()
    {
        thisImage = gameObject.GetComponent<Image>();
    }

    void OnEnable()
    {
        StartCoroutine(SlowerUpdate());
    }

    IEnumerator SlowerUpdate()
    {
        while (gameObject.activeSelf)
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
            yield return new WaitForSeconds(0.5f);
        }
    }

}
