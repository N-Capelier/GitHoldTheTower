using UnityEngine;
using System.Collections.Generic;

public class ThemeManager : Singleton<ThemeManager>
{
	[HideInInspector] public BlockBehaviour[] blocks;

	public List<LevelTerrain> terrains = new List<LevelTerrain>();

	public LevelTerrain activeTerrain;

	public BlockBehaviour[][] areas = new BlockBehaviour[3][];

	public GameObject buttonActivationEffectPrefab;

#if UNITY_EDITOR

	public void InitTerrainBlocks()
	{
		blocks = GetComponentsInChildren<BlockBehaviour>();
	}

#endif

	private void Awake()
	{
		CreateSingleton();

		blocks = GetComponentsInChildren<BlockBehaviour>();

		for (int i = 0; i < blocks.Length; i++)
        {
			blocks[i].blockID = i;
        }
	}

	public void LoadNextTerrain()
	{
		if (blocks.Length == 0)
		{
			Debug.LogError("No block found in theme");
			return;
		}
		else if (activeTerrain == null)
		{
			Debug.LogError("No terrain found in ThemeManager");
			return;
		}
		else if (blocks.Length != activeTerrain.positions.Length)
		{
			Debug.LogError("Terrain does not correspond to this theme");
			return;
		}

		foreach (BlockBehaviour _block in blocks)
		{
			_block.SetNextTerrainPosition();
		}
	}

	public void LoadTerrainForSwitchArea(int _index)
	{
		for (int i = 0; i < blocks[_index].buttonManager.switchables.Length; i++)
		{
			blocks[_index].buttonManager.switchables[i].SetNextTerrainPosition();
		}
	}
}
