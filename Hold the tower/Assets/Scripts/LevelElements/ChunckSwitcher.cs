using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ChunckSwitcher : MonoBehaviour
{
    public float activationRange = 12f;
    public ButtonManager linkedChunck;
    public BlockBehaviour linkedButton;

    public bool IsChunckReadyToSwitch()
    {
        return true;
    }

    public void SwitchChunck()
    {
        GameObject.Find("GameManager").GetComponent<ThemeInteraction>().CmdSwitchChunck(linkedButton.blockID);
    }
}
