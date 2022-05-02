using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectionButton : MonoBehaviour
{

    public ScriptableMaps mapParams;

    [Header("UI références")]

    [SerializeField]
    private Text textDescription;
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Image mapImage;


    private MenuManager menuManager;

    public void Start()
    {
        menuManager = GameObject.Find("MenuManager").GetComponent<MenuManager>();

        textDescription.text = mapParams.textDescription;
        nameText.text = mapParams.mapName;
        mapImage.sprite = mapParams.mapImage;
    }

    public void SelectMap()
    {
        menuManager.OnPressedHost(mapParams.sceneNameToLoad);
    }

    public void PlaySound()
    {
        SoundManager.Instance.PlayUIEvent("UIButtonClick");
    }
}
