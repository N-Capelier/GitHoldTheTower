using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelector : MonoBehaviour
{
    [SerializeField]
    private GameObject mapObjectType;
    [SerializeField]
    private List<ScriptableMaps> allMaps;
    private List<GameObject> allMapsObject = new List<GameObject>();

    private int mapChooseNumber;

    private const float widthNormal = 512;
    private const float heightNormal = 361.75f;
    public void Start()
    {
        for(int i = 0; i < allMaps.Count; i++)
        {
            GameObject newMap = Instantiate(mapObjectType, this.transform);
            newMap.transform.SetParent(this.transform);
            newMap.name = allMaps[i].mapName;
            newMap.GetComponent<MapDisplay>().mapParams = allMaps[i];
            allMapsObject.Add(newMap);
        }

        if (allMapsObject.Count > 1)
        {
            for(int i = 1; i < allMapsObject.Count; i++)
            {
                allMapsObject[i].SetActive(false);
            }
        }
    }

    #region Boutton

    public void PressedLeft()
    {
        allMapsObject[mapChooseNumber].SetActive(false);
        mapChooseNumber--;
        if(mapChooseNumber < 0)
        {
            mapChooseNumber = allMapsObject.Count - 1;
        }
        allMapsObject[mapChooseNumber].SetActive(true);
    }

    public void PressedRight()
    {
        allMapsObject[mapChooseNumber].SetActive(false);
        mapChooseNumber++;
        if (mapChooseNumber > allMapsObject.Count-1)
        {
            mapChooseNumber = 0;
        }
        allMapsObject[mapChooseNumber].SetActive(true);
    }

    private void TransitionToFront()
    {

    }

    #endregion
}
