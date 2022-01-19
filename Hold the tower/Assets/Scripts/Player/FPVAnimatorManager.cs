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

	private void Start()
	{
		psMainModule = loadedParticles.main;
	}

	private void Update()
	{
		//var animatorInfo = animator.GetCurrentAnimatorClipInfo(0);
		//Debug.LogWarning(animatorInfo[0].clip.name);

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
			else
			{
				loadedParticles.transform.position = Vector3.Lerp(loadedStartPos.position, loadedEndPos.position, logic.ratioAttack);
				psMainModule.startLifetime = logic.ratioAttack.Remap(0f, 1f, 0f, .8f);
			}
		}

		if((animator.GetBool("isLoadingPunch") && !loadedParticles.gameObject.activeSelf))
		{
			loadedParticles.gameObject.SetActive(true);
		}
		else if((!animator.GetBool("isPunchLoaded") && !animator.GetBool("isPunching") && !animator.GetBool("isLoadingPunch")) && loadedParticles.gameObject.activeSelf)
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
	}
}
