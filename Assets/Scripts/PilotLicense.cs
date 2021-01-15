using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PilotLicense : MonoBehaviour
{
	public static PilotLicense instance;
	[System.Serializable]
	public struct LicenseData
	{
		public Sprite sprite;
		public int score;
	}

	public LicenseData[] licenses;
	public LicenseData currentLicense
	{
		get
		{
			int score = GameManager.instance.score;
			LicenseData license = licenses[0];
			foreach(LicenseData l in licenses)
			{
				if(score >= l.score)
				{
					license = l;
				}
			}
			return license;
		}
	}

	void Awake()
	{
		instance = this;
	}
}
