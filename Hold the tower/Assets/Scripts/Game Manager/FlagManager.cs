using System.Collections;
using UnityEngine;

public class FlagManager : MonoBehaviour
{
	[SerializeField] BoxCollider boxCollider;

	private void Start()
	{
		StartCoroutine(EnableCollider());
	}

	IEnumerator EnableCollider()
	{
		yield return new WaitForSeconds(3f);
		boxCollider.enabled = true;
	}
}
