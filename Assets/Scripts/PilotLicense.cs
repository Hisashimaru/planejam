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
	public GameObject[] fireworksPrefabs;

	public LicenseData currentLicense
	{
		get; protected set;
	}

	void Awake()
	{
		instance = this;
		currentLicense = licenses[0];
	}

	void Update()
	{
		UpdateLicense();
	}

	void UpdateLicense()
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

		// License Updated
		if(currentLicense.score != license.score)
		{
			currentLicense = license;
			// Got final license
			if(currentLicense.score == licenses[licenses.Length-1].score)
			{
				StartCoroutine(ShootFireworks());
			}
		}	
	}

	IEnumerator ShootFireworks()
	{
		yield return new WaitForSeconds(1.0f);
		AirplanePlayer player = GameManager.instance.player;
		for(int i=0; i<16; i++)
		{
			if(player == null)
				break;
			Vector3 pos = new Vector3(0.0f, Random.Range(85.0f, 90.0f), 0.0f);
			Vector3 playernormal= (player.transform.position + (Quaternion.AngleAxis(Random.Range(-60.0f, 60.0f), player.transform.up) * player.transform.forward * Random.Range(5f, 10f))).normalized;
			pos = Quaternion.FromToRotation(new Vector3(0f, 1f, 0f), playernormal) * pos;
			GameObject go = Instantiate(fireworksPrefabs[Random.Range(0,fireworksPrefabs.Length)], pos, Quaternion.identity);
			Destroy(go, 5.0f);
			yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
		}
	}
}
