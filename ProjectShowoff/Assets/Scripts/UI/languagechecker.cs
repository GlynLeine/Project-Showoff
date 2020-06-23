using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class languagechecker : MonoBehaviour
{
    private TMP_Text tmpText;
    public string dutchText;
    public string englishText;
    public Button englishButton;
    public Button dutchButton;

    private void Start()
    {
        if (dutchButton != null && englishButton != null)
        {
            englishButton.onClick.AddListener(ButtonClick);
            dutchButton.onClick.AddListener(ButtonClick);   
        }
        tmpText = this.GetComponent<TMP_Text>();
        dutchText = dutchText.Replace("//n", Environment.NewLine);
        englishText = englishText.Replace("//n", Environment.NewLine);
        if (LanguageSelector.LanguageSelected == LanguageSelector.LanguageSelectorSelected.English)
        {
            tmpText.text = englishText;
        }
        if (LanguageSelector.LanguageSelected == LanguageSelector.LanguageSelectorSelected.Dutch)
        {
            tmpText.text = dutchText;
        }
    }

    public void ButtonClick()
    {
        if (LanguageSelector.LanguageSelected == LanguageSelector.LanguageSelectorSelected.English)
        {
                tmpText.text = englishText;
        }
        if (LanguageSelector.LanguageSelected == LanguageSelector.LanguageSelectorSelected.Dutch)
        {
                tmpText.text = dutchText;
        }
    }
}
