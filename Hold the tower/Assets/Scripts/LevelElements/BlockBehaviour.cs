using System.Collections;
using UnityEngine;
using Mirror;

public class BlockBehaviour : MonoBehaviour
{
	[Header("Components")]
	[SerializeField] public BoxCollider boxCollider;
	[SerializeField] public MeshRenderer meshRenderer;
	[HideInInspector] public Material blockMaterial;
	[SerializeField] GameObject tile;
	Material tileMaterial;

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
	[Range(0f, 5f)] public float timeBeforeExplosion = 2f;
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

	[Header("Highlight")]
	public GameObject highlightDisplay;
    public ParticleSystem chunkActivableParticle;
	[HideInInspector] public LevelTransition levelTransition;
	[SerializeField] public Material blockWarnMaterial;
    [SerializeField] private ParticleSystem blockWarnDirectionParticle;

    [HideInInspector]
	public int blockID;
	[HideInInspector]
	public int loadedTerrainID = 0;
	[HideInInspector]
	public bool isInChunck;
	[HideInInspector]
	public bool isSelected;

    private void Start()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		blockMaterial = meshRenderer.material;
		if (tile != null)
		{
			if (tile.activeSelf)
				tile.SetActive(false);

			if (isDestroyable)
			{
				tile.SetActive(true);
				tileMaterial = tile.GetComponent<MeshRenderer>().material;
			}
		}

		beforeExplosionTimeWait = new WaitForSeconds(timeBeforeExplosion);
		explosionTimeWait = new WaitForSeconds(explosionTime);
		waitForEndOfFrame = new WaitForEndOfFrame();
		if(isDestroyable)
		{
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
		if (isDestroyable && !isAlive)
		{
				return;
			//isAlive = true;
			//blockMaterial.SetFloat("DissolveValue", 0);
			//blockMaterial.SetFloat("PreDissolveAlphaValue", 0);
			//gameObject.layer = LayerMask.NameToLayer("Outlined");
		}
		boxCollider.enabled = true;
		startPosition = transform.position;

		loadedTerrainID++;
		if (loadedTerrainID >= ThemeManager.Instance.terrains.Count)
			loadedTerrainID = 0;

		targetPosition = ThemeManager.Instance.terrains[loadedTerrainID].positions[blockID];
		movingToTargetPos = true;
		elapsedTime = 0f;
        UpdatePreviewDirection();
    }

    public Vector3 GetNextPosDirection()
    {
        Vector3 direction = Vector3.zero;
        int nextTerrainId = loadedTerrainID + 1;
        if (nextTerrainId >= ThemeManager.Instance.terrains.Count)
            nextTerrainId = 0;

        direction = ThemeManager.Instance.terrains[nextTerrainId].positions[blockID] - ThemeManager.Instance.terrains[loadedTerrainID].positions[blockID];
        direction.Normalize();

        return direction;
    }

	public void SetBlockAlive()
	{
		if(isDestroyable)
		{
			StopAllCoroutines();
			isExploding = false;
			isAlive = true;
			blockMaterial.SetFloat("DissolveValue", 0f);
			blockMaterial.SetFloat("PreDissolveAlphaValue", 0f);
			tileMaterial.SetFloat("AlphaValue", 1f);
			gameObject.layer = LayerMask.NameToLayer("Outlined");

			loadedTerrainID = ThemeManager.Instance.blocks[0].loadedTerrainID;

			loadedTerrainID++;
			if (loadedTerrainID >= ThemeManager.Instance.terrains.Count)
				loadedTerrainID = 0;

			SetNextTerrainPosition();
		}
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

		while(_elapsedTime < timeBeforeExplosion * .4f)
		{
			_elapsedTime += Time.deltaTime;
			_completion = _elapsedTime / (timeBeforeExplosion * .4f);
			blockMaterial.SetFloat("PreDissolveAlphaValue", Mathf.Lerp(0, 1, _completion));
			yield return waitForEndOfFrame;
		}

		yield return new WaitForSeconds(timeBeforeExplosion * .6f);

		StartCoroutine(ExplodeCoroutine());
	}

	public IEnumerator ExplodeCoroutine()
	{
		gameObject.layer = LayerMask.NameToLayer("Default");
		boxCollider.enabled = false;

		float _elapsedTime = 0f;
		float _completion = 0f;
		while(_elapsedTime < explosionTime)
		{
			_elapsedTime += Time.deltaTime;
			_completion = _elapsedTime / explosionTime;
			blockMaterial.SetFloat("DissolveValue", Mathf.Lerp(0, 1, _completion));
			tileMaterial.SetFloat("AlphaValue", Mathf.Lerp(1, 0, _completion));
			yield return waitForEndOfFrame;
		}

		transform.position = new Vector3(transform.position.x, deathZoneY, transform.position.z);
		isAlive = false;
		yield return waitForEndOfFrame;
	}

	public void StartButtonActivationEffect()
    {
		GameObject effect = Instantiate(ThemeManager.Instance.buttonActivationEffectPrefab, transform);

		SoundManager.Instance.PlaySoundEvent("LevelButtonActivated");

		effect.transform.localScale = new Vector3(transform.localScale.x * 2 + 0.3f, transform.localScale.y * 2 + 2, transform.localScale.z * 2 + 0.3f);
	}

	public IEnumerator WaitAndPlayDissolveSound()
	{
		yield return new WaitForSeconds(timeBeforeExplosion);
		SoundManager.Instance.PlaySoundEvent("LevelBlockDestroyed", gameObject);
    }

    Vector3 nextPosDirection;
    private void UpdatePreviewDirection()
    {
        nextPosDirection = GetNextPosDirection();
        if (nextPosDirection.y > 0)
        {
            blockWarnDirectionParticle.transform.localPosition = Vector3.up;
            blockWarnDirectionParticle.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        }
        else if (nextPosDirection.y < 0)
        {
            blockWarnDirectionParticle.transform.position = transform.position + Vector3.up * (transform.localScale.y + 0.8f);
            blockWarnDirectionParticle.transform.localRotation = Quaternion.Euler(90, 0, 0);
        }
    }

    public void StartMovementPreview()
    {
        if(!blockWarnDirectionParticle.isPlaying)
        {
            UpdatePreviewDirection();
            blockWarnDirectionParticle.Play();
        }
    }

    public void StopMovementPreview()
    {
        blockWarnDirectionParticle.Stop();
    }
}
