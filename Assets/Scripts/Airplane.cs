using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public enum DamageType
{
	Body,
	LeftWing,
	RightWing,
	SmallObject,
	LargeObject,
	Bite,
}

public class Airplane : MonoBehaviour
{
	enum State
	{
		Flight,
		Fall,
	}
	[System.NonSerialized] State state;

	public bool isPlayer;
	public float maxSpeed = 10.0f;
	[System.NonSerialized]
	public float speed = 0f;
	public float acceleration = 5.0f;
	public float takeoffSpeed = 10.0f;
	public float turnSpeed = 100.0f;
	public float landingSpeed = 10.0f;
	public float breakPower = 10.0f;
	[System.NonSerialized]
	public float flyingHeight;
	[System.NonSerialized]
	public float groundHeight;
	protected float upVelocity;
	public GameObject explosionPrefab;
	protected bool onGround;
	protected const float tireSize = 1.0f;
	public float invincibleTime{get; protected set;}
	public bool isDead{get; protected set;}
	protected CinemachineImpulseSource impulseSource;

	[System.NonSerialized]
	public float health;
	public float maxHealth = 100.0f;
	public float healthNormal{get{return health/maxHealth;}}
	float explosionTime = float.MaxValue;

	public GameObject leftWing;
	public GameObject rightWing;
	public ParticleSystem leftWingSmoke;
	public ParticleSystem rightWingSmoke;
	public GameObject brokenLeftWingPrefab;
	public GameObject brokenRightWingPrefab;
	public AudioClip collisionSound;

	protected AirplanePlayer player;

	// AI
	float handling;

	protected virtual void Start()
	{
		flyingHeight = GameManager.instance.flyingHeight;
		groundHeight = GameManager.instance.groundHeight;
		health = maxHealth;

		if(!isPlayer)
		{
			speed = maxSpeed;
		}

		impulseSource = GetComponent<CinemachineImpulseSource>();
		player = GameManager.instance.player;
		invincibleTime = 0.0f;
		leftWingSmoke.gameObject.SetActive(true);
		rightWingSmoke.gameObject.SetActive(true);
		leftWingSmoke.Stop();
		rightWingSmoke.Stop();
	}
	
