using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class VoiceLineController : MonoBehaviour
{
    private bool isAudioPlaying;
    private bool updateBool = true;
    public int audioWaitTime = 15;
    public AnnouncerText atScript;
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

    public Animator newsCasterAnimator;

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
    private bool English;
    private bool StartScreenDone;

    void Start()
    {
        StartCoroutine(SlowStartUpdate());
    }

    IEnumerator SlowStartUpdate()
    {
        while (GameManager.time < 0.5f)
        {
            //do nothing but wait
            yield return null;
        }
        //cant call this more than once but just for safety
        if (!StartScreenDone)
        {
            OnStartScreenDone();
        }
    }
    void OnStartScreenDone()
    {
        StartScreenDone = true;
        //set all the lines to the appropriate ones, set the event instances to the appropriate ones
        if (LanguageSelector.LanguageSelected == LanguageSelector.LanguageSelectorSelected.English)
        {
            English = true;
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
        StartCoroutine(SubscribeWaiter());
        StartCoroutine(SlowUpdate());
    }

    private void OnDisable()
    {
        BuildingSystem.onBuildingPlaced -= OnBuildingPlaced;
    }

    //basically this is called whenever a voice line is ready, each voice line does an if check and if true, it sets its own bool to false aka it has played
    //the reason its a bool is so that each voice line can execute its own specific code, honestly most of that code could probably be added to this as well
    //dont know how long the audio each lasts
    private bool VoiceLinePlay(FMOD.Studio.EventInstance e, string eventName, int animationPlayTime)
    {
        if (!isAudioPlaying)
        {
            isAudioPlaying = true;
            e.start();
            e.setParameterByName(eventName, 1);
            newsCasterAnimator.SetBool("Talk", true);
            StartCoroutine(StopAnimation(animationPlayTime));
            StartCoroutine(VoiceLineWait(e, eventName));
            return true;
        }

        return false;
    }

    IEnumerator StopAnimation(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        newsCasterAnimator.SetBool("Talk", false);
    }


    //once the voice line is playing we wait for the wait time before stopping the audio and opening it back up for play again
    IEnumerator VoiceLineWait(FMOD.Studio.EventInstance e, string eventName/*,int animationPlayTime*/)
    {
        yield return new WaitForSeconds(audioWaitTime);
        e.setParameterByName(eventName, 0);
        e.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        yield return null;//redundant safety check but seems to have eliminated some weird errors
        isAudioPlaying = false;
        //this coroutine waits 7 seconds and then attempts to start their lines, aka if no voice lines for 7 seconds, play a misc line
        StartCoroutine(IdlePlayer());
    }
    //everything below here are triggers
    //this is the building trigger, checks if a building is built and what building has been built - loads of if else's
    private void OnBuildingPlaced(BuildingLocation location, BuildingPlacer buildingData, Building building)
    {
        if (GameManager.industry >= 15 && !airplanesBool)
        {
            if (English)
            {
                if (VoiceLinePlay(timeTriggers, airplanes, 4))
                {
                    atScript.TextChanger("Was that a bird? No it’s a plane! Local kid reports.");
                    airplanesBool = true;
                }
            }
            else
            {
                if (VoiceLinePlay(timeTriggers, airplanes, 4))
                {
                    atScript.TextChanger("Was dat een vogel? Nee, het is een vliegtuig!  meldt lokaal kind.");
                    airplanesBool = true;
                }
            }
        }
        else if (GameManager.industry >= 30 && !satellitesBool)
        {
            if (English)
            {
                if (VoiceLinePlay(timeTriggers, satellites, 5))
                {
                    atScript.TextChanger("Satellites improve our communication and they look cool, Scientist says."); 
                    satellitesBool = true;
                }
            }
            else
            {
                if (VoiceLinePlay(timeTriggers, satellites, 5))
                {
                    atScript.TextChanger("Satellieten verbeteren onze communicatie en ze zien er cool uit, zegt de wetenschapper.");
                    satellitesBool = true;
                }
            }
        }
        //i think this is the first time ive used a switch in any code i have, ive gotten super addicted to if else if else if else if else if else but this is way clearer
        switch (buildingData.buildingType)
        {
            case BuildingType.Factory:
                {
                    if (!factoryBool)
                    {
                        if (English)
                        {
                            if (VoiceLinePlay(playTriggers, factory, 6))
                            {
                                atScript.TextChanger("Who wants a job? We have nice jobs for everybody in our new factory, bring your own lunch.");
                                factoryBool = true;
                            }
                        }
                        else
                        {
                            if (VoiceLinePlay(playTriggers, factory, 6))
                            {
                                atScript.TextChanger("Wie wil er een baan? We hebben leuke banen voor iedereen in onze nieuwe fabriek, breng je eigen lunch mee.");
                                factoryBool = true;
                            }
                        }
                    }
                    else if (!donutsBool)
                    {
                        if (English)
                        {
                            if (VoiceLinePlay(misc, donuts, 7))
                            {
                                atScript.TextChanger("A record number of donuts has been sold this week! What do they do with the holes they cut out? Hungry man asks.");
                                donutsBool = true;
                            }
                        }
                        else
                        {
                            if (VoiceLinePlay(misc, donuts, 7))
                            {
                                atScript.TextChanger("Er is deze week een recordaantal donuts verkocht! Wat doen ze met de gaten die ze hebben uitgesneden? Vraagt een hongerige man.");
                                donutsBool = true;
                            }
                        }

                    }
                    break;
                }
            case BuildingType.Harbor:
                {
                    if (!coastBool)
                    {
                        if (English)
                        {
                            if (VoiceLinePlay(playTriggers, coast, 6))
                            {
                                coastBool = true;
                                atScript.TextChanger("Our town has reached the sea! I didn’t know the ocean was this big! fisherman responds.");

                            }
                        }
                        else
                        {
                            if (VoiceLinePlay(playTriggers, coast, 6))
                            {
                                coastBool = true;
                                atScript.TextChanger("Onze stad heeft de zee bereikt! Ik wist niet dat de oceaan zo groot was! Verteld visser.");
                            }
                        }
                    }
                    break;
                }
            case BuildingType.CoalMine:
                {
                    if (!mineBool)
                    {
                        if (English)
                        {
                            if (VoiceLinePlay(playTriggers, mine, 6))
                            {
                                atScript.TextChanger("We’re now using coal to power our towns!  All of my clothes are now black, worker complains.");
                                mineBool = true;
                            }
                        }
                        else
                        {
                            if (VoiceLinePlay(playTriggers, mine, 7))
                            {
                                atScript.TextChanger("We gebruiken nu steenkool om onze steden van stroom te voorzien! Al mijn kleren zijn nu zwart!, klaagt  medewerker.");
                                mineBool = true;
                            }
                        }
                    }
                    break;
                }
            case BuildingType.NatureReserve:
                {
                    if (!natureReserveBool)
                    {
                        if (English)
                        {
                            if (VoiceLinePlay(playTriggers, natureReserve, 8))
                            {
                                atScript.TextChanger("Our nature reserve is planted and looking beautiful! Those raccoons having been partying every night this week! Complains neighbour.");
                                natureReserveBool = true;
                            }
                        }
                        else
                        {
                            if (VoiceLinePlay(playTriggers, natureReserve, 8))
                            {
                                atScript.TextChanger("Ons natuurgebied is aangeplant en ziet er prachtig uit! Die wasberen hebben deze week elke avond gefeest! Klaagt buurman.");
                                natureReserveBool = true;
                            }
                        }
                    }
                    break;
                }
            case BuildingType.OilRig:
                {
                    if (!coastBool)
                    {
                        if (English)
                        {
                            if (VoiceLinePlay(playTriggers, coast, 6))
                            {
                                coastBool = true;
                                atScript.TextChanger("Our town has reached the sea! I didn’t know the ocean was this big! fisherman responds.");

                            }
                        }
                        else
                        {
                            if (VoiceLinePlay(playTriggers, coast, 6))
                            {
                                coastBool = true;
                                atScript.TextChanger("Onze stad heeft de zee bereikt! Ik wist niet dat de oceaan zo groot was! Verteld visser.");

                            }
                        }
                    }
                    break;
                }
            case BuildingType.SolarFarm:
                {
                    if (!solarPanelBool)
                    {
                        if (English)
                        {
                            if (VoiceLinePlay(playTriggers, solarPanel, 7))
                            {
                                atScript.TextChanger("our town now has solar energy panels, mayor reports. “Can I skateboard on those?” local girl asks.");
                                solarPanelBool = true;
                            }
                        }
                        else
                        {
                            if (VoiceLinePlay(playTriggers, solarPanel, 7))
                            {
                                atScript.TextChanger("onze stad heeft nu zonne-energie panelen, meldt burgemeester, “kan ik daarop skateboarden?” vraagt lokaal meisje.");
                                solarPanelBool = true;
                            }
                        }
                    }
                    break;
                }
            case BuildingType.TrainStation:
                {
                    if (!trainStationBool)
                    {
                        if (English)
                        {
                            if (VoiceLinePlay(playTriggers, trainStation, 5))
                            {
                                atScript.TextChanger("I’m never going  be late for work again! reports man who was late for this interview.");
                                trainStationBool = true;
                            }
                        }
                        else
                        {
                            if (VoiceLinePlay(playTriggers, trainStation, 5))
                            {
                                atScript.TextChanger("Ik kom nooit meer te laat op mijn werk! meldt man die te laat was voor dit interview.");
                                trainStationBool = true;
                            }
                        }
                    }
                    break;
                }
            default: break;
        }
    }
    //the purpose for this is because theres no reason to check for voice line availability every frame, but it needs to be checked often enough that we can catch them when relevant
    private IEnumerator SlowUpdate()
    {
        if (isAudioPlaying)
        {
            //no point in checking if audio is gonna be playing, so wait, -1 because potentially itll have been triggered a second late
            yield return new WaitForSeconds(audioWaitTime - 1);
        }
        while (updateBool)
        {
            if (GameManager.time > 304)
            {
                yield break;
            }
            if (!springBool && GameManager.time > 1)
            {
                if (English)
                {
                    if (VoiceLinePlay(timeTriggers, spring, 3))
                    {
                        atScript.TextChanger("And it’s spring again, time to clean your house.");
                        springBool = true;
                    }
                }
                else
                {
                    if (VoiceLinePlay(timeTriggers, spring, 3))
                    {
                        atScript.TextChanger("En het is weer lente, tijd om je huis schoon te maken.");
                        springBool = true;
                    }
                }
            }
            else if (!summerBool && GameManager.time >= 75 && GameManager.time <= 90)
            {
                if (English)
                {
                    if (VoiceLinePlay(timeTriggers, summer, 3))
                    {
                        atScript.TextChanger("It’s summer time, don’t forget your sunscreen!");
                        summerBool = true;
                    }
                }
                else
                {
                    if (VoiceLinePlay(timeTriggers, summer, 3))
                    {
                        atScript.TextChanger("Het is zomer, vergeet je zonnebrandcrème niet!");
                        summerBool = true;
                    }
                }
            }
            else if (!autumnBool && GameManager.time >= 150 && GameManager.time <= 165)
            {
                if (English)
                {
                    if (VoiceLinePlay(timeTriggers, autumn, 3))
                    {
                        autumnBool = true;
                        atScript.TextChanger("Autumn is here, bring an umbrella.");

                    }
                }
                else
                {
                    if (VoiceLinePlay(timeTriggers, autumn, 3))
                    {
                        autumnBool = true;
                        atScript.TextChanger("Herfst is hier, neem een paraplu mee.");

                    }
                }
            }
            else if (!winterBool && GameManager.time >= 225 && GameManager.time <= 240)
            {
                if (English)
                {
                    if (VoiceLinePlay(timeTriggers, winter, 3))
                    {
                        atScript.TextChanger("Het is winter, wie wil er warme chocolademelk?");
                        winterBool = true;
                    }
                    atScript.TextChanger("It’s winter, who wants hot coco?");
                }
                else
                {
                    if (VoiceLinePlay(timeTriggers, winter, 3))
                    {
                        atScript.TextChanger("Het is winter, wie wil er warme chocolademelk?");
                        winterBool = true;
                    }
                }

            }
            else if (!holidayBool && GameManager.time > 262.5 && GameManager.time < 280)
            {
                if (English)
                {
                    if (VoiceLinePlay(misc, holiday, 5))
                    {
                        holidayBool = true;
                        atScript.TextChanger("Today is our national holiday! Please don’t light fireworks in your parents bed.");

                    }
                }
                else
                {
                    if (VoiceLinePlay(misc, holiday, 6))
                    {
                        holidayBool = true;
                        atScript.TextChanger("Vandaag is onze nationale feestdag! Steek alsjeblieft geen vuurwerk aan in het bed van je ouders.");

                    }
                }
            }
            if (!pollutionUpBool && GameManager.pollution > 400)
            {
                if (English)
                {
                    if (VoiceLinePlay(globalWarming, pollutionUp, 5))
                    {
                        atScript.TextChanger("Nature is looking a bit sad, but our new stuff looks nice! Reports citizen");
                        pollutionUpBool = true;
                    }
                }
                else
                {
                    if (VoiceLinePlay(globalWarming, pollutionUp, 6))
                    {
                        atScript.TextChanger("De natuur ziet er een beetje triest uit, maar onze nieuwe spullen zien er cool uit! Zegt lokale man");
                        pollutionUpBool = true;
                    }
                }
            }
            else if (!pollutionDownBool && pollutionUpBool && GameManager.pollution < 400)
            {
                if (English)
                {
                    if (VoiceLinePlay(globalWarming, pollutionDown, 4))
                    {
                        atScript.TextChanger("Study shows that less industry means more rainbows!");
                        pollutionDownBool = true;
                    }
                }
                else
                {
                    if (VoiceLinePlay(globalWarming, pollutionDown, 4))
                    {
                        atScript.TextChanger("Onderzoek toont aan dat minder industrie meer regenbogen betekent!");
                        pollutionDownBool = true;
                    }
                }
            }

            if (!waterUpBool && GameManager.waterLevel > 0.15f)
            {
                if (English)
                {
                    if (VoiceLinePlay(globalWarming, waterLevelUp, 7))
                    {
                        atScript.TextChanger("Planet temperatures are up and the water level is rising reports show. Time for the beach or time for a change?");

                        waterUpBool = true;
                    }
                }
                else
                {
                    if (VoiceLinePlay(globalWarming, waterLevelUp, 7))
                    {
                        atScript.TextChanger("Planeet temperaturen stijgen en het waterpeil stijgt, laten rapporten zien. Tijd voor het strand of tijd voor verandering? ");

                        waterUpBool = true;
                    }
                }
            }

            if (!fogIncreaseBool && GameManager.pollution > 1500)
            {
                if (English)
                {
                    if (VoiceLinePlay(globalWarming, fogIncrease, 7))
                    {
                        atScript.TextChanger("Study shows smog is increasing. Sunglasses are out of fashion anyway, Factory owner responds.");

                        fogIncreaseBool = true;
                    }
                }
                else
                {
                    if (VoiceLinePlay(globalWarming, fogIncrease, 7))
                    {
                        atScript.TextChanger("Uit onderzoek blijkt dat smog toeneemt. Zonnebrillen zijn sowieso uit de mode, reageert de fabriekseigenaar.");

                        fogIncreaseBool = true;
                    }
                }
            }

            if (!natureUpBool && GameManager.nature > 100)
            {
                if (English)
                {
                    if (VoiceLinePlay(globalWarming, natureUp, 7))
                    {
                        atScript.TextChanger("Family lost in forest hike for 2 days; this forest used to be smaller! Mother responded.");

                        natureUpBool = true;
                    }
                }
                else
                {
                    if (VoiceLinePlay(globalWarming, natureUp, 7))
                    {
                        atScript.TextChanger("Familie verdwaald in boswandeling gedurende 2 dagen; Dit bos was vroeger kleiner! reageert Moeder.");

                        natureUpBool = true;
                    }
                }
            }
            else if (!natureDownBool && natureUpBool && GameManager.nature < 100)
            {
                if (English)
                {
                    if (VoiceLinePlay(globalWarming, natureDown, 5))
                    {
                        atScript.TextChanger("I can’t find my house, Squirrel reports, Where did all the trees go?");

                        natureDownBool = true;
                    }
                }
                else
                {
                    if (VoiceLinePlay(globalWarming, natureDown, 4))
                    {
                        atScript.TextChanger("Ik kan mijn huis niet vinden, meldt eekhoorn, Waar zijn alle bomen gebleven?");

                        natureDownBool = true;
                    }
                }
            }

            if (!pizzaBool && GameManager.happiness > 25)
            {
                if (English)
                {
                    if (VoiceLinePlay(misc, pizza, 8))
                    {
                        atScript.TextChanger("This just came in; The president ate an entire pizza for breakfast That’s a man I can follow, local woman says.");

                        pizzaBool = true;
                    }
                }
                else
                {
                    if (VoiceLinePlay(misc, pizza, 8))
                    {
                        atScript.TextChanger("'Dit is net binnen; De president at een hele pizza als ontbijt, dat is een man die ik kan volgen, zegt een lokale vrouw.");

                        pizzaBool = true;
                    }
                }
            }
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator IdlePlayer()
    {
        if (!dogBool && !movieBool)
        {
            yield return new WaitForSeconds(7);
            //if no audio is playing after 7 seconds after the last one was finished, do one of the misc ones
        }
        if (!isAudioPlaying)
        {
            if (GameManager.time > 300)
            {
                yield break;
            }
            /*if (!dogBool)
            {
                if (English)
                {
                    if (VoiceLinePlay(misc, dog, 8))
                    {
                        atScript.TextChanger("We have a report of a lost dog, he responds to the name “Fire” please help find him by calling for him in your neighbourhood.");
                        dogBool = true;
                    }
                }
                else
                {
                    if (VoiceLinePlay(misc, dog, 8))
                    {
                        atScript.TextChanger("We hebben een melding van een verloren hond, hij reageert op de naam “Brand”, help hem te vinden door hem in uw buurt te roepen.");
                        dogBool = true;
                    }
                }
            }*/
            else if (!movieBool)
            {
                if (English)
                {
                    if (VoiceLinePlay(misc, movie, 7))
                    {
                        movieBool = true;
                        atScript.TextChanger("A new movie is coming out this weekend! It’s about how newscasters should make more money and it’s starring me");
                    }
                }
                else
                {
                    if (VoiceLinePlay(misc, movie, 8))
                    {
                        movieBool = true;
                        atScript.TextChanger("'Dit weekend komt er een nieuwe film uit! Het gaat over hoe nieuwslezers meer geld zouden moeten verdienen en ik speel de hoofdrol.");
                    }
                }
            }
        }
    }
    private IEnumerator SubscribeWaiter()
    {
        while (GameManager.time < 1)
        {
            yield return new WaitForSeconds(1);
        }
        BuildingSystem.onBuildingPlaced += OnBuildingPlaced;
    }
}