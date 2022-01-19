using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPVAnimatorManager : MonoBehaviour
{
	[Header("References")]
	[SerializeField] Animator animator;
	[SerializeField] GameObject loadedParticles;
	[SerializeField] PlayerLogic logic;
	[SerializeField] PlayerMovement movement;

	private void Update()
	{
		//var animatorInfo = animator.GetCurrentAnimatorClipInfo(0);
		//Debug.LogWarning(animatorInfo[0].clip.name);

		if((animator.GetBool("isPunchLoaded") && !loadedParticles.activeSelf))
		{
			loadedParticles.SetActive(true);
		}
		else if(!animator.GetBool("isPunchLoaded") && loadedParticles.activeSelf)
		{
			loadedParticles.SetActive(false);
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
