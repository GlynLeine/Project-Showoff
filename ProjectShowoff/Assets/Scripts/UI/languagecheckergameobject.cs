using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class languagecheckergameobject : MonoBehaviour
{
    public GameObject dutchObject;
    public GameObject englishObject;
    public Button englishButton;
    public Button dutchButton;

    private void Start()
    {
        if (dutchButton != null && englishButton != null)
        {
            englishButton.onClick.AddListener(ButtonClick);
            dutchButton.onClick.AddListener(ButtonClick);   
        }
        if (LanguageSelector.LanguageSelected == LanguageSelector.LanguageSelectorSelected.English)
        {
            dutchObject.SetActive(false);
            englishObject.SetActive(true);
        }
        if (LanguageSelector.LanguageSelected == LanguageSelector.LanguageSelectorSelected.Dutch)
        {
            englishObject.SetActive(false);
            dutchObject.SetActive(true);
        }
    }

    public void ButtonClick()
    {
        if (LanguageSelector.LanguageSelected == LanguageSelector.LanguageSelectorSelected.English)
        {
            dutchObject.SetActive(false);
            englishObject.SetActive(true);
        }
        if (LanguageSelector.LanguageSelected == LanguageSelector.LanguageSelectorSelected.Dutch)
        {
            englishObject.SetActive(false);
            dutchObject.SetActive(true);
        }
    }
}
