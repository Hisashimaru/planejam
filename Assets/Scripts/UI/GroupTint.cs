using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupTint : Graphic
{
	CanvasRenderer[] renderers;

	void Update()
	{
		renderers = GetComponentsInChildren<CanvasRenderer>();
		foreach(var r in renderers)
		{
			r.SetColor(color);
		}
	}
}
