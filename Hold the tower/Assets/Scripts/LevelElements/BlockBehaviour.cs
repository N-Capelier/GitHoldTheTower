using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{
	Vector3 startPosition;
	Vector3 targetPosition;
	bool movingToTargetPos;

	[SerializeField] float moveDuration = 1.2f;
	float elapsedTime = 0f;
	float completion;

	[HideInInspector]
	public Vector3 speedPerframe;

	public void SetTargetPosition(Vector3 _position)
	{
		startPosition = transform.position;
		targetPosition = _position;
		movingToTargetPos = true;
		elapsedTime = 0f;
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

		Vector3 beforeMovement = transform.position;
		transform.position = Vector3.Lerp(startPosition, targetPosition, Mathf.SmoothStep(0, 1, completion));
		if(completion >= 1)
		{
			transform.position = targetPosition;
			movingToTargetPos = false;
		}
		speedPerframe = transform.position - beforeMovement;
	}
}
