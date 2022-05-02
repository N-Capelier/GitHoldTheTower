using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectionDisplayManager : MonoBehaviour
{
    [SerializeField]
    private GameObject mapButtonPrefab;
    [SerializeField]
    private RectTransform mapSelectionContent;
    [SerializeField]
    private List<ScriptableMaps> allMaps;
    private List<GameObject> allMapsButtons = new List<GameObject>();

    public float rightOffset;
    public float leftOffset;
    public float verticalOffset;
    public float startVerticalOffset;

    private void Start()
    {
        for (int i = 0; i < allMaps.Count; i++)
        {
            GameObject newMap = Instantiate(mapButtonPrefab, mapSelectionContent);

            newMap.name = allMaps[i].mapName;
            newMap.GetComponent<MapSelectionButton>().mapParams = allMaps[i];
            newMap.GetComponent<RectTransform>().anchoredPosition = new Vector2(((i % 2) == 0) ? leftOffset : rightOffset, startVerticalOffset + (Mathf.Floor(i / 2) * verticalOffset));

            allMapsButtons.Add(newMap);
        }
    }
}
