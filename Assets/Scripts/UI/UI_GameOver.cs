using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_GameOver : MonoBehaviour
{
	CanvasGroup group;
	[SerializeField] Text scoreText = null;
	[SerializeField] Text hiscoreText = null;
	[SerializeField] Image licenseImage = null;

	void Awake()
	{
		group = GetComponent<CanvasGroup>();
		group.alpha = 0.0f;
	}

	public void Show()
	{
		scoreText.text = $"SCORE {GameManager.instance.score}";
		hiscoreText.text = $"HI-SCORE {GameManager.instance.hiscore}";
		licenseImage.sprite = PilotLicense.instance.currentLicense.sprite;
		group.alpha = 0.0f;
		group.DOFade(1.0f, 1.0f);
	}
}
