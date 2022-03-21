using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameDataGatherer : MonoBehaviour
{
	InGameData data;

	private void Start()
	{
		data = new InGameData();
	}

	public void SendInGameData()
	{

	}

	public InGameData RetrieveInGameData()
	{
		return data;
	}
}