	void Update()
	{
		if(state == State.Flight)
		{
			float targetHandling = 0.0f;
			float nearest = float.MaxValue;
			// avoid plaens
			foreach(Airplane p in AirplaneManager.instance.airplanes)
			{
				if(p == this){continue;}
				Vector3 p1 = transform.position;
				Vector3 p2 = p.transform.position;
				float dist = Vector3.Distance(p1, p2);
				if(dist >= 40.0f || dist > nearest){continue;}
				//if(Vector3.Dot(transform.forward, p.transform.forward) >= 0.8f){continue;}		// check direction
				if(Vector3.Angle(transform.forward, (p2-p1).normalized) >= 20.0f){continue;}	// check angle

				float angle = Vector3.SignedAngle(transform.forward, p.transform.forward, p1.normalized);
				targetHandling = angle > 0.0f ? -1.0f : 1.0f;
				nearest = dist;
				if(targetHandling > 0.0f)
				{
					Debug.DrawLine(p1, p2, Color.blue, 0.5f);
				}
				else
				{
					Debug.DrawLine(p1, p2, Color.red, 0.5f);
				}
			}
			if(targetHandling-handling != 0.0f)
			{
				handling = Mathf.Lerp(handling, targetHandling, (1.0f/Mathf.Abs(targetHandling-handling))*Time.deltaTime);
			}

			// Rise
			if(speed >= takeoffSpeed)
			{
				float v = flyingHeight - transform.position.magnitude;
				upVelocity += v * Time.deltaTime;
				upVelocity -= upVelocity * Time.deltaTime;
			}
			speed += acceleration * Time.deltaTime;
			speed = Mathf.Clamp(speed, 0.0f, maxSpeed);
		}
		else
		{
			// Falling
			upVelocity -= 2.5f * Time.deltaTime;
		}

		// Move
		Vector3 nextPos = transform.localPosition;
		nextPos += (transform.position.normalized * upVelocity) * Time.deltaTime;
		nextPos += (transform.forward * speed) * Time.deltaTime;

		// Airplane position
		transform.localPosition = nextPos;

		// Airpalne rotation
		Vector3 normal = transform.position.normalized;
		transform.localRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, normal), normal) * Quaternion.Euler(0.0f, 90.0f*handling*Time.deltaTime, 0.0f) * Quaternion.Euler(0.0f, 0.0f, 45.0f*handling);

		// Hit to ground
		if(transform.position.magnitude <= groundHeight)
		{
			Explosion();
			return;
		}


		// Respawn
		if(player != null && Vector3.Angle(transform.position, player.transform.position) >= 62.0f)
		{
			Vector3 pos = Quaternion.FromToRotation(Vector3.up,  player.transform.position.normalized) * Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f) * Quaternion.Euler(60.0f, 0.0f, 0.0f) * new Vector3(0.0f, flyingHeight, 0.0f);
			transform.position = pos;

			// Set Airplane direction
			Vector3 upvec = transform.position.normalized;
			transform.rotation = Quaternion.LookRotation(player.transform.position - pos, upvec) * Quaternion.AngleAxis(Random.Range(-20.0f, 20.0f), upvec);
		}
	}

	public virtual void Explosion()
	{
		if(isDead)
			return;

		// Damage effect
		Quaternion rot = Quaternion.LookRotation(transform.position.normalized, transform.forward);
		EffectManager.DamageEffect(transform.position, rot);

		if(explosionPrefab)
		{
			rot = Quaternion.FromToRotation(Vector3.up, transform.position.normalized);
			Instantiate(explosionPrefab, transform.position, rot);
			
		}

		for(int i=0; i<30; i++)
		{
			Vector3 vel = Random.onUnitSphere;
			// if(Vector3.Dot(transform.position.normalized, vel) < 0.0f){vel = -vel;}	// make hemisphere velocity
			vel = Quaternion.FromToRotation(Vector3.up, transform.position.normalized) * Quaternion.Euler(0f, Random.Range(0.0f, 360.0f), 0f) * Quaternion.Euler(Random.Range(0.0f, 45.0f), 0f, 0f) * Vector3.up; // cone velocity
			PassengerManager.instance.SpawnDrop(transform.position + Random.insideUnitSphere, vel*10f);
		}

		if(!isPlayer)
		{
			AirplaneManager.instance.airplanes.Remove(this);
			// Calc distance
			if(player != null)
			{
				float dist = Vector3.Distance(player.transform.position, transform.position);
				float power = Mathf.Lerp(0.0f, 0.3f, (1f - (30f/dist)));
				NS.Gamepad.AddImpulse(power, power, 0.3f);
			}
		}
		else
		{
			NS.Gamepad.AddImpulse(0.5f, 0.5f, 0.4f);
		}
		Destroy(gameObject);
		impulseSource.GenerateImpulse();
	}

	protected virtual void OnCollisionEnter(Collision collision)
	{
		if(invincibleTime > Time.time)	// No damage
			return;

		// Ignore ground collision
		if((1<<collision.transform.gameObject.layer & LayerMask.GetMask("Ground")) != 0)
			return;

		// Falling
		if(state == State.Fall)
		{
			Explosion();
			return;
		}

		GameObject go = collision.contacts[0].thisCollider.gameObject;
		if(go == rightWing)
		{
			TakeDamage(DamageType.RightWing);
		}
		else if(go == leftWing)
		{
			TakeDamage(DamageType.LeftWing);
		}
		else
		{
			TakeDamage(DamageType.Body); // Body damage
		}
	}


	public virtual void TakeDamage(DamageType type)
	{
		if(invincibleTime > Time.time)	// No damage
			return;

		bool detachWingR = false;
		bool detachWingL = false;
		float lastHealth = health;

		// Damage effect
		Quaternion rot = Quaternion.LookRotation(transform.position.normalized, transform.forward);
		EffectManager.DamageEffect(transform.position, rot);

		if(type == DamageType.Body)	// Body damage
		{
			detachWingR = Random.Range(0, 3) == 0 ? true : false;
			detachWingL = Random.Range(0, 3) == 0 ? true : false;
			health -= 50f;
			if(isPlayer){NS.Gamepad.AddImpulse(0.5f, 0.0f, 0.3f);}
		}
		else if(type == DamageType.LeftWing)
		{
			detachWingL = true;
			health -= 30f;
			if(isPlayer){NS.Gamepad.AddImpulse(0.3f, 0.0f, 0.3f);}
		}
		else if(type == DamageType.RightWing)
		{
			detachWingR = true;
			health -= 30f;
			if(isPlayer){NS.Gamepad.AddImpulse(0.3f, 0.0f, 0.3f);}
		}
		else if(type == DamageType.SmallObject)
		{
			detachWingR = Random.Range(0, 5) == 0 ? true : false;
			detachWingL = Random.Range(0, 5) == 0 ? true : false;
			health -= 20f;
			if(isPlayer){NS.Gamepad.AddImpulse(0.3f, 0.0f, 0.3f);}
		}

		// detach wings
		if (detachWingR && rightWing.activeInHierarchy)	// Right wing
		{
			rightWing.SetActive(false);
			// Detach a wing
			GameObject wingObj = Instantiate(brokenRightWingPrefab, transform.position, transform.rotation);
			BrokenObject bobj = wingObj.GetComponent<BrokenObject>();
			bobj.velocity = transform.forward * speed;
			bobj.angularVelocity = new Vector3(0.0f, 1.0f, 0.0f);
			rightWingSmoke.Play();
			Destroy(wingObj, 60f);
		}
		if(detachWingL && leftWing.activeInHierarchy)	// Left wing
		{
			leftWing.SetActive(false);
			// Detach a wing
			GameObject wingObj = Instantiate(brokenLeftWingPrefab, transform.position, transform.rotation);
			BrokenObject bobj = wingObj.GetComponent<BrokenObject>();
			bobj.velocity = transform.forward * speed;
			bobj.angularVelocity = new Vector3(0.0f, 1.0f, 0.0f);
			leftWingSmoke.Play();
			Destroy(wingObj, 60f);
		}

		if(health <= 0f)
		{
			if(explosionTime == float.MaxValue)
			{
				state = State.Fall;
			}
			//Explosion();
		}
		else if(health < lastHealth)
		{
			SoundManager.instance.Play(collisionSound, transform.position, 0.5f);
			if(isPlayer)
			{
				impulseSource.GenerateImpulse();
			}
		}
		invincibleTime = Time.time + 1.0f;
	}
}
