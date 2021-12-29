using UnityEngine;

public class ShockwaveCollider : MonoBehaviour
{
	[SerializeField] float minRadius = 0f;
	[SerializeField] float maxRadius = 5f;
	[SerializeField] float maxExpandDuration = 2f;
	[SerializeField] SphereCollider sphereCollider;
	float elapsedTime = 0f;
	bool expanding = false;
	float completion;
	[HideInInspector] public bool hasAuthority = false;

	private void FixedUpdate()
	{
		if (expanding)
		{
			Expand();
		}
	}

	void Expand()
	{
		elapsedTime += Time.fixedDeltaTime;
		completion = elapsedTime / maxExpandDuration;
		sphereCollider.radius = Mathf.Lerp(minRadius, maxRadius, completion);
		if (completion >= 1f)
		{
			expanding = false;
		}
	}

	public void Shock(float _playerPunchLoadFactor)
	{
		maxExpandDuration *= _playerPunchLoadFactor;
		expanding = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Wall") && hasAuthority)
		{
			BlockBehaviour block = other.GetComponent<BlockBehaviour>();

			GameObject.Find("GameManager").GetComponent<ThemeInteration>().CmdWaitAndExplode(block.blockID);
		}
	}

}
