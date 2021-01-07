using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSpeedEffect : MonoBehaviour
{
	public float minFOV = 60.0f;
	public float maxFOV = 90.0f;
	public float boostFOV = 100.0f;
	float maxSpeed;
	CinemachineVirtualCamera vcam;
	AirplanePlayer player;

	void Start()
	{
		vcam = GetComponent<CinemachineVirtualCamera>();
		player = GameManager.instance.player;
		maxSpeed = player.maxSpeed;
	}

	void Update()
	{
		float targetFOV = 0f;
		if(player.boosting)
		{
			targetFOV = boostFOV;
		}
		else
		{
			targetFOV = Mathf.Lerp(minFOV, maxFOV, player.speed/maxSpeed);
		}
		vcam.m_Lens.FieldOfView = Mathf.MoveTowards(vcam.m_Lens.FieldOfView, targetFOV, 20.0f*Time.deltaTime);
	}
}
