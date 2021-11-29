using UnityEngine;
using System.Collections.Generic;
using Mirror;

public class ThemeManager : Singleton<ThemeManager>
{
	[HideInInspector] public BlockBehaviour[] blocks;

	public List<LevelTerrain> terrains = new List<LevelTerrain>();

	public LevelTerrain activeTerrain;

	public BlockBehaviour[][] areas = new BlockBehaviour[3][];

#if UNITY_EDITOR

	public void InitTerrainBlocks()
	{
		blocks = GetComponentsInChildren<BlockBehaviour>();
	}

#endif

	private void Awake()
	{
		CreateSingleton();
	}

	public void LoadTerrain(LevelTerrain _terrain)
	{
		activeTerrain = _terrain;

		blocks = GetComponentsInChildren<BlockBehaviour>();

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
		}

		for (int i = 0; i < blocks.Length; i++)
		{
			blocks[i].SetTargetPosition(activeTerrain.positions[i]);
		}
	}
}
