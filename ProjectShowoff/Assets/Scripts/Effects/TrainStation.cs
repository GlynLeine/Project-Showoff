using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainStation : MonoBehaviour
{
    public GameObject trainStation;
    public GameObject airPort;
    public GameObject spaceCenter;

    private int lvl;
    public int level
    {
        get => lvl;
        set
        {
            lvl = value;

            if (lvl <= 0)
            {
                trainStation.SetActive(true);
                airPort.SetActive(false);
                spaceCenter.SetActive(false);
            }
            else if (lvl == 1)
            {
                trainStation.SetActive(false);
                airPort.SetActive(true);
                spaceCenter.SetActive(false);
            }
            else if (lvl > 1)
            {
                trainStation.SetActive(false);
                airPort.SetActive(true);
                spaceCenter.SetActive(false);
            }
        }
    }
}
