using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
	// Update is called once per frame
	void LateUpdate()
	{
		Vector3 normal = transform.position.normalized;
		transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, normal), normal);
	}
}
