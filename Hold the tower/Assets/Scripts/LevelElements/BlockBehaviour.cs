using System.Collections;
using UnityEngine;
using Mirror;

public class BlockBehaviour : MonoBehaviour
{
	[Header("Components")]
	[SerializeField] public BoxCollider boxCollider;
	[SerializeField] MeshRenderer meshRenderer;
	Material blockMaterial;

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
	public bool isDestroyable = false;
	[SerializeField] [Range(0f, 5f)] float timeBeforeExplosion = 2f;
	[SerializeField] [Range(0f, 5f)] float explosionTime = 2f;
	[SerializeField] float deathZoneY = -100f;
	[HideInInspector] public bool isAlive;
	WaitForSeconds beforeExplosionTimeWait;
	WaitForSeconds explosionTimeWait;
	WaitForEndOfFrame waitForEndOfFrame;
	[HideInInspector] public bool isExploding = false;

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
		waitForEndOfFrame = new WaitForEndOfFrame();
		if(isDestroyable)
		{
			blockMaterial = meshRenderer.material;
		}
		isAlive = true;
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
		if(isDestroyable && !isAlive)
		{
			isAlive = true;
			blockMaterial.SetFloat("DissolveValue", 0);
			blockMaterial.SetFloat("PreDissolveAlphaValue", 0);
			gameObject.layer = LayerMask.NameToLayer("Outlined");
		}
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

		float _elapsedTime = 0f;
		float _completion = 0f;

		while(_elapsedTime < timeBeforeExplosion)
		{
			_elapsedTime += Time.deltaTime;
			_completion = _elapsedTime / timeBeforeExplosion;
			blockMaterial.SetFloat("PreDissolveAlphaValue", Mathf.Lerp(0, 1, _completion));
			yield return waitForEndOfFrame;
		}

		StartCoroutine(ExplodeCoroutine());
	}

	public IEnumerator ExplodeCoroutine()
	{
		gameObject.layer = LayerMask.NameToLayer("Default");
		boxCollider.enabled = false;

		SoundManager.Instance.PlaySoundEvent("LevelBlockDestroyed");

		float _elapsedTime = 0f;
		float _completion = 0f;
		while(_elapsedTime < explosionTime)
		{
			_elapsedTime += Time.deltaTime;
			_completion = _elapsedTime / explosionTime;
			blockMaterial.SetFloat("DissolveValue", Mathf.Lerp(0, 1, _completion));
			yield return waitForEndOfFrame;
		}

		transform.position = new Vector3(transform.position.x, deathZoneY, transform.position.z);
		isAlive = false;
		yield return waitForEndOfFrame;
	}

	public void StartButtonActivationEffect()

    {

		// JB !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! c'est � toi !

		GameObject effect = Instantiate(ThemeManager.Instance.buttonActivationEffectPrefab, transform);

		SoundManager.Instance.PlaySoundEvent("LevelButtonActivated");

		effect.transform.localScale = new Vector3(transform.localScale.x * 2 + 0.3f, transform.localScale.y * 2 + 2, transform.localScale.z * 2 + 0.3f);

	}
}
