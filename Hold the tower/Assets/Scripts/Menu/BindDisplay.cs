using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BindDisplay : MonoBehaviour
{
    [SerializeField]
    private ScriptableParamsPlayer paramsPlayer;

    [SerializeField]
    private Text textForwardBind;
    [SerializeField]
    private Text textLeftBind;
    [SerializeField]
    private Text textRightBind;
    [SerializeField]
    private Text textBehindBind;
    [SerializeField]
    private Text textJumpBind;

    [SerializeField]
    private Text textSensy;
    [SerializeField]
    private Slider sliderSensy;

    private void Start()
    {
        SaveManager.LoadParams(ref paramsPlayer);
        DisplayKey();
        DisplaySensy();
        sliderSensy.value = paramsPlayer.mouseSensivity;
    }
    public void DisplayKey()
    {
        textForwardBind.text = paramsPlayer.front.ToString();
        textLeftBind.text = paramsPlayer.left.ToString();
        textRightBind.text = paramsPlayer.right.ToString();
        textBehindBind.text = paramsPlayer.back.ToString();
        textJumpBind.text = paramsPlayer.jump.ToString();
    }

    public void DisplaySensy()
    {
        textSensy.text = paramsPlayer.mouseSensivity.ToString();
    }
}
