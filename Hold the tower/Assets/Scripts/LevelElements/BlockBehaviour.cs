using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{
	[SerializeField] float moveSpeed = 0f;
	[SerializeField] float snapThreshold = .1f;

	Vector3 targetPosition;
	bool movingToTargetPos;

	public void SetTargetPosition(Vector3 _position)
	{
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
		Vector3 _direction = targetPosition - transform.position;
		_direction.Normalize();
		_direction *= moveSpeed;

		transform.Translate(_direction * Time.fixedDeltaTime);

		if((targetPosition - transform.position).magnitude <= snapThreshold)
		{
			transform.position = targetPosition;
			movingToTargetPos = false;
		}
	}
}
