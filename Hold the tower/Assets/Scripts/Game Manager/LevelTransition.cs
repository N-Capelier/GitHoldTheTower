using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class LevelTransition : NetworkBehaviour
{
    [SerializeField]
    private GameObject ThemeObject;

    private double networkTime = 0;
    private int niveau = 0;

    private struct LevelInformation : NetworkMessage
    {
        float niveau;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnChangeTerrain()
    {
        networkTime = NetworkTime.time;
        if (niveau == 0)
            niveau = 1;
        else
            niveau = 0;

        ThemeObject.GetComponent<ThemeManager>().LoadTerrain(ThemeObject.GetComponent<ThemeManager>().terrains[niveau]);

    }

    // Update is called once per frame
    void Update()
    {
        
        if(NetworkTime.time >= networkTime + 10d)
        {
            OnChangeTerrain();
        }
    }
}
