using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGravityParticle : MonoBehaviour
{
	public float gravityScale = 1.0f;

	void Start()
	{
		Vector3 gravity = -transform.position.normalized * Physics.gravity.magnitude * gravityScale;
		ParticleSystem ps = GetComponent<ParticleSystem>();
		var vel = ps.velocityOverLifetime;//new ParticleSystem.VelocityOverLifetimeModule();
		vel.space = ParticleSystemSimulationSpace.World;
		vel.speedModifier = 1.0f;
		vel.x = gravity.x;
		vel.y = gravity.y;
		vel.z = gravity.z;
	}
}
