using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Globalization;

public class PA_RenderPosition : MonoBehaviour
{
    public string pathFilePositions;

    //Can draw all the players paths
    public bool drawnGizmos;
    public List<List<Vector3>> playersKeyPosition = new List<List<Vector3>>();
    public List<Color> colors;

    void OnDrawGizmos()
    {

        //Gizmos.DrawLine(new Vector3(0,0,0), new Vector3(0, 100, 0));
        int j = 0;
        foreach(List<Vector3> KeyPositions in playersKeyPosition)
        {
            Gizmos.color = colors[j];
            if (KeyPositions.Count > 3) // sup to 3 because there is min 2 lines per files
            {
                for (int i = 1; i < KeyPositions.Count; i++)
                {
                    Gizmos.DrawLine(KeyPositions[i - 1], KeyPositions[i]);
                }
            }
            j++; 
        }

    }

    public void LoadKeyPositions()
    {
        playersKeyPosition.Clear();
        try
        {
            StreamReader sr = new StreamReader(pathFilePositions);

            string firstLine = sr.ReadLine();
            string[] allPlayersInLine = firstLine.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            
            //Colors add or remove
            if(colors.Count > allPlayersInLine.Length)
            {
                while(colors.Count != allPlayersInLine.Length)
                {
                    colors.RemoveAt(colors.Count-1);
                }
            }
            if (colors.Count < allPlayersInLine.Length)
            {
                while (colors.Count != allPlayersInLine.Length)
                {
                    colors.Add(new Color());
                }
            }

            foreach (string players in allPlayersInLine)
            {
                if(players.Length > 1)
                {
                    playersKeyPosition.Add(new List<Vector3>());
                    
                }
            }
            
            //Read and parse file position
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (!line.Contains("//"))
                {
                    //Find each object
                    string[] playersPositions = line.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    //Parse position for an object
                    for(int i = 0; i < playersPositions.Length; i++)
                    {
                        string[] playerPosition = playersPositions[i].Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        float _x = float.Parse(playerPosition[0]);
                        float _y = float.Parse(playerPosition[1]);
                        float _z = float.Parse(playerPosition[2]);

                        playersKeyPosition[i].Add(new Vector3(_x, _y, _z));
                    }
  
                }
            }
            sr.Close();
        }catch(Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
}
