using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameDataGatherer : Singleton<InGameDataGatherer>
{
	public InGameData data;

	private void Awake()
	{
		CreateSingleton(true);
	}

	public void CreateNewInGameData()
	{
		data = new InGameData();
	}
}
