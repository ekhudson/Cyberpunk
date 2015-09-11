using UnityEngine;

using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveLoadController : Singleton<SaveLoadController>
{
    private const string kPlayerDataFileName = "/playerData.dat";

    private static string PlayerDataPath
    {
        get
        {
            return Application.persistentDataPath + kPlayerDataFileName;
        }
    }

    public bool Save(PlayerData playerData)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(PlayerDataPath, FileMode.OpenOrCreate);

        bf.Serialize(file, playerData);
        file.Close();

        Debug.Log(string.Format("Data saved to {0}", PlayerDataPath));
        return true;
    }

    public PlayerData Load()
    {
        if (File.Exists(PlayerDataPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(PlayerDataPath, FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();
            return data;
        }
        else 
        {
            return null;
        }
    }

}

