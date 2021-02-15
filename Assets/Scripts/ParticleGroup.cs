using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleGroup : MonoBehaviour
{
	ParticleSystem[] particles;

	void Awake()
	{
		particles = GetComponentsInChildren<ParticleSystem>();
	}

	public void Play()
	{
		foreach(ParticleSystem ps in particles)
		{
			ps.Play();
		}
	}

	public void Stop()
	{
		foreach(ParticleSystem ps in particles)
		{
			ps.Stop();
		}
	}
}
