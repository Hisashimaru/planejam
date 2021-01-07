using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirportManager : MonoBehaviour
{
	public Airport startAirport;
	public List<AirportData> airportData;
	public List<GameObject> airportPrefab;
	public float spacing = 70.0f;
	[System.NonSerialized]
	public List<Airport> airportList = new List<Airport>();
	const int samplings = 1000;

	void Awake()
	{
		GameManager.instance.airportManager = this;
	}


	public void Build()
	{
		startAirport.transform.position = new Vector3(0.0f, GameManager.instance.groundHeight, 0.0f);
		airportList.Add(startAirport);
		float radius = GetComponent<Planet>().size;

		float angleSpace = (spacing/(radius*2f*Mathf.PI)) * 360.0f;
		List<Vector3> positions = new List<Vector3>();
		positions.Add(startAirport.transform.position.normalized);
		for(int i=0; i<samplings; i++)
		{
			Vector3 pos = Random.onUnitSphere;
			bool overlapped = false;
			for(int j=0; j<positions.Count; j++)
			{
				if(Vector3.Angle(pos, positions[j]) <= angleSpace)
				{
					overlapped = true;
					break;
				}
			}
			if(!overlapped)
			{
				positions.Add(pos);
			}
		}
		positions.RemoveAt(0); // Remove Start Airport position

		// Create airport
		for(int i=0; i<airportData.Count; i++)
		{
			int idx = Random.Range(0, positions.Count);
			Vector3 pos = positions[idx];
			positions.RemoveAt(idx);

			// Create Airport
			Transform t = Instantiate(airportPrefab[Random.Range(0, airportPrefab.Count)]).transform;
			t.localPosition = (pos.normalized*GameManager.instance.groundHeight);
			Quaternion rot = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
			t.localRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(rot * Vector3.forward, pos), pos);
			t.SetParent(transform, true);
			Airport port = t.GetComponent<Airport>();
			AirportData data = airportData[i];
			port.color = data.color;
			port.materialColor = data.materialColor;
			port.unlockValue = data.unlockValue;
			port.item1 = data.item1;
			port.item2 = data.item2;
			airportList.Add(port);
			if(port.unlockValue > 0)
			{
				port.gameObject.SetActive(false);
			}
		}

		// Sort airports and assign id
		airportList.Sort((a, b)=>a.unlockValue - b.unlockValue);
		for(int i=0; i<airportList.Count; i++)
			airportList[i].id = i;
	}
}
