using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapDisplay : MonoBehaviour
{

    public ScriptableMaps mapParams;

    [Header("UI références")]

    [SerializeField]
    private Text textDescription;
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Image mapImage;

    [SerializeField]
    private Button hostButton;
    [SerializeField]
    private Button joinButton;

    public void Start()
    {
        MenuManager menuManager = GameObject.Find("MenuManager").GetComponent<MenuManager>();

        textDescription.text = mapParams.textDescription;
        nameText.text = mapParams.mapName;
        mapImage.color = new Color(255, 255, 255, 255);
        mapImage.sprite = mapParams.mapImage;

        hostButton.onClick.AddListener(() => { menuManager.OnPressedHost(mapParams.sceneNameToLoad);});
        joinButton.onClick.AddListener(() => { menuManager.OnPressedHost(mapParams.sceneNameToLoad);});

    }


}
