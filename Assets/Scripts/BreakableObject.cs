using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
	public Transform defaultObject;
	public Transform afterObject;
	public BrokenObject[] brokenObjects;
	public bool isBroken{get; set;}

	void Awake()
	{
		foreach(BrokenObject obj in brokenObjects)
		{
			obj.gameObject.SetActive(false);
		}
	}

	public void Break(Vector3 position, Vector3 velocity)
	{
		if(isBroken)
			return;

		float power = Mathf.Clamp01(velocity.magnitude / 15.0f);
		defaultObject.gameObject.SetActive(false);
		if(afterObject)
			afterObject.gameObject.SetActive(true);
		foreach(BrokenObject obj in brokenObjects)
		{
			obj.gameObject.SetActive(true);

			Vector3 dir = Quaternion.FromToRotation(Vector3.up, transform.position.normalized) * Quaternion.Euler(Random.Range(0f, 20f), Random.Range(0f, 360f), 0f) * Vector3.up;
			obj.velocity = dir * (Mathf.Lerp(0.5f, 5.0f, power) + Random.Range(0.0f, 2.0f));

			Vector3 axis = Vector3.Cross(velocity.normalized, transform.position.normalized);
			axis = Quaternion.AngleAxis(Random.Range(-30f, 30f), transform.position.normalized) * axis;
			obj.angularVelocity = axis * (Mathf.Lerp(0.1f, 5.0f, power) + Random.Range(0.0f, 2.0f));
		}
		isBroken = true;
	}
}
