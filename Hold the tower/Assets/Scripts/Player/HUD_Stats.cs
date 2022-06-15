using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUD_Stats : MonoBehaviour
{
    [SerializeField]
    private TMP_Text mapText;
    [SerializeField]
    private Image mapImage;

    [SerializeField]
    private TMP_Text victoryLooseText;
    [SerializeField]
    private TMP_Text killsText;
    [SerializeField]
    private TMP_Text pointsText;
    [SerializeField]
    private TMP_Text timeText;


    public void InitHudStats(string mapName, Sprite mapSprite, string victoryLoose, string kills, string points, string time)
    {
        this.gameObject.SetActive(true);
        mapText.text = mapName;
        mapImage.sprite = mapSprite;
        victoryLooseText.text = victoryLoose;
        killsText.text = kills;
        pointsText.text = points;
        timeText.text = time;
    }
}
