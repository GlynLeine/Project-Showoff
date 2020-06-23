using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnnouncerText : MonoBehaviour
{
    public int announcerTextSpeed = 1;
    [SerializeField] private GameObject announcerTextBox = null;
    private Vector2 announcerPosition;
    private RectTransform announcerRectTransform;
    private float announcerTextWidth;
    private TMP_Text announcerTmpText;
    private bool textChangeRequest = false;
    private string textChange = null;

    private void Start()
    {
        announcerRectTransform = (RectTransform)announcerTextBox.transform;
        announcerTmpText = announcerTextBox.GetComponent<TMP_Text>();
        announcerTextWidth = announcerRectTransform.rect.width;
    }

    void Update()
    {
        announcerPosition = announcerRectTransform.anchoredPosition;
        announcerPosition.x -= announcerTextSpeed * GameManager.deltaTime;
        announcerRectTransform.anchoredPosition = announcerPosition;
        if (announcerPosition.x <= announcerTextWidth)
        {
            if (textChangeRequest)
            {
                StartCoroutine(TextChangeSetter());
                textChangeRequest = false;
            }
            else
            {
                announcerTmpText.text = "";
                announcerTextWidth = 120;
            }
            announcerPosition = announcerRectTransform.anchoredPosition;
            announcerPosition.x = 120;
            announcerRectTransform.anchoredPosition = announcerPosition;
        }
    }

    public void TextChanger(string newText)
    {
        textChangeRequest = true;
        textChange = newText;
    }

    IEnumerator TextChangeSetter()
    {
        announcerTmpText.text = textChange;
        //we need to give it one frame to change the width before we get the width
        yield return null;
        announcerTextWidth = -(announcerRectTransform.rect.width + 120);

    }
}
