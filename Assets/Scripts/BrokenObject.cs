using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenObject : MonoBehaviour
{
	public float gravityScale = 1.0f;
	public Vector3 center;
	//public Vector3 size = new Vector3(1f, 1f, 1f);
	[System.NonSerialized]
	public Vector3 velocity;
	[System.NonSerialized]
	public Vector3 angularVelocity;
	bool grounded;

	void Update()
	{
		if(grounded)
			return;

		Vector3 gravity = -transform.position.normalized * gravityScale;
		velocity += gravity * Time.deltaTime;
		transform.position += velocity * Time.deltaTime;
		velocity = velocity - (velocity * 0.4f * Time.deltaTime);

		Vector3 pos = transform.position + (transform.rotation*center);
		transform.RotateAround(pos, angularVelocity, Mathf.Rad2Deg*angularVelocity.magnitude * Time.deltaTime);

		// Check collision
		const int layerMask = 1 << 8; // ground
		Collider[] colliders = Physics.OverlapSphere(pos, 0.5f, layerMask);
		if(colliders.Length > 0)
		{
			grounded = true;
		}
	}


	void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0, 1, 0, 1.0f);
		//Gizmos.DrawWireCube(transform.position + (transform.rotation*center), size);
		Gizmos.DrawRay(transform.position + (transform.rotation*center) + new Vector3(0f, -0.5f, 0f), Vector3.up);
		Gizmos.DrawRay(transform.position + (transform.rotation*center) + new Vector3(-0.5f, 0f, 0f), Vector3.right);
		Gizmos.DrawRay(transform.position + (transform.rotation*center) + new Vector3(0f, 0f, -0.5f), Vector3.forward);
	}
}
