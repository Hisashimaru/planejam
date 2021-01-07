using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEffect : MonoBehaviour
{
	ParticleSystem.EmissionModule[] em;
	
	public bool emmision
	{
		set
		{
			if(value) Play(); else Stop();
		}
	}

	void Start()
	{
		var particles = GetComponentsInChildren<ParticleSystem>();
		em = new ParticleSystem.EmissionModule[particles.Length];
		for(int i=0; i<particles.Length; i++)
		{
			em[i] = particles[i].emission;
		}
	}


	public void Play()
	{
		for(int i=0; i<em.Length; i++)
		{
			em[i].enabled = true;
		}
	}

	public void Stop()
	{
		for(int i=0; i<em.Length; i++)
		{
			em[i].enabled = false;
		}
	}
}
