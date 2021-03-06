using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FPVAnimatorManager : MonoBehaviour
{
	[Header("References")]
	[SerializeField] Animator animator;
	[SerializeField] ParticleSystem loadedParticles;
	ParticleSystem.MainModule psMainModule;
	[SerializeField] PlayerLogic logic;
	[SerializeField] PlayerMovement movement;
	[SerializeField] Transform loadedStartPos, loadedEndPos;
	Coroutine moveParticlesCoroutine;

	private Vector3 previousPos;
	private bool isMooving;

	private void Start()
	{
		psMainModule = loadedParticles.main;
		previousPos = transform.position;
	}

	private void Update()
	{
        //var animatorInfo = animator.GetCurrentAnimatorClipInfo(0);
        //Debug.LogWarning(animatorInfo[0].clip.name);
        if (transform.hasChanged)
        {
			if (!previousPos.x.Equals(transform.position.x) || previousPos.z.Equals(transform.position.z))
            {
				isMooving = true;
			}
			transform.hasChanged = false;
		}


#if UNITY_EDITOR
		if(Input.GetKeyDown(KeyCode.P))
		{
			EditorApplication.isPaused = true;
		}
#endif

		if(loadedParticles.gameObject.activeSelf)
		{
			if (animator.GetBool("isPunchLoaded"))
			{
				psMainModule.startLifetime = .8f;
			}
			else if(animator.GetBool("isLoadingPunch"))
			{
				loadedParticles.transform.position = Vector3.Lerp(loadedStartPos.position, loadedEndPos.position, logic.ratioAttack);
				psMainModule.startLifetime = logic.ratioAttack.Remap(0f, 1f, 0f, .8f);
			}
		}

		if(animator.GetBool("isLoadingPunch") && !loadedParticles.gameObject.activeSelf && !logic.hasFlag)
		{
			loadedParticles.gameObject.SetActive(true);
		}
		else if(((!animator.GetBool("isPunchLoaded") && !animator.GetBool("isLoadingPunch")) || logic.hasFlag) && loadedParticles.gameObject.activeSelf)
		{
			loadedParticles.gameObject.SetActive(false);
		}

		if (movement.selfRbd.velocity.x != 0f || movement.selfRbd.velocity.z != 0f)
		{
			animator.SetBool("isMoving", true);
		}
		else
		{
			animator.SetBool("isMoving", false);
		}

		if((animator.GetBool("isInAir") || animator.GetBool("isJumping")) && logic.isGrounded)
		{
			animator.SetBool("isJumping", false);
			animator.SetBool("isInAir", false);
			//animator.SetBool("isImpactingGround", true);
		}
		else if(!animator.GetBool("isInAir") && !animator.GetBool("isJumping") && (!logic.isGrounded && Mathf.Abs(movement.selfRbd.velocity.y) > 1f))
		{
			animator.SetBool("isInAir", true);
		}
	}

	IEnumerator MoveParticlesOnPunch()
	{
		float _completion = 0f;
		while(_completion < .4f)
		{

			loadedParticles.transform.position = Vector3.Lerp(loadedEndPos.position, loadedStartPos.position, _completion);
			//Debug.LogWarning(loadedParticles.transform.position);
			_completion += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
	}


	public void AnimateJump()
	{
		animator.SetBool("isJumping", true);
	}

	public void AnimateLoadPunch()
	{
		if(!animator.GetBool("isPunchLoaded"))
			animator.SetBool("isLoadingPunch", true);
	}

	public void AnimatePunch(bool _value)
	{
		animator.SetBool("isPunchLoaded", false);
		animator.SetBool("isPunching", true);

		if(moveParticlesCoroutine == null)
		{
			StartCoroutine(MoveParticlesOnPunch());
		}
	}

	public void StopAnimatePunch()
    {
		animator.SetBool("isPunchLoaded", false);
		animator.SetBool("isPunching", false);
	}
}
