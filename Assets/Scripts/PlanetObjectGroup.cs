using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetObjectGroup : MonoBehaviour
{
	public AudioSource breakSound;
	float breakSoundTime;
	AirplanePlayer player;
	BreakableObject[] objects;
	float radius = 0.0f;

	void Awake()
	{
		objects = transform.GetComponentsInChildren<BreakableObject>();
		foreach(var obj in objects)
		{
			float dist = obj.transform.localPosition.magnitude;
			if(dist > radius)
			{
				radius = dist;
			}
		}
		radius += 5.0f;
	}

	void Start()
	{
		player = GameManager.instance.player;
	}

	void Update()
	{
		if(!player)
			return;

		bool hited = false;
		if(Vector3.Distance(player.transform.position, transform.position) <= radius)
		{
			foreach(var obj in objects)
			{
				if(!obj.isBroken && (player.transform.position - obj.transform.position).sqrMagnitude <= 2.5f*2.5f)
				{
					if(breakSoundTime <= Time.time)
					{
						breakSound.PlayOneShot(breakSound.clip);
						breakSoundTime = Time.time + 0.2f;
						hited = true;
					}
					obj.Break(player.transform.position, player.transform.forward * player.speed);
					player.TakeDamage(DamageType.SmallObject);
				}
			}
		}

		if(hited)
		{
			NS.Gamepad.AddImpulse(0.0f, 0.2f, 0.1f);
		}
	}
}
