using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class PA_Position : MonoBehaviour
{
    public MatchManager gameManager;
    public string fileToStorePosition;

    public float timeEachBreak;
    private float ownDeltaTime;

    public List<Transform> analyticGameObjectPosition;

    private bool onceInit = true;

    private void Start()
    {
        if (GameObject.Find("ServerManager").GetComponent<MyNewNetworkManager>().analyticsPath != string.Empty)
        {
            fileToStorePosition = GameObject.Find("ServerManager").GetComponent<MyNewNetworkManager>().analyticsPath;
        }
        else
        {
            gameObject.SetActive(false);
        }
        
    }

    void InitAnalytics()
    {
        StreamWriter writer = new StreamWriter(fileToStorePosition, false);
        writer.Write("// ");
        for (int i = 0; i < analyticGameObjectPosition.Count; i++)
        {
            writer.Write("Object " + i.ToString() + "-");
        }
        writer.WriteLine();
        writer.Close();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.startGame)
        {
            if (onceInit)
            {
                onceInit = false;
                InitAnalytics();
            }

            ownDeltaTime += Time.deltaTime;
            if (ownDeltaTime >= timeEachBreak)
            {
                WriteAllObjectPosition();
                ownDeltaTime = 0f;
            }
        }
        
    }

    void OnApplicationQuit()
    {
        StreamWriter writer = new StreamWriter(fileToStorePosition, true);
        DateTime dt = DateTime.Now;
        writer.WriteLine("// " + dt.ToString() + " //");
        writer.Close();
    }

    private void WriteAllObjectPosition()
    {
        StreamWriter writer = new StreamWriter(fileToStorePosition, true);
        foreach(Transform objPosition in analyticGameObjectPosition)
        {
            writer.Write(objPosition.position.x.ToString() + "|" + objPosition.position.y.ToString() + "|" + objPosition.position.z.ToString() + ";");
        }
        writer.Write("\n");
        writer.Close();
    }
}
