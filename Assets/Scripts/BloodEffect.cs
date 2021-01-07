using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffect : MonoBehaviour
{
	public static BloodEffect instance;
	public float gravityScale = 1.0f;
	public ParticleSystem bloodParticle;
	public ParticleSystem bloodDecal;
	ParticleSystem.Particle[] particle;
	ParticleSystem.Particle[] decals;

	void Awake()
	{
		instance = this;
		particle = new ParticleSystem.Particle[bloodParticle.main.maxParticles];
		decals = new ParticleSystem.Particle[bloodDecal.main.maxParticles];
	}

	void LateUpdate()
	{
		float planetSize = GameManager.instance.planet.size;
		float planetSize2 = planetSize * planetSize;

		int num = bloodParticle.GetParticles(particle);
		for(int i=0; i<num; i++)
		{
			particle[i].velocity += particle[i].position.normalized * -9.8f * gravityScale * Time.deltaTime;

			// a particle hit the ground
			if(particle[i].position.sqrMagnitude < planetSize2)
			{
				particle[i].remainingLifetime = 0.0f;
				//particle[i].velocity = new Vector3();
				PaintDecal(particle[i].position.normalized * planetSize);
			}
		}
		bloodParticle.SetParticles(particle, num);
	}

	public void Emit(Vector3 pos)
	{
		Vector3 normal = pos.normalized;
		ParticleSystem.EmitParams p = new ParticleSystem.EmitParams();
		p.position = pos;

		for(int i=0; i<10; i++)
		{
			p.startSize = EvaluateRandom(bloodParticle.main.startSize);
			Quaternion rot = Quaternion.FromToRotation(Vector3.up, normal) * Quaternion.Euler(0f, Random.Range(0f, 360f), 0f) * Quaternion.Euler(Random.Range(0f, 25f), 0f, 0f);
			p.velocity = rot * Vector3.up * EvaluateRandom(bloodParticle.main.startSpeed);
			bloodParticle.Emit(p, 1);
		}
	}

	void PaintDecal(Vector3 pos)
	{
		Vector3 normal = pos.normalized;
		ParticleSystem.EmitParams p = new ParticleSystem.EmitParams();
		p.position = pos + normal * 0.1f;
		Vector3 rot = Quaternion.LookRotation(-normal).eulerAngles;
		rot.z = Random.Range(0f, 360f);
		p.rotation3D = rot;
		p.startSize = EvaluateRandom(bloodDecal.main.startSize);
		bloodDecal.Emit(p, 1);
	}

	float EvaluateRandom(ParticleSystem.MinMaxCurve curve)
	{
		if(curve.mode == ParticleSystemCurveMode.Constant)
		{
			return curve.constant;
		}
		else if(curve.mode == ParticleSystemCurveMode.TwoConstants)
		{
			return Random.Range(curve.constantMin, curve.constantMax);
		}
		return curve.Evaluate(Random.value);
	}
}
