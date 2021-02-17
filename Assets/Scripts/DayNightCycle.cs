using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DayNightCycle : MonoBehaviour
{
	static DayNightCycle instance;
	public Material daySky;
	public Material nightSky;
	public Light light;
	Material skyMat;

	void Awake()
	{
		instance = this;
		skyMat = new Material(daySky);
		RenderSettings.skybox = skyMat;
	}

	public static Tweener ToNight()
	{
		return instance.Transition(0.0f, 1.0f, 2.0f);
	}

	public static Tweener ToDay()
	{
		return instance.Transition(1.0f, 0.0f, 2.0f);
	}

	Tweener Transition(float from, float to, float duration)
	{
		return DOVirtual.Float(from, to, duration, v=>
		{
			string name = "Color_96401022";
			Color col = Color.Lerp(daySky.GetColor(name), nightSky.GetColor(name), v);
			skyMat.SetColor(name, col);

			name = "Color_C92CD896";
			col = Color.Lerp(daySky.GetColor(name), nightSky.GetColor(name), v);
			skyMat.SetColor(name, col);
			
			RenderSettings.ambientIntensity = Mathf.Lerp(0.0f, -0.6f, v);
			light.intensity = Mathf.Lerp(1.0f, 0.1f, v);
		});
	}
}
