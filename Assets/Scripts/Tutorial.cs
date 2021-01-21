using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
	AirplanePlayer player;
	Transform firstAirport;

	bool landing = false;

	void Start()
	{
		player = GameManager.instance.player;
		firstAirport = GameManager.instance.airportManager.airportList[1].transform;
	}


	void Update()
	{
		// Landing
		if(!landing && Vector3.Distance(player.transform.position, firstAirport.position) < 35f)
		{
			UI.instance.Tutorial.ShowLanding();
			landing = true;
		}
	}
}
