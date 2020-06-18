using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;

[System.Serializable]
public class Profile
{
    public float[] timeStamps;
    public int[] timeMap;
    public float[] deltaTimes;
    public float minDelta;
    public float maxDelta;
}

public class Profiler : MonoBehaviour
{
    private Mutex dataMutex;
    private float lastDelta;
    private List<float> timeStamps;
    private List<int> timeMap;
    private List<float> deltas;
    private float min;
    private float max;


    private string fileDir;

    private Mutex logMutex;
    private string log;
    // Use this for initialization
    void Start()
    {
        dataMutex = new Mutex();
        lastDelta = 0;
        timeStamps = new List<float>();
        timeMap = new List<int>();
        deltas = new List<float>();
        min = float.MaxValue;
        max = float.MinValue;
        logMutex = new Mutex();

        string path = Application.persistentDataPath + "/Data/";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        fileDir = path + "profile.data";

        Debug.Log(fileDir);

        StartCoroutine(PeriodicSave());
    }

    IEnumerator PeriodicSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            new Thread(new ThreadStart(Save)).Start();
        }
    }

    void Save()
    {
        FileStream file = File.Create(fileDir);

        Profile profile = new Profile();

        dataMutex.WaitOne();
        profile.timeStamps = timeStamps.ToArray();
        profile.timeMap = timeMap.ToArray();
        profile.deltaTimes = deltas.ToArray();
        profile.minDelta = min;
        profile.maxDelta = max;
        dataMutex.ReleaseMutex();

        try
        {
            new BinaryFormatter().Serialize(file, profile);
        }
        catch (SerializationException e)
        {
            logMutex.WaitOne();
            log = e.Message;
            logMutex.ReleaseMutex();
        }
        finally
        {
            file.Close();
        }
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;
        dataMutex.WaitOne();
        if (delta > 0.029f)
        {
            if (lastDelta <= 0.029f)
            {
                timeStamps.Add(Time.time);
                timeMap.Add(deltas.Count);
            }

            deltas.Add(delta);
        }

        lastDelta = delta;

        if (delta < min)
            min = delta;
        if (delta > max)
            max = delta;

        dataMutex.ReleaseMutex();

        logMutex.WaitOne();
        if (log != null)
        {
            Debug.Log(log);
            log = null;
        }
        logMutex.ReleaseMutex();
    }
}
