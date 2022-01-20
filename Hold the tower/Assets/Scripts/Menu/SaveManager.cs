using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

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
        FileStream write =  File.Create(Application.dataPath + "/Saves/menuParams.json");
        string json = JsonUtility.ToJson(menuParams);
        bf.Serialize(write, json);
        write.Close();
    }

    public static void LoadParams(ref ScriptableMenuParams menuParams)
    {
        if (!File.Exists(Application.dataPath + "/Saves/menuParams.json"))
        {
            SaveParams(menuParams);
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.dataPath + "/Saves/menuParams.json", FileMode.Open);
        JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), menuParams) ;
        file.Close();
    }

    /*public static void SaveParams(ScriptableParamsPlayer playerParams)
    {
        if (!File.Exists(Application.dataPath + "/Saves/menuParams"))
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
    }*/

    public static void LoadParams(ref Dictionary<string, string> ipToSave)
    {
        if (!File.Exists(Application.dataPath + "/Saves/IpList.json"))
        {
            SaveParams(ref ipToSave);
        }

        using (StreamReader file = File.OpenText(Application.dataPath + "/Saves/IpList.json"))
        {
            JsonSerializer serializer = new JsonSerializer();
            ipToSave = (Dictionary<string, string>)serializer.Deserialize(file, typeof(Dictionary<string, string>));
        }
    }

    public static void SaveParams(ref Dictionary<string,string> ipToSave)
    {
        if (!File.Exists(Application.dataPath + "/Saves/IpList.json"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Saves");
        }

        using (StreamWriter file = File.CreateText(Application.dataPath + "/Saves/IpList.json"))
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, ipToSave);
        }

    }
}
