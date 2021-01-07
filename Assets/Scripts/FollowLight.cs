using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class FollowLight : MonoBehaviour
{
	public Transform followTarget;
	Vector3 relativePosition;
	Quaternion relativeRotation;

	void Awake()
	{
		if(followTarget)
		{
			relativePosition = transform.position - followTarget.position;
			relativeRotation = transform.rotation * Quaternion.Inverse(followTarget.rotation);
		}
	}

	void LateUpdate()
	{
		if(followTarget)
		{
			transform.position = followTarget.position + relativePosition;
			transform.rotation = followTarget.rotation * relativeRotation;
		}
	}
}
