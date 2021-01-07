using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlanetGenTest : MonoBehaviour
{
	//public float maxTry = 10;
	public float distance = 70.0f;
	public int samplings = 1000;
	public float planetSize = 80.0f;

	void Update()
	{
		if(Keyboard.current.gKey.isPressed)
		{
			Gen();
		}
	}

	void Gen()
	{
		float angleSpace = (distance/(planetSize*2f*Mathf.PI)) * 360.0f;
		List<Vector3> positions = new List<Vector3>();
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

		Debug.Log("Samples :" + positions.Count);
	}
}
