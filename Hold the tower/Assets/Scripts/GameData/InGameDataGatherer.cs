using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameDataGatherer : Singleton<InGameDataGatherer>
{
	public InGameData data;
	public Sprite mapImage;
	public string mapText;

	private void Awake()
	{
		CreateSingleton(true);
	}

	public void CreateNewInGameData()
	{
		data = new InGameData();
	}

	public void FillMapData(Sprite imageMap, string textMap)
    {
		mapImage = imageMap;
		mapText = textMap;
	}
}
