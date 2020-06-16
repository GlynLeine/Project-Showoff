using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageSelector : MonoBehaviour
{
    public enum LanguageSelectorSelected
    {
        English, Dutch
    }

    public static LanguageSelectorSelected LanguageSelected = LanguageSelectorSelected.Dutch;

    public void englishSelectButton()
    {
        LanguageSelected = LanguageSelectorSelected.English;
    }

    public void dutchSelectButton()
    {
        LanguageSelected = LanguageSelectorSelected.Dutch;
    }
}
