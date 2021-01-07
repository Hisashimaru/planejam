using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuaternionTest : MonoBehaviour
{
	Vector3 forward = Vector3.forward;
	void Start()
	{
		//transform.localRotation =  Quaternion.Euler(0.0f, 0.0f, 45.0f) * Quaternion.Euler(90.0f, 0.0f, 0.0f) * Quaternion.Euler(0.0f, 90.0f, 0.0f);
	}

	void Update()
	{
		Vector3 nextpos = transform.position;

		if(Input.GetKey(KeyCode.W))
		{
			nextpos += transform.forward * 5.0f * Time.deltaTime;
		}

		float handling = 0.0f;
		if(Input.GetKey(KeyCode.D))
		{
			handling = 1.0f;
		}

		Vector3 normal = nextpos.normalized;
		transform.position = normal * 5.0f;
		//transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, normal), normal) * Quaternion.Euler(0.0f, 0.0f, 90.0f*handling);
		transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, normal), normal);
		transform.rotation *= Quaternion.Euler(0.0f, 0.0f, 90.0f*handling);//Quaternion.AngleAxis(1.0f*handling, transform.forward);
	}
}
