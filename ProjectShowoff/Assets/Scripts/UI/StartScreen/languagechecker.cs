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
    private bool englishAlreadySelected;
    private bool dutchAlreadySelected;

    private void Start()
    {
        tmpText = this.GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (!englishAlreadySelected)
        {
            if (LanguageSelector.EnglishSelected)
            {
                englishAlreadySelected = true;
                dutchAlreadySelected = false;
                tmpText.text = englishText;
            }
        }
        if (!dutchAlreadySelected)
        {
            if (LanguageSelector.DutchSelected)
            {
                englishAlreadySelected = false;
                dutchAlreadySelected = true;
                tmpText.text = dutchText;
            }
        }
    }
}
