using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public Object scene;

    public void sceneChangeClick()
    {
        GameManager.AppendDataAndOverwrite();
        SceneManager.LoadScene(0);
    }
}
