using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetReset : MonoBehaviour
{
    [SerializeField] private string loadSceneName = "Planet";
    [SerializeField] private TMP_Text secondsLeft = null;
    [SerializeField] private GameObject resetPanel = null;
    
    private float timer = 0;
    private bool resetPressed = false;
    
    public void SceneResetButton()
    {
        resetPressed = true;
        resetPanel.SetActive(true);
    }

    public void StopResetButton()
    {
        resetPressed = false;
        timer = 0;
        resetPanel.SetActive(false);
    }
    void Update()
    {
        if (resetPressed)
        {
            timer += Time.deltaTime;
            secondsLeft.text = Mathf.CeilToInt(5-timer).ToString();
            if (timer > 5)
            {
                SceneManager.LoadScene(loadSceneName);
            }
        }
    }
}
