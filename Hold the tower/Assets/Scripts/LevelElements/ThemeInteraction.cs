using Mirror;
using UnityEngine;

public class ThemeInteraction : NetworkBehaviour
{
	[SerializeField] GameObject visualShockwavePrefab;

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
	public void CmdInstantiateShockwave(Vector3 _spawnPoint, float _punchRatio)
	{
		RpcInstantiateShockwave(_spawnPoint, _punchRatio);
	}

	[ClientRpc]
	public void RpcInstantiateShockwave(Vector3 _spawnPoint, float _punchRatio)
	{
		GameObject _shockWave = Instantiate(visualShockwavePrefab, _spawnPoint, Quaternion.identity);
		_shockWave.GetComponent<ShockwaveCollider>().Shock(_punchRatio);
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
