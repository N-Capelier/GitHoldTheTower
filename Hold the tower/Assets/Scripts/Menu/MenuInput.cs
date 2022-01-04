using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuInput : MonoBehaviour
{
    // Start is called before the first frame update
    public Text forwardText, backText, leftText, rightText, jumpText;
    public Text mouseSensivityText;

    [SerializeField]
    private ScriptableParamsPlayer paramsPlayer;

    private void Update()
    {
        forwardText.text = paramsPlayer.front.ToString();
        backText.text = paramsPlayer.back.ToString();
        leftText.text = paramsPlayer.left.ToString();
        rightText.text = paramsPlayer.right.ToString();
        jumpText.text = paramsPlayer.jump.ToString();
        mouseSensivityText.text = paramsPlayer.mouseSensivity.ToString();
    }
}
