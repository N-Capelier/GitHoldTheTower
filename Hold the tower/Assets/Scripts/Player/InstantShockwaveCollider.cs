using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantShockwaveCollider : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Wall"))
		{
			BlockBehaviour block = other.GetComponent<BlockBehaviour>();
			if(block.isDestroyable)
			{
				block.isExploding = true;
				GameObject.Find("GameManager").GetComponent<ThemeInteraction>().CmdExplode(block.blockID);
			}
		}
	}
}
