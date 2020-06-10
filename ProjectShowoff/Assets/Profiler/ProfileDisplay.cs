using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;
using UnityEngine;
using SFB;

public class ProfileDisplay : MonoBehaviour
{
    public Profile profile;

    public System.Action onLoadProfile;

    public void LoadData()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Data file", "", "data", false);

        if (paths.Length == 0)
            return;

        string path = paths[0];

        Profile newProfile = new Profile();

        if (File.Exists(path))
        {
            Debug.Log("Loading file: " + path);
            FileStream file = File.Open(path, FileMode.Open);

            try
            {
                newProfile = (Profile)new BinaryFormatter().Deserialize(file);
                profile = newProfile;

                Debug.Log("Loaded file: " + path);
                onLoadProfile?.Invoke();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
            finally
            {
                file.Close();
            }
        }
        else
        {
            Debug.LogWarning("File not found");
        }

    }
}
