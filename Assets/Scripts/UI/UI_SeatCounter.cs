using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_SeatCounter : MonoBehaviour
{
	public Text counterText;
	Tween displayTween;
	CanvasGroup canvas;
	Sequence fadeOutSequence;

	void Awake()
	{
		canvas = GetComponent<CanvasGroup>();
		canvas.alpha = 0f;
	}


	public void UpdateCounter()
	{
		counterText.text = GameManager.instance.player.capacity.ToString();

		// Fadein and punch scale
		if(displayTween != null)
			displayTween.Kill();

		displayTween = canvas.DOFade(1.0f, (1f-canvas.alpha)/4f); // 1/4 sec
		transform.DORewind();
		transform.DOPunchScale(new Vector3 (.25f, .25f, .25f), .25f);


		// Fadeout
		if(fadeOutSequence != null)
			fadeOutSequence.Kill();
		fadeOutSequence = DOTween.Sequence().AppendInterval(1.0f).Append(canvas.DOFade(0.0f, 0.25f));
	}
}
