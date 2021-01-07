using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class AirplanePlayer : Airplane
{
	[Header("Player")]
	[SerializeField]
	AudioSource engineSound = null;
	[SerializeField]
	TrailRenderer wingTrailRight = null;
	[SerializeField]
	TrailRenderer wingTrailLeft = null;
	protected Quaternion rollRotation = Quaternion.identity;
	bool engineStarted;
	float inputAxisX = 0.0f;

	// Airport
	[System.NonSerialized]
	public Airport currentAirport;
	protected  Airport lastAirport;
	public bool onAirport{get{return (currentAirport != null);}}

	// Gas
	public float gas{get; protected set;}
	public float maxGas = 100.0f;
	public float gasNormal{get{return gas/maxGas;}}
	float gasExposionTime = float.MaxValue;

	// Boost
	public float maxBoostSpeed = 30f;
	[System.NonSerialized]public bool enableBoost = false;	// Equiped
	[System.NonSerialized]public float boost = 0.0f;
	[System.NonSerialized]public bool boosting = false;

	// Shield
	[System.NonSerialized]public bool enableShield = false;

	// Passengers
	public int capacity = 30;	// max seat
	[System.NonSerialized]
	public List<int> passengers = new List<int>();
	float nextDisembarkTime = 0;
	public int passengerCount
	{
		get
		{
			int cnt = 0;
			foreach(int n in passengers)
			{
				cnt += n;
			}
			return cnt;
		}
	}
	public int freeSeat{get{return capacity - passengerCount;}}

	[Header("Effects")]
	public Transform boostEffectL;
	public Transform boostEffectR;
	public GroundEffect groundEffect;
	public ParticleSystem smokeEffect;
	public ParticleSystem moneyEffect;
	public ParticleSystem breakShieldEffect;
	public AudioClip scoreSound;
	public AudioClip boardSound;
	public AudioClip tireSound;
	public AudioClip breakShieldSound;
	public AudioSource boostSound;
	float nextPlayBoardSoundTime;
	Tweener scaleTweener;
	Material material;
	

	// Input
	InputAction moveAction, breakAction, boostAction;

	public bool isInAirport
	{
		get{return (currentAirport != null && onGround);}
	}

	void Awake()
	{
		if(isPlayer)
			GameManager.instance.player = this;
	}

	protected override void Start()
	{
		base.Start();

		moveAction = GetComponent<PlayerInput>().currentActionMap["Move"];
		breakAction = GetComponent<PlayerInput>().currentActionMap["Break"];
		boostAction = GetComponent<PlayerInput>().currentActionMap["Boost"];
		// boostAction.started += x => boosting = true;
		// boostAction.canceled += x => boosting = false;

		transform.position = new Vector3(0.0f, GameManager.instance.groundHeight + 1.0f, -3.0f);

		// Initialize passengers
		int airportcnt = GameManager.instance.airportManager.airportList.Count;
		int availablecnt = GameManager.instance.availableAirportCount;
		passengers.AddRange(new int[airportcnt]);
		gas = maxGas;
		onGround = true;

		// Set custom material
		material = GetComponent<Renderer>().material;
		leftWing.GetComponent<Renderer>().sharedMaterial = material;
		rightWing.GetComponent<Renderer>().sharedMaterial = material;
	}


	void Update()
	{
		if(isDead)
			return;

		// engine Sound
		engineSound.pitch = Mathf.Clamp01(speed/takeoffSpeed).Remap(0f, 1f, 0.95f, 1.05f);
		
		// Wait for first passengers
		if(!engineStarted)
		{
			if(passengerCount >= capacity)
			{
				engineStarted = true;
			}
		}
		

		bool takeoff = false;
		if(engineStarted)
		{
			//NS.Gamepad.motorSpeeds += new Vector2(0.1f, 0f);
			NS.Gamepad.motorSpeeds += new Vector2(0f, 0.1f);
			float breakValue = breakAction.ReadValue<float>();
			if(gas <= 0f)
			{
				breakValue = Mathf.Max(0.1f, breakValue);
			}
			if(breakValue>0.0f) // Break
			{
				// Descent
				if(onGround)
				{
					upVelocity = 0.0f;
					float bp = isInAirport ? breakPower : breakPower * 0.5f;
					speed -= bp * breakValue * Time.deltaTime;
				}
				else
				{
					upVelocity -= landingSpeed * breakValue * Time.deltaTime;
				}
			}
			else
			{
				// Rise
				if(speed >= takeoffSpeed)
				{
					float v = flyingHeight - transform.position.magnitude;
					upVelocity += v * Time.deltaTime;
					upVelocity -= upVelocity * Time.deltaTime;
					if(upVelocity >= 0.0f)
					{
						takeoff = true;
					}
				}
				speed += acceleration * Time.deltaTime;

				// consume gas
				gas -= 0.769f * Time.deltaTime;
				gas = Mathf.Max(0.0f, gas);
			}

			// Boost
			bool lastBoosting = boosting;
			float speedLimit = maxSpeed;
			if(enableBoost && boost > 0f && !onGround && boostAction.ReadValue<float>() > 0f)
			{
				speed += acceleration * 2.0f *  Time.deltaTime;
				boostEffectL.gameObject.SetActive(true);
				boostEffectR.gameObject.SetActive(true);
				boost -= (1.0f/5.0f) * Time.deltaTime; // consume boost energy (5sec)
				boosting = true;
				speedLimit = maxBoostSpeed;
				NS.Gamepad.motorSpeeds += new Vector2(0.3f, 0.0f);
			}
			else
			{
				boosting = false;
			}

			if(boosting && !lastBoosting)
			{
				boostSound.DOKill();
				boostEffectL.DOScale(new Vector3(1f, 1f, 1f), 0.2f);
				boostEffectR.DOScale(new Vector3(1f, 1f, 1f), 0.2f);
				boostSound.DOFade(0.5f, 0.3f);
				boostSound.Play();
			}
			if(!boosting && lastBoosting)
			{
				boostSound.DOKill();
				boostEffectL.DOScale(new Vector3(0f, 0f, 0f), 0.2f);
				boostEffectR.DOScale(new Vector3(0f, 0f, 0f), 0.2f);
				boostSound.DOFade(0.0f, 0.5f).OnComplete(()=>boostSound.Stop());
			}

			// Clamp Speed
			speed = Mathf.Clamp(speed, 0.0f, maxBoostSpeed);
			if(speed > speedLimit)
			{
				speed = Mathf.MoveTowards(speed, speedLimit, acceleration*3.0f*Time.deltaTime);
			}
		}

		// Move
		Vector3 nextPos = transform.localPosition;
		nextPos += (transform.position.normalized * upVelocity) * Time.deltaTime;
		nextPos += (transform.forward * speed) * Time.deltaTime;

		Quaternion noiseRot = Quaternion.identity;
		Vector3 normal = transform.position.normalized;
		// Check the ground
		Ray ray = new Ray(nextPos, -normal);
		RaycastHit hitinfo;
		int layermask = ~(LayerMask.GetMask("Ignore Raycast") | LayerMask.GetMask("Airpalne"));//LayerMask.GetMask("Ground");
		const float distanceError = 0.1f;
		bool dirtEffectFlag = false;
		if(Physics.Raycast(ray, out hitinfo, tireSize+1.0f, layermask) && hitinfo.distance <= tireSize+distanceError && !takeoff)
		{
			// On Ground
			bool firstTouch = !onGround;
			onGround = true;
			nextPos = hitinfo.point + normal * tireSize;

			if(isInAirport)
			{
				if(firstTouch)	// landed effect
				{
					NS.Gamepad.AddImpulse(0.5f, 0.0f, 0.2f);
					SoundManager.instance.Play(tireSound, transform.position, 0.7f);
					smokeEffect.Play();
				}

				// No gas
				if(gas <= 0f && gasExposionTime == float.MaxValue)
				{
					gasExposionTime = Time.time + 5.0f;	// set explosion delay
				}
			}
			else
			{
				// Bad ground
				// Shake airplane
				float s = speed/maxSpeed; // normalize speed
				s *= s;
				noiseRot = Quaternion.Euler(0.0f, 0.0f, Random.Range(-5.0f, 5.0f)*s);

				// Take damages
				if(speed > 1.0f)
				{
					health -= 10.0f * Time.deltaTime;
					dirtEffectFlag = true;
					impulseSource.GenerateImpulse(0.1f);
					NS.Gamepad.motorSpeeds += new Vector2(0.2f, 0.2f);
				}

				// No gas
				if(gas <= 0f)
				{
					Explosion();
				}
			}
		}
		else
		{
			onGround = false;
		}

		// Dirteffect
		groundEffect.emmision = dirtEffectFlag;

		// no gas explosion
		if(gasExposionTime <= Time.time)
		{
			Explosion();
			gasExposionTime = float.MaxValue;
		}


		// Turn control
		inputAxisX = Mathf.MoveTowards(inputAxisX, moveAction.ReadValue<float>(), 8.0f*Time.deltaTime);
		float horizontal = inputAxisX;
		float damagedRotation = 0.0f;
		// Affect the wing state
		if(!leftWing.activeInHierarchy)
		{
			damagedRotation += 0.2f; // learning the plane
			if(horizontal < 0)
				horizontal *= 0.5f; // decrease handl control
			else
				horizontal += 0.2f; // if player not handling, the handle leanings
		}
		if(!rightWing.activeInHierarchy)
		{
			damagedRotation -= 0.2f;
			if(horizontal > 0)
				horizontal *= 0.5f;
			else
				horizontal -= 0.2f;
		}

		Quaternion turn = Quaternion.identity;
		if(onGround)
		{
			rollRotation = Quaternion.Slerp(rollRotation, Quaternion.identity, 4f*Time.deltaTime);
			horizontal *= speed/takeoffSpeed;	// Fast speed airplane can turning fast
		}
		else
		{
			// roll animation
			float rotation = Mathf.Clamp(horizontal + damagedRotation, -1.0f, 1.0f);
			rollRotation = Quaternion.Slerp(rollRotation, Quaternion.Euler(0.0f, 0.0f, rotation * -45.0f), 3f*Time.deltaTime);
		}
		transform.localRotation *= Quaternion.Euler(0.0f, horizontal * turnSpeed * Time.deltaTime, 0.0f); // Turn the airplane

		// Airpalne rotation
		transform.localRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, normal), normal) * rollRotation * noiseRot;

		// Airplane position
		transform.localPosition = nextPos;



		// Gettting on and off the airplane
		if(speed <= 0.2f && currentAirport)
		{
			// Dismbark
			if(nextDisembarkTime <= Time.time)
			{
				for(int i=0; i<passengers.Count; i++)
				{
					int id = (int)currentAirport.id;
					if(passengers[id] > 0)
					{
						if(scaleTweener.Elapsed() >= 0.1f)
							scaleTweener.Restart();
						moneyEffect.Emit(1);
						SoundManager.instance.Play(scoreSound, 0.7f);
						GameManager.instance.AddScore(1);
						passengers[id]--;
						Vector3 rpos = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), transform.forward) * ((transform.forward * Random.Range(-1.0f, 1.0f) * 1.5f));
						PassengerManager.instance.Spawn(transform.position + rpos, currentAirport.entrance.position, -1, PassengerManager.PassengerAction.Airport);
						nextDisembarkTime = Time.time + Random.Range(0.01f, 0.03f);
						break;
					}
				}
			}

			// Boarding
			if(lastAirport != currentAirport)
			{
				// int airportcnt = GameManager.instance.airportList.Count;
				lastAirport = currentAirport;
				// int cnt = 10 - passengerCount;
				// for(int i=0; i<cnt; i++)
				// {
				// 	int targetid = 0;
				// 	while(true)
				// 	{
				// 		targetid = Random.Range(0, airportcnt);
				// 		if(targetid == currentAirport.id){continue;}
				// 		break;
				// 	}
				// 	passengers[targetid]++;
				// }
			}
		}


		// Wing trail
		if(transform.position.magnitude >= flyingHeight-2.0f)
		{
			wingTrailLeft.emitting = leftWing.activeInHierarchy;
			wingTrailRight.emitting = rightWing.activeInHierarchy;
		}
		else
		{
			wingTrailLeft.emitting = false;
			wingTrailRight.emitting = false;
		}

		if(health <= 0.0f)
		{
			Explosion();
		}
	}

	public void Board(int id)
	{
		if(nextPlayBoardSoundTime <= Time.time)
		{
			SoundManager.instance.Play(boardSound, 0.7f);
			nextPlayBoardSoundTime = Time.time + 0.05f;
		}
		if(id < passengers.Count)
			passengers[id]++;
		
		if(scaleTweener == null)
		{
			scaleTweener = transform.DOScale(1.1f, 0.5f).SetEase(Ease.OutElastic).SetLoops(1, LoopType.Yoyo).SetAutoKill(false);
		}
		else
		{
			//scaleTweener.ChangeStartValue(transform.localScale);
			scaleTweener.Restart();
		}
	}

	public void Fix()
	{
		leftWing.SetActive(true);
		rightWing.SetActive(true);
		leftWingSmoke.Stop();
		rightWingSmoke.Stop();
	}

	public void Refuel(float value)
	{
		gas = Mathf.Min(gas+value, maxGas);
		gasExposionTime = float.MaxValue;
	}

	public void EquipShield()
	{
		material.SetFloat("_Shield", 1.0f);
		enableShield = true;
	}
	public void BreakShield()
	{
		breakShieldEffect.Play();
		SoundManager.instance.Play(breakShieldSound, 0.4f, 0.6f);
		material.SetFloat("_Shield", 0.0f);
		enableShield = false;
	}

	public override void TakeDamage(DamageType type)
	{
		if(enableShield)
		{
			UI.instance.HUD.BreakShield();
			BreakShield();
			invincibleTime = Time.time + 1.0f;
			impulseSource.GenerateImpulse();
		}
		else
		{
			base.TakeDamage(type);
		}
	}

	// protected override void OnCollisionEnter(Collision collision)
	// {
	// 	// Ignore ground collision
	// 	if((1<<collision.transform.gameObject.layer & LayerMask.GetMask("Ground")) != 0)
	// 		return;

	// 	if(enableShield)
	// 	{
	// 		UI.instance.HUD.BreakShield();
	// 		BreakShield();
	// 		invincibleTime = Time.time + 1.0f;
	// 		// TODO: Play shield break sound
	// 		impulseSource.GenerateImpulse();
	// 	}
	// 	else
	// 	{
	// 		base.OnCollisionEnter(collision);
	// 	}
	// }

	void OnGUI()
	{
		return;
		if(!Application.isEditor)
			return;

		GUIStyle style = new GUIStyle();
		style.fontSize = 30;
		style.normal.textColor = Color.white;
		GUILayout.Label("OnAirport " + onAirport, style);
		string airportname = (currentAirport != null) ? currentAirport.name : "Ground";
		if(speed <= 0.2f)
		{
			GUILayout.Label("LANDED ON " + airportname, style);
		}
		else
		{
			GUILayout.Label("MOVING", style);
		}

		// Score
		GUILayout.Label(GameManager.instance.score + "Point", style);
	}
}
