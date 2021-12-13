using System.Collections;
using UnityEngine;
using Mirror;

public class BlockBehaviour : MonoBehaviour
{
	[Header("Components")]
	[SerializeField] BoxCollider boxCollider;

	[Header("Movement")]
	//Movement
	[SerializeField] [Range(0.1f, 15f)] float moveDuration = 1.2f;
	Vector3 startPosition;
	Vector3 targetPosition;
	bool movingToTargetPos;
	float elapsedTime = 0f;
	float completion;
	[HideInInspector] public Vector3 ownVelo = Vector3.zero; //networking update

	[Header("Destruction")]
	//Explosion
	[SerializeField] [Range(0f, 5f)] float timeBeforeExplosion = 2f;
	[SerializeField] [Range(0f, 5f)] float explosionTime = 2f;
	[SerializeField] float deathZoneY = -100f;
	[HideInInspector] public bool isAlive;
	WaitForSeconds beforeExplosionTimeWait;
	WaitForSeconds explosionTimeWait;

	[Header("Buttons")]
	//Button switch
	public bool isButton;
	public int buttonActiveTerrainIndex;
	public ButtonManager buttonManager;

	[HideInInspector]
	public int blockID;
	[HideInInspector]
	public int loadedTerrainID = 0;

	private void Start()
	{
		beforeExplosionTimeWait = new WaitForSeconds(timeBeforeExplosion);
		explosionTimeWait = new WaitForSeconds(explosionTime);
	}

	private void FixedUpdate()
	{
		double beforeTimeNetwork = NetworkTime.time;
		if (movingToTargetPos)
		{
			MoveToTargetPos();
		}
	}

	public void SetNextTerrainPosition()
	{
		isAlive = true;
		boxCollider.enabled = true;
		startPosition = transform.position;
		loadedTerrainID++;
		if(loadedTerrainID >= ThemeManager.Instance.terrains.Count)
			loadedTerrainID = 0;
		targetPosition = ThemeManager.Instance.terrains[loadedTerrainID].positions[blockID];
		movingToTargetPos = true;
		elapsedTime = 0f;
	}

	void MoveToTargetPos()
	{
		elapsedTime += Time.fixedDeltaTime; //Need to change
		completion = elapsedTime / moveDuration;
		Vector3 previousPos = transform.position;
		transform.position = Vector3.Lerp(startPosition, targetPosition, Mathf.SmoothStep(0, 1, completion));

		//Calculate velocity of each block (give this velo to player)
		ownVelo = ((transform.position - previousPos));
		//ownVelo = -Vector3.Lerp(ownVelo, currentFrameVel, 0.1f);
		//ownVelo = new Vector3(0, ownVelo.y, 0);

		if (completion >= 1)
		{
			transform.position = targetPosition;
			movingToTargetPos = false;
		}
	}

	public IEnumerator WaitAndExplode()
	{
		//Start crumble vfx

		yield return beforeExplosionTimeWait;

		StartCoroutine(ExplodeCoroutine());
	}

	public IEnumerator ExplodeCoroutine()
	{
		isAlive = false;
		boxCollider.enabled = false;

		//start explosion vfx


		yield return explosionTimeWait;
		transform.position = new Vector3(transform.position.x, deathZoneY, transform.position.z);
	}


}
