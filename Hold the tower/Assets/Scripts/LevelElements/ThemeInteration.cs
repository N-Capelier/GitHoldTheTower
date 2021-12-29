using Mirror;

public class ThemeInteration : NetworkBehaviour
{
	[Command(requiresAuthority = false)]
	public void CmdExplode(int _index)
	{
		RpcExplode(_index);
	}

	[ClientRpc]
	void RpcExplode(int _index)
	{
		ThemeManager.Instance.blocks[_index].StartCoroutine(ThemeManager.Instance.blocks[_index].ExplodeCoroutine());
	}

	[Command(requiresAuthority = false)]
	public void CmdInstantiateShockwave()
	{
		RpcInstantiateShockwave();
	}

	[ClientRpc]
	public void RpcInstantiateShockwave()
	{

	}

	[Command(requiresAuthority = false)]
	public void CmdWaitAndExplode(int _index)
	{
		RpcWaitAndExplode(_index);
	}

	[ClientRpc]
	public void RpcWaitAndExplode(int _index)
	{
		ThemeManager.Instance.blocks[_index].StartCoroutine(ThemeManager.Instance.blocks[_index].WaitAndExplode());
	}

	[Command(requiresAuthority = false)]
	public void CmdSwitchArea(int _index)
	{
		RpcSwitchArea(_index);
	}

	[ClientRpc]
	public void RpcSwitchArea(int _index)
	{
		ThemeManager.Instance.LoadTerrainForSwitchArea(_index);
	}
}
