using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
	public GameObject markerPrefab;
	Airplane player;
	//List<RectTransform> markers = new List<RectTransform>();

	struct MarkerData
	{
		public Transform airport;
		public RectTransform marker;
		public Image image;
		public float distance;
	}
	MarkerData[] markerData;

	void Start()
	{
		List<Airport> airportList = GameManager.instance.airportManager.airportList;
		player = GameManager.instance.player;
		markerData = new MarkerData[airportList.Count];

		for(int i=0; i<airportList.Count; i++)
		{
			Airport airport = airportList[i];
			RectTransform t = (RectTransform)Instantiate(markerPrefab).transform;
			Image image = t.GetComponent<Image>();
			image.color = airport.color;
			t.SetParent(transform, false);
			//markers.Add(t);
			markerData[i].airport = airportList[i].transform;
			markerData[i].marker = t;
			markerData[i].image = image;
		}
	}

	float easing(float t)
	{
		//return 1.0f - Mathf.Pow(2, -8 * t);
		//return Mathf.Clamp01((Mathf.Pow( 2f, 8f * t ) - 1f) / 255f);
		return Mathf.Clamp01(t * t * t);
	}

	void Update()
	{
		if(player == null)
			return;
			
		Vector3 playerSide = Vector3.Cross(player.transform.position.normalized, player.transform.forward).normalized;
		float angle = 0.0f;

		for(int i=0; i<markerData.Length; i++)
		{
			RectTransform marker = markerData[i].marker;
			Transform airport = markerData[i].airport;
			Image image = markerData[i].image;
			if(!airport.gameObject.activeInHierarchy)
			{
				marker.gameObject.SetActive(false);
				continue;
			}
			else
			{
				marker.gameObject.SetActive(true);
			}

			Vector3 airportnormal = airport.transform.position.normalized;
			float dot = Vector3.Dot(player.transform.position.normalized, airportnormal);
			Vector3 airportSide = Vector3.Cross(player.transform.position.normalized, airportnormal).normalized;
			angle = -Vector3.SignedAngle(airportSide, playerSide, player.transform.position.normalized);
			angle = Mathf.Clamp(angle, -90.0f, 90.0f) / 90.0f;
			RectTransform rt = (RectTransform)transform;
			Rect rect = rt.rect;
			float x = rect.width * 0.5f * angle;
			const float errordist = 30.0f; // attenuate the angle width distance
			float dist = Vector3.Distance(airport.transform.position, player.transform.position);
			float t = Mathf.Clamp01(dist/errordist);
			x *= t * t;

			// marker Y position
			t = Vector3.Distance(player.transform.position, airport.transform.position)/(GameManager.instance.planet.size*2f); // 0.0 ~ 1.0(max is planet diameter)
			float h = rect.height * 0.05f;
			float y = Mathf.Lerp(-h, h, t);

			// Move a marker
			if(Mathf.Abs(marker.localPosition.x - x) > rect.width * 0.8f)
			{
				// move a maker immediately
				Color c = image.color;
				c.a = Mathf.MoveTowards(image.color.a, 0f, 4f*Time.deltaTime);
				image.color = c;
				if(c.a <= 0f) marker.localPosition = new Vector3(x, y, 0.0f);
			}
			else
			{
				// move a maker smoothly
				Color c = image.color;
				c.a = Mathf.MoveTowards(image.color.a, 1f, 4f*Time.deltaTime);
				image.color = c;
				marker.localPosition = Vector3.MoveTowards(marker.localPosition, new Vector3(x, y, 0.0f), rect.width*Time.deltaTime);
			}
			float scale = Mathf.Lerp(1.0f, 0.7f, t);
			marker.localScale = new Vector3(scale, scale, scale);
		}

		// Sort markers by distance between player to airport
		Vector3 playerPos = player.transform.position;
		for(int i=0; i<markerData.Length; i++)
		{
			markerData[i].distance = Vector3.Distance(markerData[i].airport.position, playerPos);
		}
		System.Array.Sort(markerData, (a,b)=>b.distance.CompareTo(a.distance));
		for(int i=0; i<markerData.Length; i++)
		{
			markerData[i].marker.SetSiblingIndex(i);
		}
	}
}
