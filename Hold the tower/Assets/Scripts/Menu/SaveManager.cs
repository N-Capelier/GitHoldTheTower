using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public enum saveFile
{
    menu,
    playerOption
}

public class SaveManager
{
    public static bool IsSaveFile()
    {
        return Directory.Exists(Application.dataPath + "/Saves");
    }

    public static void SaveParams(ScriptableMenuParams menuParams)
    {
        if (!IsSaveFile())
        {
            Directory.CreateDirectory(Application.dataPath + "/Saves");
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream write =  File.Create(Application.dataPath + "/Saves/menuParams");
        string json = JsonUtility.ToJson(menuParams);
        bf.Serialize(write, json);
        write.Close();
    }

    public static void LoadParams(ref ScriptableMenuParams menuParams)
    {
        if (!IsSaveFile())
        {
            SaveParams(menuParams);
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.dataPath + "/Saves/menuParams", FileMode.Open);
        JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), menuParams) ;
        file.Close();
    }

    public static void SaveParams(ScriptableParamsPlayer playerParams)
    {
        if (!IsSaveFile())
        {
            Directory.CreateDirectory(Application.dataPath + "/Saves");
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream write = File.Create(Application.dataPath + "/Saves/menuParams");
        string json = JsonUtility.ToJson(playerParams);
        bf.Serialize(write, json);
        write.Close();
    }

    public static void LoadParams(ref ScriptableParamsPlayer playerParams)
    {
        if (!IsSaveFile())
        {
            SaveParams(playerParams);
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.dataPath + "/Saves/menuParams", FileMode.Open);
        JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), playerParams);
        file.Close();
    }
}
