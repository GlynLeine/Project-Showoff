using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class languagechecker : MonoBehaviour
{
    private TMP_Text tmpText;
    public string dutchText;
    public string englishText;

    private void Start()
    {
        tmpText = this.GetComponent<TMP_Text>();
        dutchText = dutchText.Replace("//n", Environment.NewLine);
        englishText = englishText.Replace("//n", Environment.NewLine);
    }

    void Update()
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
