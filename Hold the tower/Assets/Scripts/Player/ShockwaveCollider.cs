using UnityEngine;

public class ShockwaveCollider : MonoBehaviour
{
	[SerializeField] bool isVisual = false;

	[SerializeField] float minRadius = 0f;
	[SerializeField] float maxRadius = 5f;
	[SerializeField] float maxExpandDuration = 2f;
	float expandDuration = 2f;
	[SerializeField] SphereCollider sphereCollider;
	float elapsedTime = 0f;
	bool expanding = false;
	float completion;
	[SerializeField] GameObject shockwaveRenderer;

	private void FixedUpdate()
	{
		if (expanding)
		{
			Expand();
		}
	}

	//private void OnDrawGizmos()
	//{
	//	if (!expanding)
	//		return;

	//	Gizmos.color = Color.blue;
	//	Gizmos.DrawSphere(transform.position, sphereCollider.radius);
	//}

	void Expand()
	{
		elapsedTime += Time.fixedDeltaTime;
		completion = elapsedTime / expandDuration;
		sphereCollider.radius = Mathf.Lerp(minRadius, maxRadius, completion);

		if(isVisual)
		{
			shockwaveRenderer.transform.localScale = new Vector3(sphereCollider.radius * 2f, sphereCollider.radius * 2f, sphereCollider.radius * 2f);
		}

		if (completion >= 1f)
		{
			expanding = false;
			Destroy(gameObject);
		}
	}

	public void Shock(float _playerPunchLoadFactor)
	{
		expandDuration = (1 - _playerPunchLoadFactor) * maxExpandDuration;
		expandDuration = expandDuration.Remap(0f, maxExpandDuration, .5f, maxExpandDuration);

		maxRadius *= _playerPunchLoadFactor;
		if(_playerPunchLoadFactor == 0f)
			maxRadius = minRadius;

		expanding = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (isVisual)
			return;

		if (other.CompareTag("Wall"))
		{
			BlockBehaviour block = other.GetComponent<BlockBehaviour>();
			if(block.isDestroyable && !block.isExploding)
			{
				GameObject.Find("GameManager").GetComponent<ThemeInteraction>().CmdWaitAndExplode(block.blockID);
			}
		}
	}

}
