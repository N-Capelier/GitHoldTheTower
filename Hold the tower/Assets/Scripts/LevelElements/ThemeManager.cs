using UnityEngine;
using System.Collections.Generic;
using Mirror;

public class ThemeManager : Singleton<ThemeManager>
{
	[HideInInspector] public BlockBehaviour[] blocks;

	public List<LevelTerrain> terrains = new List<LevelTerrain>();

	public LevelTerrain activeTerrain;

	public BlockBehaviour[][] areas = new BlockBehaviour[3][];

	public GameObject buttonActivationEffectPrefab;

	private LevelTransition levelTransition;

#if UNITY_EDITOR

	public void InitTerrainBlocks()
	{
		blocks = GetComponentsInChildren<BlockBehaviour>();
		UnityEditor.EditorUtility.SetDirty(this);
		UnityEditor.AssetDatabase.SaveAssets();
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

    private void Update()
    {
		UpdateFlickeringBeforAutoSwitch();
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

	public void LoadNextStateForChunck(int blockIndex)
	{
		for (int i = 0; i < blocks[blockIndex].buttonManager.switchables.Length; i++)
		{
			blocks[blockIndex].buttonManager.switchables[i].SetNextTerrainPosition();
		}
	}

	public void ResetDestroyedBlocks()
	{
		for (int i = 0; i < blocks.Length; i++)
		{
			blocks[i].SetBlockAlive();
		}
	}


	float evolveWarningTimeLeft;
	float timerBeforeNextTransition;
	bool warningEndFlag;
	GameObject gameManager;
	private void UpdateFlickeringBeforAutoSwitch()
	{
		if(levelTransition == null)
        {
			if(gameManager != null)
			{
				levelTransition = gameManager.GetComponent<LevelTransition>();
				for (int i = 0; i < blocks.Length; i++)
				{
					blocks[i].levelTransition = levelTransition;
				}
			}
			else
			{
				gameManager = GameObject.Find("GameManager");
			}
		}
		else
		{
			timerBeforeNextTransition = (float)(levelTransition.timerChange - (NetworkTime.time - levelTransition.networkTime));


			if (timerBeforeNextTransition < 6f)
			{
				evolveWarningTimeLeft -= Time.deltaTime;
				warningEndFlag = true;
				if (evolveWarningTimeLeft <= 0)
				{
					for (int i = 0; i < blocks.Length; i++)
					{
						if (blocks[i].isInChunck)
						{
							if (blocks[i].meshRenderer.sharedMaterial == blocks[i].blockMaterial)
							{
								blocks[i].meshRenderer.sharedMaterial = blocks[i].blockWarnMaterial;
							}
							else
							{
								blocks[i].meshRenderer.sharedMaterial = blocks[i].blockMaterial;
							}
						}
					}

					if (timerBeforeNextTransition < 2f)
					{
						evolveWarningTimeLeft = 0.1f;
					}
					else
					{
						evolveWarningTimeLeft = 0.5f;
					}
				}
			}
			else if (warningEndFlag)
			{
				for (int i = 0; i < blocks.Length; i++)
				{
					if (blocks[i].isInChunck)
					{
						blocks[i].meshRenderer.sharedMaterial = blocks[i].blockMaterial;
					}
				}
				warningEndFlag = false;
				evolveWarningTimeLeft = 0;
			}
		}
	}
}
