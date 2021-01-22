using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UI_Tutorial : MonoBehaviour
{
	public CanvasGroup gotonetxt;
	public CanvasGroup landing;
	public CanvasGroup boosting;
	public CanvasGroup license;

	void Start()
	{
		Reset();
	}

	public void Reset()
	{
		gotonetxt.gameObject.SetActive(false);
		landing.gameObject.SetActive(false);
		license.gameObject.SetActive(false);
		boosting.gameObject.SetActive(false);
	}

	public void ShowGoto()
	{
		StartCoroutine(_ShowCanvas(gotonetxt, 5.0f, 3f));
	}

	public void ShowLanding()
	{
		StartCoroutine(_ShowCanvas(landing, 5.0f));
	}

	public void ShowGetLicense()
	{
		StartCoroutine(_ShowCanvas(license, 5.0f));
	}

	public void ShowBoosting()
	{
		StartCoroutine(_ShowCanvas(boosting, 5.0f));
	}

	IEnumerator _ShowCanvas(CanvasGroup group, float time, float delay=0f)
	{
		if(delay > 0f)
		{
			yield return new WaitForSecondsRealtime(delay);
		}

		group.gameObject.SetActive(true);
		group.alpha = 0f;
		group.DOFade(1.0f, 0.5f);
		yield return new WaitForSecondsRealtime(0.5f);
		yield return new WaitForSecondsRealtime(time);

		group.DOFade(0.0f, 0.5f);
		yield return new WaitForSecondsRealtime(0.5f);
		group.gameObject.SetActive(false);
	}
}
