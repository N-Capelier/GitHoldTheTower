using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ChunckSwitcher : MonoBehaviour
{
    public Material readyMaterial;
    public Material rechargingMaterial;
    public GameObject highlight;
    public ParticleSystem useEffect;
    public float activationRange = 12f;
    public ButtonManager linkedChunck;
    public BlockBehaviour linkedButton;
    public ParticleSystem chunkParticle;

    private MeshRenderer display;

    private void Start()
    {
        display = GetComponent<MeshRenderer>();
        UnSelect();
    }

    private void Update()
    {
        display.sharedMaterial = linkedChunck.GetCDRatio() == 1 ? readyMaterial : rechargingMaterial;
    }

    public bool IsChunckReadyToSwitch()
    {
        return true;
    }

    public void SwitchChunck()
    {
        GameObject.Find("GameManager").GetComponent<ThemeInteraction>().CmdSwitchChunck(linkedButton.blockID);
    }

    public void Use()
    {
        useEffect.Play();
    }

    public void Select()
    {
        //highlight.SetActive(true);
        if(!chunkParticle.isPlaying)
        {
            Debug.Log("play");
            chunkParticle.Play();
        }
    }

    public void UnSelect()
    {
        //highlight.SetActive(false);
        chunkParticle.Stop();
        Debug.Log("stop");
    }

}
