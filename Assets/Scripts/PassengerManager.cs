using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerManager : MonoBehaviour
{
	public static PassengerManager instance;

	public enum PassengerAction
	{
		Boarding,
		Airport,
		Drop,
		Dead,
	}
	struct Passenger
	{
		public int type;	// Passenger character type
		public Vector3 position;
		public Vector3 velocity;
		public PassengerAction action;
		public int animation;
		public Vector3 startAirportPos;	// start airport position
		public int targetAirport;		// airport id
		public float time;
		public bool active;
	}

	public int maxPassengers = 1000;
	public int maxDeadBoadies = 1000;
	public int maxCharacterType = 4;
	public int maxAnimations = 4;
	public int gridSize = 4;
	public ParticleSystem passengerParticle;
	public ParticleSystem deadBodyParticle;
	new MeshRenderer renderer;
	Passenger[] passengers;
	Passenger[] deadBodies;
	AirplanePlayer player;
	ParticleSystem.Particle[] passengerParticles;
	ParticleSystem.Particle[] deadBodyParticles;
	int deadBodyIndex = 0;

	// Sounds
	public AudioClip goreSound;
	float goreSoundTime;

	public int boardingPassengers
	{
		get
		{
			int n = 0;
			foreach(Passenger p in passengers)
			{
				if(p.active && p.action == PassengerAction.Boarding)
					n++;
			}
			return n;
		}
	}

	void Awake()
	{
		instance = this;
		passengers = new Passenger[maxPassengers];
		deadBodies = new Passenger[maxDeadBoadies];

		// Set particle size
		ParticleSystem.MainModule main = passengerParticle.main;
		main.maxParticles = maxPassengers;
		main = deadBodyParticle.main;
		main.maxParticles = maxDeadBoadies;

		passengerParticles = new ParticleSystem.Particle[maxPassengers];
		deadBodyParticles = new ParticleSystem.Particle[maxDeadBoadies];
	}

	void Start()
	{
		player = GameManager.instance.player;
	}


	void Update()
	{
		if(GameManager.instance.state == GameState.Title)
			return;

		// Action
		for(int i=0; i<maxPassengers; i++)
		{
			Passenger p = passengers[i];
			if(!p.active) continue;

			Vector3 dir = new Vector3();
			const float speed = 3.0f;
			p.position += p.velocity * Time.deltaTime;
			if(p.action == PassengerAction.Boarding)	// Boarding to the plane
			{
				if(player.isInAirport)
				{
					dir = player.transform.position - p.position;
					if(dir.sqrMagnitude <= 1.0f)
					{
						// Boarded the plane
						player.Board(p.targetAirport);
						Despawn(i);
						continue;
					}
				}
				else
				{
					// Back to the airport
					dir = p.startAirportPos - p.position;
					if(dir.sqrMagnitude <= 0.5f * 0.5f)
					{
						// Enter the airport
						Despawn(i);
						continue;
					}
				}
				// walking on the ground
				p.position = p.position.normalized * (GameManager.instance.planet.size + 0.7f + (Mathf.Sin(p.time*30.0f) * 0.1f));
				p.velocity = dir.normalized * speed;
			}
			else if(p.action == PassengerAction.Airport)	// Enter the airport
			{
				dir = p.startAirportPos - p.position;
				if(dir.sqrMagnitude <= 0.5f * 0.5f)
				{
					// Enter the airport
					Despawn(i);
					continue;
				}
				// walking on the ground
				p.position = p.position.normalized * (GameManager.instance.planet.size + 0.7f + (Mathf.Sin(p.time*30.0f) * 0.1f));
				p.velocity = dir.normalized * speed;
			}
			else if(p.action == PassengerAction.Drop)	// Dropping
			{
				if(p.position.magnitude <= GameManager.instance.planet.size)	// Touch the ground
				{
					Vector3 pos = p.position.normalized * (GameManager.instance.planet.size + 0.5f);
					BloodEffect.instance.Emit(pos);
					SpawnDeadBody(pos, p.type);

					if(goreSoundTime <= Time.time)
					{
						SoundManager.instance.Play(goreSound, pos, 1.0f, Random.Range(1f, 1.2f));
						goreSoundTime = Time.time + 0.01f;
					}
					Despawn(i);
					continue;
				}
				else
				{
					p.velocity -= p.velocity * 0.4f * Time.deltaTime; // Airdrag
					p.velocity += p.position.normalized*-9.8f*Time.deltaTime;
				}
			}

			p.time += Time.deltaTime;
			passengers[i] = p;
		}

		// Align array
		int idx = 0;
		int activeCnt = 0;
		for(int i=0; i<maxPassengers; i++)
		{
			if(passengers[i].active)
			{
				activeCnt++;
				if(i != idx)
				{
					passengers[idx] = passengers[i];
					passengers[i].active = false;
				}
				idx++;
			}
		}

		// Update particles
		int num = passengerParticle.GetParticles(passengerParticles);
		Vector3 camforward = Camera.main.transform.forward;
		for(int i=0; i<num; i++)
		{
			if(passengers[i].active)
			{
				Passenger p = passengers[i];
				Color c = new Color();
				c.r = (float)((passengers[i].type * maxAnimations) + passengers[i].animation) / (float)(gridSize * gridSize);
				c.g = Mathf.Sign(Vector3.SignedAngle(p.velocity, camforward, p.position.normalized)) >= 0f ? 0.0f : 1.0f;
				passengerParticles[i].startColor = c;
				passengerParticles[i].position = passengers[i].position;
				passengerParticles[i].remainingLifetime = 100f;
			}
			else
			{
				passengerParticles[i].remainingLifetime = 0f;
			}
		}
		passengerParticle.SetParticles(passengerParticles, num);


		// Deadbody
		num = deadBodyParticle.GetParticles(deadBodyParticles);
		for(int i=0; i<num; i++)
		{
			Color c = new Color();
			c.r = (float)((deadBodies[i].type * maxAnimations) + deadBodies[i].animation) / (float)(gridSize * gridSize);
			c.g = 0f;
			deadBodyParticles[i].position = deadBodies[i].position;
			deadBodyParticles[i].velocity = deadBodies[i].velocity;
			deadBodyParticles[i].remainingLifetime = 10f;
			deadBodyParticles[i].startColor = c;
		}
		deadBodyParticle.SetParticles(deadBodyParticles, num);
	}

	public void Spawn(Vector3 position, Vector3 airportPos, int targetAirport, PassengerAction action)
	{
		int id = FindPassenger();
		if(id < 0) return;

		passengers[id].position = position;
		passengers[id].velocity = new Vector3();
		passengers[id].action = action;
		passengers[id].animation = 0;
		passengers[id].startAirportPos = airportPos;
		passengers[id].targetAirport = targetAirport;
		passengers[id].type = Random.Range(0, maxCharacterType);
		passengers[id].active = true;

		passengerParticle.Emit(1);
	}

	public void SpawnDrop(Vector3 position, Vector3 velocity)
	{
		int id = FindPassenger();
		if(id < 0) return;

		passengers[id].position = position;
		passengers[id].velocity = velocity;
		passengers[id].action = PassengerAction.Drop;
		passengers[id].animation = 1;
		passengers[id].type = Random.Range(0, maxCharacterType);
		passengers[id].active = true;

		passengerParticle.Emit(1);
	}


	public void SpawnDeadBody(Vector3 position, int type)
	{
		if(deadBodyIndex >= maxDeadBoadies)
		{
			deadBodyIndex = 0;
		}

		int idx = deadBodyIndex;
		deadBodies[idx].position = position;
		deadBodies[idx].velocity = new Vector3();
		deadBodies[idx].type = type;
		deadBodies[idx].animation = 2;
		deadBodies[idx].active = true;
		deadBodyIndex++;

		deadBodyParticle.Emit(1);
	}


	int FindPassenger()
	{
		// Find inactive passenger
		for(int i=0; i<maxPassengers; i++)
		{
			if(!passengers[i].active)
			{
				return i;
			}
		}
		return -1;
	}

	void Despawn(int id)
	{
		passengers[id].active = false;
	}
}
