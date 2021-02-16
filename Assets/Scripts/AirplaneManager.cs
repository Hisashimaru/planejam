using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneManager : MonoBehaviour
{
	public static AirplaneManager instance;
	public GameObject airplanePrefab;
	float nextSpawnTime;
	float spawnDelay = 0.1f;
	Airplane player;
	[System.NonSerialized]
	public List<Airplane> airplanes = new List<Airplane>();
	public int maxAirplanes = 20;
	int targetAirplaneCount = 0;

	void Awake()
	{
		if(instance == null)
		{
			instance = this;
			Debug.Log(gameObject.GetInstanceID() + "    Set Instance");
		}
	}

	void Start()
	{
		Reset();
	}

	public void Reset()
	{
		spawnDelay = 0.1f;
		nextSpawnTime = Time.time + spawnDelay;
		player = GameManager.instance.player;
		airplanes = new List<Airplane>();
	}

	void Update()
	{
		if(!player || !GameManager.instance.isPlaying)
			return;

		targetAirplaneCount = Mathf.CeilToInt(((float)GameManager.instance.score / 1000.0f) * (float)(maxAirplanes-3)) + 3;
		if(airplanes.Count < targetAirplaneCount && nextSpawnTime <= Time.time)
		{
			float maxHeight = GameManager.instance.groundHeight + Random.Range(2f, 5.5f);
			GameObject go = Instantiate(airplanePrefab);
			Airplane airplane = go.GetComponent<Airplane>();
			airplanes.Add(airplane);
			airplane.flyingHeight = maxHeight;
			Vector3 pos = Vector3.up * maxHeight;
			pos = Quaternion.FromToRotation(Vector3.up, player.transform.position.normalized) * Quaternion.Euler(0.0f, Random.Range(0.0f, 360f), 0.0f) * Quaternion.Euler( 60f, 0.0f, 0.0f) * pos;
			airplane.transform.position = pos;

			// // Set Airplane direction
			Vector3 upvec = airplane.transform.position.normalized;
			airplane.transform.rotation = Quaternion.LookRotation(player.transform.position - pos, upvec) * Quaternion.AngleAxis(Random.Range(-20.0f, 20.0f), upvec);
			nextSpawnTime = Time.time + spawnDelay;
			spawnDelay = 1.0f - (((float)GameManager.instance.score / 1000.0f) * 0.4f);
		}
		//Debug.Log("Target :" + targetAirplaneCount + "  Current :" + airplanes.Count);
	}
}
