using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BlockBehaviour : MonoBehaviour
{
	Vector3 startPosition;
	Vector3 targetPosition;
	bool movingToTargetPos;

	[SerializeField] float moveDuration = 1.2f;
	float elapsedTime = 0f;
	float completion;

	public Vector3 ownVelo = Vector3.zero;

	public void SetTargetPosition(Vector3 _position)
	{
		startPosition = transform.position;
		targetPosition = _position;
		movingToTargetPos = true;
		elapsedTime = 0f;
	}

	private void FixedUpdate()
	{
		double beforeTimeNetwork = NetworkTime.time;
		if (movingToTargetPos)
		{
			MoveToTargetPos();
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
}
