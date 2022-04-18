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

	private void Start()
	{
		data = new InGameData();
	}

	public void ResetData()
	{
		data = new InGameData();
	}
}
