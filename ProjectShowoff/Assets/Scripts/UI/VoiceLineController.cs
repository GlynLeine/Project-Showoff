using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceLineController : MonoBehaviour
{
    private bool isAudioPlaying;
    private bool updateBool = true;
    public int audioWaitTime = 15;
    [FMODUnity.EventRef] //basically everything is repeated as english and dutch versions
    //event strings
    public string englishGlobalWarming = "event:/Voicelines/Global_warming_triggers_EN";
    public string dutchGlobalWarming = "event:/Voicelines/Global_warming_triggers_NL";
    public string englishMisc = "event:/Voicelines/Miscellaneous_EN";
    public string dutchMisc = "event:/Voicelines/Miscellaneous_NL";
    public string englishPlayTriggers = "event:/Voicelines/Play_action_triggers_EN";
    public string dutchPlayTriggers = "event:/Voicelines/Play_action_triggers_NL";
    public string englishTimeTriggers = "event:/Voicelines/Time_base_triggers_EN";
    public string dutchTimeTriggers = "event:/Voicelines/Time_base_triggers_NL";
    public string englishTutorial = "event:/Voicelines/Tutorial_EN";
    public string dutchTutorial = "event:/Voicelines/Tutorial_NL";

    private FMOD.Studio.EventInstance globalWarming;
    private FMOD.Studio.EventInstance misc;
    private FMOD.Studio.EventInstance playTriggers;
    private FMOD.Studio.EventInstance timeTriggers;
    private FMOD.Studio.EventInstance tutorial;
    
    //English audio
    //global warming triggers
    private string ePollutionUp = "EN_pollution_up";
    private string eWaterLevelUp = "EN_water_level_rises";
    private string eFogIncrease = "EN_pollution_fog_increase";
    private string eNatureDown = "EN_nature_decreases";
    private string eNatureUp = "EN_nature_grows";
    private string ePollutionDown = "EN_pollution_down";
    //misc triggers
    private string eHoliday = "EN_holiday";
    private string eDog = "EN_lost_dog";
    private string eMovie = "EN_new_movie_comes_out";
    private string ePizza = "EN_president_eats_pizza";
    private string eDonuts = "EN_record_number_of_donuts_sold";
    //play action triggers
    private string eMine = "EN_new_coalmine";
    private string eCoast = "EN_new_coast_building";
    private string eFactory = "EN_new_factory";
    private string eNatureReserve = "EN_new_nature_reserve";
    private string eSolarPanel = "EN_new_solar_panel";
    private string eTrainStation = "EN_new_train_depot";
    //private string eNuclearPlant = "EN_new_nuclear_power_plant" replaced by oil rig
    //time based triggers
    private string eAirplanes = "EN_airplanes_start_appearing";
    private string eNewArea = "EN_first_building_on_new_area_(growth)";
    private string eSatellites = "EN_satellites_start_appearing";
    private string eAutumn = "EN_season_change_autumn";
    private string eSpring = "EN_season_change_spring";
    private string eSummer = "EN_season_change_summer";
    private string eWinter = "EN_season_change_winter";
    //tutorial lines
    private string eBuild = "EN_build";
    private string eDestroy = "EN_destroy";
    private string eSwipe = "EN_swipe";
    private string eZoom = "EN_zoom";
    
    //Dutch audio
    private string nlPollutionUp = "NL_pollution_up";
    private string nlWaterLevelUp = "NL_water_level_rises";
    private string nlFogIncrease = "NL_pollution_fog_increase";
    private string nlNatureDown = "NL_nature_decreases";
    private string nlNatureUp = "NL_nature_grows";
    private string nlPollutionDown = "NL_pollution_down";
    //misc triggers
    private string nlHoliday = "NL_holiday";
    private string nlDog = "NL_lost_dog";
    private string nlMovie = "NL_new_movie_comes_out";
    private string nlPizza = "NL_president_eats_pizza";
    private string nlDonuts = "NL_record_number_of_donuts_sold";
    //play triggers
    private string nlMine = "NL_new_coalmine";
    private string nlCoast = "NL_new_coast_building";
    private string nlFactory = "NL_new_factory";
    private string nlNatureReserve = "NL_new_nature_reserve";
    private string nlSolarPanel = "NL_new_solar_panel";
    private string nlTrainStation = "NL_new_train_depot";
    //private string nlNuclearPlant = "NL_new_nuclear_power_plant" replaced by oil rig
    //time triggers
    private string nlAirplanes = "NL_airplanes_start_appearing";
    private string nlNewArea = "NL_first_building_on_new_area_(growth)";
    private string nlSatellites = "NL_satellites_start_appearing";
    private string nlAutumn = "NL_season_change_autumn";
    private string nlSpring = "NL_season_change_spring";
    private string nlSummer = "NL_season_change_summer";
    private string nlWinter = "NL_season_change_winter";
    //tutorial lines
    private string nlBuild = "NL_build";
    private string nlDestroy = "NL_destroy";
    private string nlSwipe = "NL_swipe";
    private string nlZoom = "NL_zoom";
    
    //the actual strings that set stuff, these get set to English or Dutch equivelant at enable
    private string pollutionUp;
    private string waterLevelUp;
    private string fogIncrease;
    private string natureDown;
    private string natureUp;
    private string pollutionDown;
    //misc stuff
    private string holiday;
    private string dog;
    private string movie;
    private string pizza;
    private string donuts;
    //play triggers
    private string mine;
    private string coast;
    private string factory;
    private string natureReserve;
    private string solarPanel;
    private string trainStation;
    //private string NuclearPlant;
    //time triggers
    private string airplanes;
    private string newArea;
    private string satellites;
    private string autumn;
    private string spring;
    private string summer;
    private string winter;
    //tutorial lines
    private string build;
    private string destroy;
    private string swipe;
    private string zoom;
    //now heres a list for all the bools, to check if audio has already placed before - they should only fire once per game after all
    private bool pollutionUpBool;
    private bool waterUpBool;
    private bool fogIncreaseBool;
    private bool natureDownBool;
    private bool natureUpBool;
    private bool pollutionDownBool;
    //misc stuff
    private bool holidayBool;
    private bool dogBool;
    private bool movieBool;
    private bool pizzaBool;
    private bool donutsBool;
    //play triggers
    private bool mineBool;
    private bool coastBool;
    private bool factoryBool;
    private bool natureReserveBool;
    private bool solarPanelBool;
    private bool trainStationBool;
    //private bool NuclearPlant;
    //time triggers
    private bool airplanesBool;
    private bool newAreaBool;
    private bool satellitesBool;
    private bool autumnBool;
    private bool springBool;
    private bool summerBool;
    private bool winterBool;
    //tutorial triggers
    private bool buildBool;
    private bool destroyBool;
    private bool swipeBool;
    private bool zoomBool;
    void Start()
    {
        if (LanguageSelector.LanguageSelected == LanguageSelector.LanguageSelectorSelected.English)
        {
            globalWarming = FMODUnity.RuntimeManager.CreateInstance(englishGlobalWarming);
            misc = FMODUnity.RuntimeManager.CreateInstance(englishMisc);
            playTriggers = FMODUnity.RuntimeManager.CreateInstance(englishPlayTriggers);
            timeTriggers = FMODUnity.RuntimeManager.CreateInstance(englishTimeTriggers);
            tutorial = FMODUnity.RuntimeManager.CreateInstance(englishTutorial);
            pollutionUp = ePollutionUp;
            pollutionDown = ePollutionDown;
            waterLevelUp = eWaterLevelUp;
            fogIncrease = eFogIncrease;
            natureDown = eNatureDown;
            natureUp = eNatureUp;
            holiday = eHoliday;
            dog = eDog;
            movie = eDog;
            pizza = ePizza;
            donuts = eDonuts;
            mine = eMine;
            coast = eCoast;
            factory = eFactory;
            natureReserve = eNatureReserve;
            solarPanel = eNatureReserve;
            trainStation = eTrainStation;
            airplanes = eAirplanes;
            newArea = eNewArea;
            satellites = eSatellites;
            autumn = eAutumn;
            spring = eSpring;
            summer = eSummer;
            winter = eWinter;
            build = eBuild;
            destroy = eDestroy;
            swipe = eSwipe;
            zoom = eZoom;
        }
        else
        {
            globalWarming = FMODUnity.RuntimeManager.CreateInstance(dutchGlobalWarming);
            misc = FMODUnity.RuntimeManager.CreateInstance(dutchMisc);
            playTriggers = FMODUnity.RuntimeManager.CreateInstance(dutchPlayTriggers);
            timeTriggers = FMODUnity.RuntimeManager.CreateInstance(dutchTimeTriggers);
            tutorial = FMODUnity.RuntimeManager.CreateInstance(dutchTutorial);
            pollutionUp = nlPollutionUp;
            pollutionDown = nlPollutionDown;
            waterLevelUp = nlWaterLevelUp;
            fogIncrease = nlFogIncrease;
            natureDown = nlNatureDown;
            natureUp = nlNatureUp;
            holiday = nlHoliday;
            dog = nlDog;
            movie = nlDog;
            pizza = nlPizza;
            donuts = nlDonuts;
            mine = nlMine;
            coast = nlCoast;
            factory = nlFactory;
            natureReserve = nlNatureReserve;
            solarPanel = nlNatureReserve;
            trainStation = nlTrainStation;
            airplanes = nlAirplanes;
            newArea = nlNewArea;
            satellites = nlSatellites;
            autumn = nlAutumn;
            spring = nlSpring;
            summer = nlSummer;
            winter = nlWinter;
            build = nlBuild;
            destroy = nlDestroy;
            swipe = nlSwipe;
            zoom = nlZoom;
        }
        //subscribe to onbuildingplaced and onbuildingdestroyed
        //first set all the variables to their appropriate language then call slow update function
        //we want to have an update, but its not in a rush so we do a slow update every 1sec
        StartCoroutine(slowUpdate());
    }

    private void OnDisable()
    {
        //unsubscribe
    }

    //basically this is called whenever a voice line is ready, each voice line does an if check and if true, it sets its own bool to false aka it has played
    private bool VoiceLinePlay(FMOD.Studio.EventInstance e, string eventName)
    {
        if (!isAudioPlaying)
        {
            isAudioPlaying = true;
            e.start();
            e.setParameterByName(eventName, 1);
            StartCoroutine(VoiceLineWait(e,eventName));
            return true;
        }
        else
        {
            return false;
        }
    }
    //once the voice line is playing we wait for the wait time before stopping the audio and opening it back up for play again
    IEnumerator VoiceLineWait(FMOD.Studio.EventInstance e, string eventName)
    {
            yield return new WaitForSeconds(audioWaitTime);
            e.setParameterByName(eventName, 0);
            e.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            yield return null;//redundant safety check but seems to have eliminated some weird errors
            isAudioPlaying = false;
    }

    //the purpose for this is because theres no reason to check for voice line availability every frame, but it needs to be checked often enough that we can catch them when relevant
    IEnumerator slowUpdate()
    {
        while (updateBool)
        {
            if (!pollutionUpBool && GameManager.pollution > 400)
            {
                if (VoiceLinePlay(globalWarming, pollutionUp))
                {
                    pollutionUpBool = true;
                }
            }
            if (!pollutionDownBool && pollutionUpBool && GameManager.pollution < 400)
            {
                if (VoiceLinePlay(globalWarming, pollutionDown))
                {
                    pollutionDownBool = true;
                }
            }

            if (!waterUpBool && GameManager.waterLevel > 0.15f)
            {
                if (VoiceLinePlay(globalWarming, waterLevelUp))
                {
                    waterUpBool = true;
                }
            }

            if (!fogIncreaseBool && GameManager.pollution > 1500)
            {
                if (VoiceLinePlay(globalWarming, fogIncrease))
                {
                    fogIncreaseBool = true;
                }
            }

            if (!natureUpBool && GameManager.nature > 100)
            {
                if (VoiceLinePlay(globalWarming, natureUp))
                {
                    natureUpBool = true;
                }
            }

            if (!natureDownBool && natureUpBool && GameManager.nature > 100)
            {
                if (VoiceLinePlay(globalWarming, natureDown))
                {
                    natureDownBool = true;
                }
            }
            yield return new WaitForSeconds(1);
        }
    }
}