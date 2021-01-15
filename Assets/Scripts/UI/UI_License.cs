using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_License : MonoBehaviour
{
	public Image image;

	public void Reset()
	{
		image.sprite = PilotLicense.instance.currentLicense.sprite;
	}

	void Update()
	{
		Sprite sp = PilotLicense.instance.currentLicense.sprite;
		if(image.sprite != sp)
		{
			Debug.Log("Update License");
			image.sprite = sp;
			image.transform.DOPunchScale(new Vector3(1.2f, 1.2f, 1.0f), 0.5f);
		}
	}
}
