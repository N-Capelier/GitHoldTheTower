using UnityEngine;

public class ThemeManager : MonoBehaviour
{
	[HideInInspector] public BlockBehaviour[] blocks;

	public LevelTheme theme;
	public LevelTerrain terrain;

#if UNITY_EDITOR
	public void SetupTheme()
	{
		blocks = GetComponentsInChildren<BlockBehaviour>();
	}
#endif
}
