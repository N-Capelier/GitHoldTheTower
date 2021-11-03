using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{
	Vector3 startPosition;
	Vector3 targetPosition;
	bool movingToTargetPos;

	[SerializeField] float moveDuration = 1.2f;
	float elapsedTime;
	float completion;

	public void SetTargetPosition(Vector3 _position)
	{
		startPosition = transform.position;
		targetPosition = _position;
		movingToTargetPos = true;
	}

	private void FixedUpdate()
	{
		if(movingToTargetPos)
		{
			MoveToTargetPos();
		}
	}

	void MoveToTargetPos()
	{
		elapsedTime += Time.fixedDeltaTime;
		completion = elapsedTime / moveDuration;

		transform.position = Vector3.Lerp(startPosition, targetPosition, Mathf.SmoothStep(0, 1, completion));
		if(completion >= 1)
		{
			transform.position = targetPosition;
			movingToTargetPos = false;
		}
	}
}
