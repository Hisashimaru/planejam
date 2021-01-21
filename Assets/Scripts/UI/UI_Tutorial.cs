using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UI_Tutorial : MonoBehaviour
{
	public CanvasGroup landing;

	void Start()
	{
		landing.alpha = 0f;
	}

	public void ShowLanding()
	{
		StartCoroutine(_ShowLanding());
	}
	IEnumerator _ShowLanding()
	{
		landing.gameObject.SetActive(true);
		landing.DOFade(1.0f, 0.5f);
		yield return new WaitForSecondsRealtime(0.5f);
		yield return new WaitForSecondsRealtime(5.0f);

		landing.DOFade(0.0f, 0.5f);
		yield return new WaitForSecondsRealtime(0.5f);
		landing.gameObject.SetActive(false);
	}
}
