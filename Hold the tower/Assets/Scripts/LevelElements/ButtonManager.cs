using UnityEngine;
using Mirror;

public class ButtonManager : NetworkBehaviour
{
	public BlockBehaviour[] switchables;
	[SerializeField]
	private float activationCooldown;

	[HideInInspector] public float cooldownRemaining;
    [HideInInspector] public bool isHighlighted;

    private void Start()
    {
        for (int i = 0; i < switchables.Length; i++)
        {
            switchables[i].isInChunck = true;
        }
    }

    private void Update()
    {
        if(cooldownRemaining > 0)
        {
            cooldownRemaining -= Time.deltaTime;
        }
        else
        {
            cooldownRemaining = 0;
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdUse()
    {
        cooldownRemaining = activationCooldown;
        RpcUse();
    }

    [ClientRpc]
    public void RpcUse()
    {
        cooldownRemaining = activationCooldown;
        for (int i = 0; i < switchables.Length; i++)
        {
            switchables[i].chunkActivableParticle.Play();
        }
    }

    public float GetCDRatio()
    {
        return (activationCooldown - cooldownRemaining) / activationCooldown;
    }

    public void HighlightChunck(bool doHighlight)
    {
        if((isHighlighted && !doHighlight) || (!isHighlighted && doHighlight))
        {
            isHighlighted = doHighlight;
            for (int i = 0; i < switchables.Length; i++)
            {
                switchables[i].highlightDisplay.SetActive(doHighlight);
                if(doHighlight && GetCDRatio() >= 1)
                {
                    switchables[i].StartMovementPreview();
                }
                else
                {
                    switchables[i].StopMovementPreview();
                }
                switchables[i].isSelected = doHighlight;
            }
        }
    }

    public void StartAllMovementPreview()
    {
        for (int i = 0; i < switchables.Length; i++)
        {
            switchables[i].StartMovementPreview();
        }
    }

    public void StopAllMovementPreview()
    {
        for (int i = 0; i < switchables.Length; i++)
        {
            switchables[i].StopMovementPreview();
        }
    }
}
