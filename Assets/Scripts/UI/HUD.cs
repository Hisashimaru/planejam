using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
	public Text pointText;
	// Airport counter
	public RectTransform airportCounter;
	public GameObject airportCounterPrefab;
	public Slider hpSlider;
	public Slider damageSlider;
	public Slider gasSlider;
	public Slider boostSlider;
	public UI_License license;
	public UI_SeatCounter seatCounter;

	[Header("Sheild")]
	public Transform shield;
	public ParticleSystem shieldBreakEffect;

	List<GameObject> airportCounters = new List<GameObject>();
	List<Text> airportCountTexts = new List<Text>();
	AirplanePlayer player;

	public void ResetHUD()
	{
		player = GameManager.instance.player;
		license.Reset();

		if(airportCounters.Count > 0)
			return;

		// Build Airport Counter
		var airports = GameManager.instance.airportManager.airportList;
		Debug.Log(airports.Count);
		foreach(Airport airport in airports)
		{
			GameObject go = Instantiate(airportCounterPrefab);
			Image img = go.transform.GetComponentInChildren<Image>();
			img.color = airport.color;
			Text txt = go.transform.GetComponentInChildren<Text>();
			airportCountTexts.Add(txt);
			airportCounters.Add(go);
			go.transform.SetParent(airportCounter, false);
		}

		shield.gameObject.SetActive(false);
		boostSlider.transform.parent.gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{
		// Update Damage Slider
		damageSlider.value = Mathf.MoveTowards(damageSlider.value, hpSlider.value, 0.4f*Time.deltaTime);
		
		if(player == null)
			return;
			
		// Game score
		pointText.text = GameManager.instance.score.ToString();
		

		// Update airport counter
		for(int i=0; i<player.passengers.Count; i++)
		{
			int cnt = player.passengers[i];
			if(cnt > 0)
			{
				airportCounters[i].SetActive(true);
				airportCountTexts[i].text = cnt.ToString();
			}
			else
			{
				airportCounters[i].SetActive(false);
			}
		}

		hpSlider.value = player.healthNormal;
		gasSlider.value = player.gasNormal;
		boostSlider.value = player.boost;
	}

	public void EquipShield()
	{
		shield.gameObject.SetActive(true);
	}
	
	public void BreakShield()
	{
		shield.gameObject.SetActive(false);
		shieldBreakEffect.Play();
	}

	public void ShowBoost()
	{
		boostSlider.transform.parent.gameObject.SetActive(true);
	}
}
