using UnityEngine;

public class LevelTerrain : ScriptableObject
{
	public string terrainName;
	[HideInInspector] public Vector3[] positions;
}
