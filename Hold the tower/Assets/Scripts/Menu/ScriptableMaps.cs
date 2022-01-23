using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Map params",menuName ="Map",order =0)]
public class ScriptableMaps : ScriptableObject
{
    public Sprite mapImage;
    public Sprite mapOverview;
    public string mapName;
    public string textDescription;

    public string sceneNameToLoad;
}
