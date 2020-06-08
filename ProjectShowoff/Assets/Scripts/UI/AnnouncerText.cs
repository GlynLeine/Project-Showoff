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
        announcerRectTransform = announcerTextBox.GetComponent<RectTransform>();
        announcerTextText = announcerTextBox.GetComponent<TMP_Text>();
    }

    void Update()
    {
        announcerTextBox.transform.Translate(-announcerTextSpeed*GameManager.deltaTime,0,0);
        announcerPosition = announcerTextBox.transform.localPosition;
        announcerTextWidth = announcerRectTransform.sizeDelta.x;
        if (announcerPosition.x <= (-announcerTextWidth + -100))
        {
            if (textChangeRequest)
            {
                announcerTextText.text = textChange;
                textChangeRequest = false;
            }
            announcerPosition.x = 120;
        }
        announcerTextBox.transform.localPosition = announcerPosition;
    }

    public void TextChanger(string newText)
    {
        textChangeRequest = true;
        textChange = newText;
    }
}
