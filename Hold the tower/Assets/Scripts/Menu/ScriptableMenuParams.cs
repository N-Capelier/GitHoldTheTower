using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Menu Params", menuName = "Menu/Params")]
[System.Serializable]
public class ScriptableMenuParams : ScriptableObject
{
    public string playerPseudo;
    public string ipToJoin;
}
