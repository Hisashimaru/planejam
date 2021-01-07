using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class Planet : MonoBehaviour
{
	public float size = 50.0f; // planet radius
	public List<GameObject> mountainPrefabs;
	public List<GameObject> forestPrefabs;
	public float airportRadius = 5.0f;
	public float mountainRadius = 5.0f;
	public float objectRadius = 5.0f;
	public int mountainCount = 5;
	public int objectCount = 5;
	//List<Transform> worldObjects = new List<Transform>();
	//List<Transform> mountains = new List<Transform>();
	AirportManager airportManager;

	struct PlanetObject
	{
		public Vector3 position;
		public float radius;

		public PlanetObject(Vector3 position, float radius)
		{
			this.position = position;
			this.radius = radius;
		}
	}
	List<PlanetObject> planetObjects = new List<PlanetObject>();

	void Awake()
	{
		transform.localScale = new Vector3(size, size, size);
		airportManager = GetComponent<AirportManager>();
	}

	public void Build()
	{
		airportManager.Build();
		foreach(Airport a in airportManager.airportList)
		{
			planetObjects.Add(new PlanetObject(a.transform.position, mountainRadius));
		}

		CreateMountains(mountainCount);
		CreateWorldObjects(objectCount);
	}

	void CreateMountains(int count)
	{
		var positions = GetRandomPositions(mountainRadius);
		int cnt = positions.Count < count ? positions.Count : count;
		for(int i=0; i<cnt; i++)
		{
			Vector3 pos = positions[i];
			GameObject go = Instantiate(mountainPrefabs[Random.Range(0, mountainPrefabs.Count)]);
			Transform t = go.transform;
			Quaternion rot = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), pos.normalized) * Quaternion.FromToRotation(Vector3.up, pos.normalized);
			t.position = pos;
			t.rotation = rot;
			planetObjects.Add(new PlanetObject(pos, mountainRadius));
		}
		Debug.Log($"{cnt} Mountains ({positions.Count})");
	}

	void CreateWorldObjects(int count)
	{
		var positions = GetRandomPositions(objectRadius);
		int cnt = positions.Count < count ? positions.Count : count;
		for(int i=0; i<cnt; i++)
		{
			Vector3 pos = positions[i];
			GameObject go = Instantiate(forestPrefabs[Random.Range(0, forestPrefabs.Count)]);
			Transform t = go.transform;
			Quaternion rot = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), pos.normalized) * Quaternion.FromToRotation(Vector3.up, pos.normalized);
			t.position = pos;
			t.rotation = rot;
			planetObjects.Add(new PlanetObject(pos, objectRadius));
		}
		Debug.Log($"{cnt} World Objects ({positions.Count})");
	}

	List<Vector3> GetRandomPositions(float radius)
	{
		List<Vector3> positions = new List<Vector3>();
		for(int i=0; i<1000; i++)
		{
			Vector3 pos = Random.onUnitSphere * size;
			bool overlapped = false;

			// Check overlap with other sample
			for(int j=0; j<positions.Count; j++)
			{
				if((pos-positions[j]).sqrMagnitude < radius*radius*2)
				{
					overlapped = true;
					break;
				}
			}
			if(overlapped)
				continue;

			
			// Check overlap with airports
			foreach(PlanetObject p in planetObjects)
			{
				if((p.position-pos).magnitude < radius+p.radius)
				{
					overlapped = true;
					break;
				}
			}
			if(overlapped)
				continue;

			positions.Add(pos);
		}
		return positions;
	}
}
