using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
	public static float Remap(this float value, float from1 = 0f, float to1 = 1f, float from2 = -1f, float to2 = 1f)
	{
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}
	public static float RemapClamp(this float value, float from1 = 0f, float to1 = 1f, float from2 = -1f, float to2 = 1f)
	{
		return Mathf.Clamp((value - from1) / (to1 - from1) * (to2 - from2) + from2, from2, to2);
	}

}