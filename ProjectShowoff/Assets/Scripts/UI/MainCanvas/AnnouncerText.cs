using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnnouncerText : MonoBehaviour
{
    public int announcerTextSpeed = 1;
    [SerializeField] private GameObject announcerTextBox = null;
    private Vector3 announcerPosition;
    private RectTransform announcerRectTransform;
    private float announcerTextWidth;
    private TMP_Text announcerTextText;
    private bool textChangeRequest = false;
    private string textChange = null;

    private void Start()
    {
        announcerRectTransform = (RectTransform)announcerTextBox.transform;
        announcerTextText = announcerTextBox.GetComponent<TMP_Text>();
        announcerTextWidth = announcerRectTransform.sizeDelta.x;
    }

    void Update()
    {
        announcerTextBox.transform.Translate(-announcerTextSpeed * GameManager.deltaTime, 0, 0);
        announcerPosition = announcerTextBox.transform.localPosition;
        if (announcerPosition.x <= (-announcerTextWidth + -100))
        {
            if (textChangeRequest)
            {
                announcerTextText.text = textChange;
                announcerTextWidth = announcerRectTransform.sizeDelta.x;
                textChangeRequest = false;
            }
            announcerPosition.x = 120;
            announcerTextBox.transform.localPosition = announcerPosition;
        }
    }

    public void TextChanger(string newText)
    {
        textChangeRequest = true;
        textChange = newText;
    }
}
