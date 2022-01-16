using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPVAnimatorManager : MonoBehaviour
{
	[Header("References")]
	[SerializeField] Animator animator;
	[SerializeField] PlayerLogic logic;
	[SerializeField] PlayerMovement movement;

	int jumpStart, jumpInAir, jumpFall, punchLoad, punchRelease;

	bool isMoving, isLoadingPunch, isPunching, isInAir;

	private void Start()
	{
		jumpStart = Animator.StringToHash("AnimJumpStart");
		jumpInAir = Animator.StringToHash("AnimJumpInAir");
		jumpFall = Animator.StringToHash("AnimJumpFall");
		punchLoad = Animator.StringToHash("AnimPunchLoad");
		punchRelease = Animator.StringToHash("AnimPunchRelease");
	}

	private void Update()
	{
		if(isInAir && logic.isGrounded && !isLoadingPunch && !animator.GetBool("isPunching"))
		{
			isInAir = false;
			animator.SetBool("isInAir", false);
		}
		else if(!isInAir && !logic.isGrounded && !isLoadingPunch && !animator.GetBool("isPunching"))
		{
			isInAir = true;
			animator.SetBool("isInAir", true);
		}

		if(movement.selfRbd.velocity.x != 0f || movement.selfRbd.velocity.z != 0f)
		{
			if(isMoving == false)
			{
				isMoving = true;
				animator.SetBool("isMoving", true);
			}
		}
		else
		{
			if(isMoving == true)
			{
				isMoving = false;
				animator.SetBool("isMoving", false);
			}
		}

		if (!isLoadingPunch && movement.isChargingPunch)
		{
			isLoadingPunch = true;
			animator.SetBool("isLoadingPunch", true);
			animator.Play(punchLoad);
		}
		else if(isLoadingPunch && !movement.isChargingPunch)
		{
			isLoadingPunch = false;
			animator.SetBool("isLoadingPunch", false);

			animator.Play(punchRelease);
		}
	}

	public void AnimateJump()
	{
		if(!isLoadingPunch)
		{
			animator.SetBool("isJumping", true);
			animator.SetBool("isInAir", true);
			animator.Play(jumpStart);
			animator.SetBool("isPunching", true);
			isInAir = true;
		}
	}
}
