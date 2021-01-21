using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UI : MonoBehaviour
{
	public static UI instance;
	public HUD HUD;
	public UI_Tutorial Tutorial;
	public UI_Shop Shop;
	public Compass compass;
	[SerializeField]
	UI_GameOver ui_GameOver = null;
	[SerializeField]
	CanvasGroup titleCanvas = null;
	[SerializeField]
	CanvasGroup hudCanvas = null;
	[SerializeField]
	CanvasGroup blackCanvas = null;
	[SerializeField]
	UI_Pause pauseUI = null;
	
	void Awake()
	{
		if(instance != null)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
		pauseUI.gameObject.SetActive(false);
		DontDestroyOnLoad(gameObject);
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void RunOnStart()
	{
		instance = null;
	}

	public void Reset()
	{
		compass.Reset();
		HUD.ResetHUD();
		Tutorial.Reset();
		Shop.Reset();
	}

	public void HideTitle()
	{
		titleCanvas.DOFade(0f, 1f).SetUpdate(true);
	}

	public void ShowHUD()
	{
		hudCanvas.DOFade(1f, 0.4f);
	}

	public void HideHUD()
	{
		hudCanvas.DOFade(0f, 0.4f);
	}

	public void ShowGameOver()
	{
		ui_GameOver.gameObject.SetActive(true);
		ui_GameOver.Show();
	}

	public void HideGameOver()
	{
		ui_GameOver.gameObject.SetActive(false);
	}

	public void ShowPause()
	{
		pauseUI.gameObject.SetActive(true);
		pauseUI.Active();
		hudCanvas.gameObject.SetActive(false);
	}

	public void HidePause()
	{
		pauseUI.gameObject.SetActive(false);
		hudCanvas.gameObject.SetActive(true);
	}

	public void FadeIn(float time=1.0f)
	{
		blackCanvas.DOKill();
		blackCanvas.gameObject.SetActive(true);
		blackCanvas.alpha = 1.0f;
		blackCanvas.DOFade(0f, time).OnComplete(() => blackCanvas.gameObject.SetActive(false)).SetUpdate(true);
	}

	public void FadeOut(float time=1.0f)
	{
		blackCanvas.DOKill();
		blackCanvas.gameObject.SetActive(true);
		blackCanvas.alpha = 0.0f;
		blackCanvas.DOFade(1f, time).SetUpdate(true);
	}
}
