using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
	public static Tutorial instance;
	AirplanePlayer player;
	Transform firstAirport;

	bool gotonext = false;
	bool landing = false;
	bool license = false;

	void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
	}

	void Start()
	{
		Reset();
	}

	public void Reset()
	{
		player = GameManager.instance.player;
		firstAirport = GameManager.instance.airportManager.airportList[1].transform;
	}

	void Update()
	{
		if(player == null)
			return;
	
		// Landing
		if(!landing && Vector3.Distance(player.transform.position, firstAirport.position) < 35f)
		{
			UI.instance.Tutorial.ShowLanding();
			landing = true;
		}

		if(!license && PilotLicense.instance.currentLicense.score > 0)
		{
			UI.instance.Tutorial.ShowGetLicense();
			license = true;
		}
	}

	public void ShowGoto()
	{
		if(gotonext)
			return;
		
		UI.instance.Tutorial.ShowGoto();
		gotonext = true;
	}
}
