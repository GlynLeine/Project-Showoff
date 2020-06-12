using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageSelector : MonoBehaviour
{
    public static bool EnglishSelected = true;
    public static bool DutchSelected;

    public void englishSelectButton()
    {
        EnglishSelected = true;
        DutchSelected = false;
    }

    public void dutchSelectButton()
    {
        EnglishSelected = false;
        DutchSelected = true;
    }
}
