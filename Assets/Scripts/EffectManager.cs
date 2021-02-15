using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
	public static EffectManager instance;
	public ParticleSystem damageParticle;

	void Awake()
	{
		instance = this;
	}

	public static void DamageEffect(Vector3 pos, Quaternion rot)
	{
		instance.damageParticle.transform.position = pos;
		instance.damageParticle.transform.rotation = rot;
		instance.damageParticle.Play();
	}
}
