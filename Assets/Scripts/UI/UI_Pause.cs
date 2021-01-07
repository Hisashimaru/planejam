using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Pause : MonoBehaviour
{
	public Button startButton;
	public Slider bgmSlider;
	public Slider sfxSlider;

	public void Active()
	{
		// Select UI Object
		GameObject selected = null;
		if(NS.Gamepad.useGamepad)
		{
			selected = startButton.gameObject;
		}
		EventSystem.current.SetSelectedGameObject(selected);

		// Update sound volume slider
		bgmSlider.value = SoundManager.instance.bgmVolume;
		sfxSlider.value = SoundManager.instance.sfxVolume;
	}


	public void OnClickResueme()
	{
		GameManager.instance.Pause();
	}

	public void OnClickRestart()
	{
		GameManager.instance.Restart();
	}

	public void OnClickQuit()
	{
		Application.Quit();
	}

	public void OnChangeBGM(Slider slider)
	{
		SoundManager.instance.bgmVolume = slider.value;
	}

	public void OnChangeSFX(Slider slider)
	{
		SoundManager.instance.sfxVolume = slider.value;
	}
}
