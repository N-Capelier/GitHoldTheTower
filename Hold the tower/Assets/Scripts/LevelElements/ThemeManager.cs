using UnityEngine;

public class ThemeManager : MonoBehaviour
{
	[HideInInspector] public BlockBehaviour[] blocks;

	public LevelTerrain terrain;

	public void LoadActiveTerrain()
	{

	}

#if UNITY_EDITOR
	public void SetupTheme()
	{
		blocks = GetComponentsInChildren<BlockBehaviour>();
	}
#endif
}
