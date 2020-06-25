using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceLineSetterEndScreen : MonoBehaviour
{
    public AnnouncerText announcerText;
    private bool english;
    void Start()
    {
        announcerText = gameObject.GetComponent<AnnouncerText>();
    }

    private void OnEnable()
    {
        if (LanguageSelector.LanguageSelected == LanguageSelector.LanguageSelectorSelected.English)
        {
            english = true;
        }
        StartCoroutine(WaitASecond());
    }

    IEnumerator WaitASecond()
    {
        while (gameObject.activeSelf)
        {
            if (english)
            {
                announcerText.TextChanger("Thank you for playing!");
            }
            else
            {
                announcerText.TextChanger("Dankjewel voor het spelen!");
            }
            yield return new WaitForSeconds(1);
        }
    }
}
