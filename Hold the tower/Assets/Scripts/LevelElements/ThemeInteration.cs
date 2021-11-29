using Mirror;

public class ThemeInteration : NetworkBehaviour
{
	[Command(requiresAuthority = false)]
	public void CmdExplode(int _index)
	{
		print("CmdExplode");
		RpcExplode(_index);
	}

	[ClientRpc]
	void RpcExplode(int _index)
	{
		print("RpcExplode");
		ThemeManager.Instance.blocks[_index].StartCoroutine(ThemeManager.Instance.blocks[_index].ExplodeCoroutine());
	}

	[Command(requiresAuthority = false)]
	public void CmdWaitAndExplode(int _index)
	{
		print("CmdWaitAndExplode");
		RpcWaitAndExplode(_index);
	}

	[ClientRpc]
	public void RpcWaitAndExplode(int _index)
	{
		print("RpcWaitAndExplode");
		ThemeManager.Instance.blocks[_index].StartCoroutine(ThemeManager.Instance.blocks[_index].WaitAndExplode());
	}
}
