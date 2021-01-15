using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSprite : MonoBehaviour
{
	public Sprite keyboardSprite;
	public Sprite gamepadSprite;
	Image image;

	void Awake()
	{
		image = GetComponent<Image>();
	}

	void Update()
	{
		if(image == null)
			return;

		if(NS.Gamepad.useGamepad)
		{
			if(gamepadSprite != null)
			{
				image.sprite = gamepadSprite;
				image.enabled = true;
			}
			else
			{
				image.enabled = false;
			}
		}
		else
		{
			if(keyboardSprite != null)
			{
				image.sprite = keyboardSprite;
				image.enabled = true;
			}
			else
			{
				image.enabled = false;
			}
		}
	}
}
