using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[DefaultExecutionOrder(-1)]
public class GameScene : MonoBehaviour
{
	public static GameScene instance;
	public AirplanePlayer player;
	public CinemachineVirtualCamera mainCamera;
	public CinemachineVirtualCamera titleCamera;
	public Planet planet;

	void Awake()
	{
		instance = this;
	}
}
