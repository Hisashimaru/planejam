using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cinemachine;

public class MegaShark : MonoBehaviour
{
	public float radius = 10f;

	//shark
	public Transform sharkShadow;
	public Transform shark;
	public Transform mouth;
	public ParticleSystem outSplashParticle;
	public ParticleSystem inSplashParticle;
	public AudioSource outSplashSound;
	public AudioSource inSplashSound;
	Vector3 nextPos;
	Vector3 velocity;
	AirplanePlayer player;
	State state;
	bool bitPlane;
	CinemachineImpulseSource impulseSource;
	float triggerAnngle = 3.0f;
	float planetSize;

	enum State
	{
		Swim,
		Jump,
		Fall,
	}

	void Start()
	{
		planetSize = GameManager.instance.planet.size;
		transform.position = transform.position.normalized * (planetSize - 0.2f);
		transform.rotation = Quaternion.FromToRotation(Vector3.up, transform.position.normalized);
		nextPos = GetNextPosition();
		player = GameManager.instance.player;
		shark.gameObject.SetActive(false);
		impulseSource = GetComponent<CinemachineImpulseSource>();
		triggerAnngle = triggerAnngle = Random.Range(3.0f, 8.0f);
	}

	void Update()
	{
		switch(state)
		{
			case State.Swim:
				Swim();
				break;
			case State.Jump:
				Jump();
				break;
		}
	}

	void Swim()
	{
		Vector3 dir = nextPos - sharkShadow.position;
		if(dir.magnitude <= 0.5f)
		{
			nextPos = GetNextPosition();
		}
		else
		{
			// Move
			velocity += dir.normalized * Time.deltaTime;
			velocity = velocity.magnitude > 3.0f ? velocity.normalized * 3.0f : velocity;
			Vector3 pos = sharkShadow.position + velocity * Time.deltaTime;
			sharkShadow.position = pos.normalized * (planetSize - 0.2f);
			sharkShadow.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(velocity, pos.normalized), pos.normalized);
			velocity = velocity - (velocity * 0.5f * Time.deltaTime);
		}

		if(player)
		{
			float angle = Vector3.Angle(player.transform.position.normalized, sharkShadow.position.normalized);
			if(angle < triggerAnngle)
			{
				state = State.Jump;
				sharkShadow.gameObject.SetActive(false);

				Vector3 sharkpos =sharkShadow.position;
				shark.gameObject.SetActive(true);
				shark.position = sharkpos;

				// shark rotation
				Vector3 right = Vector3.Cross(player.transform.position.normalized, player.transform.forward);
				right = Random.Range(0, 2) == 0 ? right : -right;
				shark.rotation = Quaternion.LookRotation(player.transform.position.normalized, right);
				velocity = sharkpos.normalized * 10.0f;
				impulseSource.GenerateImpulse();

				outSplashParticle.transform.position = sharkpos;
				outSplashParticle.Play();
				outSplashSound.Play();

				StartCoroutine(EmitInSplashParticle());
				triggerAnngle = Random.Range(3.0f, 8.0f);
			}
		}
	}

	IEnumerator EmitInSplashParticle()
	{
		inSplashParticle.transform.position = sharkShadow.position;
		yield return new WaitForSeconds(2);
		inSplashParticle.Play();
		inSplashSound.Play();
	}

	void Jump()
	{
		shark.position += velocity * Time.deltaTime;
		// gravity
		velocity = velocity - shark.position.normalized * 8.0f * Time.deltaTime;

		// Collision
		if(player && !bitPlane && Vector3.Distance(player.transform.position, mouth.position) < 2.0f)
		{
			bitPlane = true;
			player.enabled = false;
			player.transform.position = mouth.position;
			//player.transform.rotation = Quaternion.LookRotation(mouth.forward, mouth.up);
			player.transform.parent = shark;
			impulseSource.GenerateImpulse(2.0f);
			player.TakeDamage(DamageType.Bite);
		}

		// Land on the water
		if(shark.position.magnitude < 75.0f)
		{
			if(bitPlane)
			{
				player.Explosion();
			}

			state = State.Swim;
			shark.gameObject.SetActive(false);
			sharkShadow.gameObject.SetActive(true);
			velocity = transform.forward * 0.0001f;
		}
	}

	Vector3 GetNextPosition()
	{
		Vector2 v2 = Random.insideUnitCircle * radius;
		Vector3 pos = new Vector3(v2.x, 0.0f, v2.y);
		transform.TransformPoint(pos);
		return (transform.position + pos).normalized * 80f;
	}

	#if UNITY_EDITOR
	void OnDrawGizmosSelected()
	{

		Handles.DrawWireDisc(transform.position, transform.up, radius);
	}
	#endif
}