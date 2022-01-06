using UnityEngine;
using Mirror;

public class ButtonManager : NetworkBehaviour
{
	public BlockBehaviour[] switchables;
	[SerializeField]
	private float activationCooldown;

	public float cooldownRemaining;

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
    }

    public float GetCDRatio()
    {
        return (activationCooldown - cooldownRemaining) / activationCooldown;
    }
}
