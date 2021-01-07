using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CurvedWorld : MonoBehaviour
{
	public float power = 1.0f;

	void Awake()
	{
		if(Application.isPlaying)
		{
			Shader.EnableKeyword("CURVED_WORLD");
		}
		else
		{
			Shader.DisableKeyword("CURVED_WORLD");
		}
	}

	void Update()
	{
		Shader.SetGlobalFloat("_WORLD_CURVE_POWER", power);
	}
}
