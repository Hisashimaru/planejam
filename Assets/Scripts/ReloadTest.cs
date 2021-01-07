using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReloadTest : MonoBehaviour
{
	static int staticValue = 0;
	[SerializeField]int privateValue = 0;
	public int publicValue = 0;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	static void Init()
	{
		staticValue = 0;
	}

	void Start()
	{
		Debug.Log($"Static Value : {staticValue}");
		Debug.Log($"Private Value : {privateValue}");
		Debug.Log($"Public Value : {publicValue}");
	}

	void Update()
	{
		if(Keyboard.current.anyKey.wasPressedThisFrame)
		{
			staticValue = 666;
			privateValue = 666;
			publicValue = 666;
		}
	}
}
