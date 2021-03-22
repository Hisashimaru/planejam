using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Airport : MonoBehaviour
{
	public Color color = Color.white;
	public int unlockValue;
	[System.NonSerialized]
	public ItemType item1;
	[System.NonSerialized]
	public ItemType item2;
	public Transform entrance;
	public MeshRenderer buildingRenderer;
	[System.NonSerialized]
	public int id;
	bool isAirplaneStay;
	// passengers
	float nextSpawnTime;
	AirplanePlayer player;

	public Color materialColor{set{buildingRenderer.material.color = value;}}

	void Start()
	{
		player = GameManager.instance.player;
		if(id == 0)
		{
			item1 = ItemType.Fix;
		}
	}

	void Update()
	{
		if(!GameManager.instance.isPlaying || Time.timeScale <= 0f)
			return;

		if(isAirplaneStay && nextSpawnTime <= Time.time)
		{
			if(player.freeSeat > PassengerManager.instance.boardingPassengers)
			{
				int airportcnt = GameManager.instance.availableAirportCount;
				int targetid = 0;
				while(true)
				{
					targetid = Random.Range(0, airportcnt);
					if(targetid == id){continue;}
					break;
				}
				Vector3 rpos = Quaternion.AngleAxis(Random.Range(0.0f, 180.0f), transform.forward) * ((transform.forward * Random.Range(-1.0f, 1.0f) * 2.0f));
				PassengerManager.instance.Spawn(entrance.position + rpos, entrance.position, targetid, PassengerManager.PassengerAction.Boarding);
			}
			nextSpawnTime = Time.time + Random.Range(0.01f, 0.03f);
		}
	}

	void OnTriggerEnter(Collider collider)
	{		
		AirplanePlayer player = GameManager.instance.player;
		if(player.transform == collider.transform)
		{
			player.currentAirport = this;
			isAirplaneStay = true;
			UI_Shop.instance.UpdateButtons();
			if(GameManager.instance.isPlaying)
				UI_Shop.instance.Show(true);
		}
	}

	void OnTriggerExit(Collider collider)
	{
		AirplanePlayer player = GameManager.instance.player;
		if(player && player.transform == collider.transform && player.currentAirport == this)
		{
			player.currentAirport = null;
			isAirplaneStay = false;
			UI_Shop.instance.Show(false);
		}
	}
}
