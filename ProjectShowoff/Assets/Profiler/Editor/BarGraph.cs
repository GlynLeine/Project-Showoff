using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarGraph : MonoBehaviour
{
    public ProfileDisplay display;
    public GameObject graphBarPrefab;
    public GameObject graphSeperatorPrefab;

    public RectTransform graphArea;

    public Text minRangeText;
    public Text maxRangeText;

    public Text minStampText;
    public Text maxStampText;
    public Slider timeStampSlider;

    public Text minStartText;
    public Text maxStartText;
    public Slider timeStartSlider;


    private void Awake()
    {
        display.onLoadProfile += UpdateGraph;

        timeStampSlider.onValueChanged.AddListener(OnSliderChange);
        timeStartSlider.onValueChanged.AddListener(OnSliderChange);
    }

    public int startStamp;
    public int displayStamps;

    public void OnSliderChange(float value)
    {
        displayStamps = (int)timeStampSlider.value;
        startStamp = (int)timeStartSlider.value;
        UpdateGraph();
    }

    public void UpdateGraph()
    {
        Profile profile = display.profile;
        if (profile == null)
            return;

        Debug.Log("Updating bar graph");

        for (int i = 0; i < graphArea.childCount; i++)
            Destroy(graphArea.GetChild(i).gameObject);

        int stampCount = profile.timeMap.Length;

        if (stampCount == 0)
        {
            Debug.Log("Profile is empty.");
            return;
        }

        startStamp = Mathf.Clamp(startStamp, 0, stampCount);
        displayStamps = Mathf.Clamp(displayStamps, 1, stampCount - startStamp);

        minRangeText.text = "0";
        maxRangeText.text = profile.maxDelta.ToString();

        minStampText.text = "1";
        maxStampText.text = profile.timeMap.Length.ToString();
        timeStampSlider.minValue = 1;
        timeStampSlider.maxValue = profile.timeMap.Length;

        minStartText.text = "0";
        maxStartText.text = (profile.timeMap.Length - displayStamps).ToString();
        timeStartSlider.minValue = 0;
        timeStartSlider.maxValue = profile.timeMap.Length - displayStamps;

        for (int j = startStamp; j < startStamp + displayStamps; j++)
        {
            int startIndex = profile.timeMap[j];

            int endIndex = profile.deltaTimes.Length;
            if (j <= stampCount - 2)
                endIndex = profile.timeMap[j + 1];

            endIndex = Mathf.Clamp(endIndex, 0, profile.deltaTimes.Length - 1);

            for (int i = startIndex; i < endIndex; i++)
            {
                GameObject bar = Instantiate(graphBarPrefab, graphArea);
                Slider slider = bar.GetComponent<Slider>();
                slider.value = profile.deltaTimes[i] / profile.maxDelta;
                slider.colors = new ColorBlock() { disabledColor = new Color(slider.value, 1f - slider.value, 0), colorMultiplier = 1f };
                LayoutElement layout = bar.GetComponent<LayoutElement>();
                layout.preferredWidth = profile.deltaTimes[i];
            }

            if (j + 1 < startStamp + displayStamps)
            {
                GameObject seperator = Instantiate(graphSeperatorPrefab, graphArea);
                LayoutElement layout = seperator.GetComponent<LayoutElement>();
                layout.preferredWidth = profile.timeStamps[j + 1] - profile.timeStamps[j];
            }
        }

    }
}
