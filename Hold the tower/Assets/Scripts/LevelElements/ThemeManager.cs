using UnityEngine;

public class ThemeManager : MonoBehaviour
{
	[HideInInspector] public BlockBehaviour[] blocks;

	public LevelTerrain terrain;

	public void LoadActiveTerrain()
	{

	}

#if UNITY_EDITOR

	public void InitTerrainBlocks()
	{
		blocks = GetComponentsInChildren<BlockBehaviour>();
	}

#endif
	public void LoadTerrain(LevelTerrain _terrain)
	{
		terrain = _terrain;

		blocks = GetComponentsInChildren<BlockBehaviour>();

		if (blocks.Length == 0)
		{
			Debug.LogError("No block found in theme");
			return;
		}
		else if (terrain == null)
		{
			Debug.LogError("No terrain found in ThemeManager");
			return;
		}
		else if (blocks.Length != terrain.positions.Length)
		{
			Debug.LogError("Terrain does not correspond to this theme");
		}

		for (int i = 0; i < blocks.Length; i++)
		{
			blocks[i].SetTargetPosition(terrain.positions[i]);
		}
	}
}
