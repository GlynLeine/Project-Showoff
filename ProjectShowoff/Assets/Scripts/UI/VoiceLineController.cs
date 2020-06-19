using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceLineController : MonoBehaviour
{
    private bool isAudioPlaying;
    public int audioWaitTime;
    [FMODUnity.EventRef]
    public string englishGlobalWarming = "event:/Voicelines/Global_warming_triggers_EN";
    public string dutchGlobalWarming = "event:/Voicelines/Global_warming_triggers_NL";
    private FMOD.Studio.EventInstance globalWarming;
    //English audio
    //global warming triggers
    private string ePollutionUp = "EN_pollution_up";
    private string eWaterLevelUp = "EN_water_level_rises";
    private string eFogIncrease = "EN_pollution_fog_increase";
    private string eNatureDown = "EN_nature_decreases";
    private string eNatureUp = "EN_nature_grows";
    private string ePollutionDown = "EN_pollution_down";
    //Dutch audio
    private string nlPollutionUp = "NL_pollution_up";
    private string nlWaterLevelUp = "NL_water_level_rises";
    private string nlFogIncrease = "NL_pollution_fog_increase";
    private string nlNatureDown = "NL_nature_decreases";
    private string nlNatureUp = "NL_nature_grows";
    private string nlPollutionDown = "NL_pollution_down";
    //the actual strings that set stuff, these get set to English or Dutch equivelant at start
    private string pollutionUp;
    private string waterLevelUp;
    private string fogIncrease;
    private string natureDown;
    private string natureUp;
    private string pollutionDown;
    void Start()
    {
        if (LanguageSelector.LanguageSelected == LanguageSelector.LanguageSelectorSelected.English)
        {
            globalWarming = FMODUnity.RuntimeManager.CreateInstance(englishGlobalWarming);
            pollutionUp = ePollutionUp;
            pollutionDown = ePollutionDown;
            waterLevelUp = eWaterLevelUp;
            fogIncrease = eFogIncrease;
            natureDown = eNatureDown;
            natureUp = eNatureUp;
        }
        else
        {
            globalWarming = FMODUnity.RuntimeManager.CreateInstance(dutchGlobalWarming);
            pollutionUp = nlPollutionUp;
            pollutionDown = nlPollutionDown;
            waterLevelUp = nlWaterLevelUp;
            fogIncrease = nlFogIncrease;
            natureDown = nlNatureDown;
            natureUp = nlNatureUp;
        }

        StartCoroutine(VoiceLineSystem(globalWarming, pollutionUp));
    }

    IEnumerator VoiceLineSystem(FMOD.Studio.EventInstance e, string eventName)
    {
        if (!isAudioPlaying)
        {
            isAudioPlaying = true;
            Debug.Log(e);
            Debug.Log(eventName);
            e.start();
            e.setParameterByName(eventName, 1);
            yield return new WaitForSeconds(audioWaitTime);
            e.setParameterByName(eventName, 0);
            e.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            yield return null;
            isAudioPlaying = false;
        }
    }
}
