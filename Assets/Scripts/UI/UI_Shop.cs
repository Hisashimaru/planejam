using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UI_Shop : MonoBehaviour
{
	public static UI_Shop instance;
	[SerializeField] private InputActionAsset inputActionAsset = null;
	private InputAction buyAction0;
	private InputAction buyAction1;
	private InputAction buyAction2;


	public Text moneyText;
	public Button topButton;
	public Button leftButton;
	public Button rightButton;
	ButtonSet[] buttons = new ButtonSet[3];

	[Header("Sounds")]
	//public AudioClip purchaseSound;
	public AudioSource purchaseSound;
	public AudioSource soundEffect;
	public AudioClip fixSound;
	public AudioClip gasSound;
	public AudioClip upgradeSound;

	CanvasGroup canvasGroup;
	AirplanePlayer player;
	const string currency = "CB";
	//List<Button> activeButtons = new List<Button>();
	Dictionary<Button, Image> gamepadButtonMap = new Dictionary<Button, Image>();

	struct ButtonSet
	{
		public Button button;
		public Text name;
		public Text cost;
		public System.Func<bool> check;
	}

	void Awake()
	{
		instance = this;
		canvasGroup = GetComponent<CanvasGroup>();
		buttons[0].button = leftButton;
		buttons[0].name = leftButton.transform.Find("Text/Name").GetComponent<Text>();
		buttons[0].cost = leftButton.transform.Find("Text/Cost").GetComponent<Text>();
		buttons[1].button = rightButton;
		buttons[1].name = rightButton.transform.Find("Text/Name").GetComponent<Text>();
		buttons[1].cost = rightButton.transform.Find("Text/Cost").GetComponent<Text>();
		buttons[2].button = topButton;
		buttons[2].name = topButton.transform.Find("Text/Name").GetComponent<Text>();
		buttons[2].cost = topButton.transform.Find("Text/Cost").GetComponent<Text>();

		buyAction0 = inputActionAsset["Buy0"];
		buyAction1 = inputActionAsset["Buy1"];
		buyAction2 = inputActionAsset["Buy2"];
	}

	void Start()
	{
		player = GameManager.instance.player;
		canvasGroup.alpha = 0.0f;
		//gameObject.SetActive(false);
	}


	void Update()
	{
		if(player == null || !player.isInAirport)
			return;
		Airport port = player.currentAirport;

		// Money
		moneyText.text = GameManager.instance.money.ToString() + " " + currency;

		// Check button interactive
		for(int i=0; i<buttons.Length; i++)
		{
			if(buttons[i].check != null)
				buttons[i].button.interactable = buttons[i].check();
		}

		// Click button with gamepad
		if(buyAction0.triggered)
		{
			//activeButtons[0].onClick.Invoke();
			ExecuteEvents.Execute(buttons[0].button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
		}
		if(buyAction1.triggered)
		{
			//activeButtons[1].onClick.Invoke();
			ExecuteEvents.Execute(buttons[1].button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
		}
		if(buyAction2.triggered)
		{
			//activeButtons[2].onClick.Invoke();
			ExecuteEvents.Execute(buttons[2].button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
		}
	}


	public void UpdateButtons()
	{
		Airport port = player.currentAirport;

		// Money
		moneyText.text = GameManager.instance.money.ToString() + " " + currency;

		buttons[0].button.gameObject.SetActive(false);
		buttons[0].button.onClick.RemoveAllListeners();
		buttons[0].button.interactable = false;
		//buttons[0].check = null;
		buttons[1].button.gameObject.SetActive(false);
		buttons[1].button.onClick.RemoveAllListeners();
		buttons[1].button.interactable = false;
		//buttons[1].check = null;
		buttons[2].button.gameObject.SetActive(false);
		buttons[2].button.onClick.RemoveAllListeners();
		buttons[2].button.interactable = false;
		//buttons[2].check = null;

		int n = 0;

		// Seat
		buttons[n].button.gameObject.gameObject.SetActive(true);
		buttons[n].button.onClick.AddListener(OnClickBuySeat);
		buttons[n].check = CanBuySheat;
		buttons[n].name.text = "SEAT";
		buttons[n].cost.text = $"{GameManager.instance.seatCost}{currency}";
		n++;
		
		// Fix
		if(port.item1 == ItemType.Fix || port.item2 == ItemType.Fix)
		{
			buttons[n].button.gameObject.gameObject.SetActive(true);
			float damage = player.maxHealth - player.health;
			buttons[n].button.interactable = true;
			buttons[n].button.onClick.AddListener(OnClickFix);
			buttons[n].check = CanFix;
			buttons[n].name.text = "FIX";
			buttons[n].cost.text = $"{GameManager.instance.fixCost}{currency}";
			//float fixable = (GameManager.instance.money > damage)? damage : GameManager.instance.money;
			n++;
		}

		// Gas
		if(port.item1 == ItemType.Gas || port.item2 == ItemType.Gas)
		{
			buttons[n].button.gameObject.gameObject.SetActive(true);
			float freeSpace = player.maxGas - player.gas;
			buttons[n].button.interactable = true;
			buttons[n].button.onClick.AddListener(OnClickBuyGas);
			buttons[n].check = CanBuyGas;
			buttons[n].name.text = "GAS";
			buttons[n].cost.text = $"{GameManager.instance.gasCost}{currency}";
			//float buyableGas = (GameManager.instance.money > freeSpace)? freeSpace : GameManager.instance.money;
			n++;
		}


		// Shield
		if(port.item1 == ItemType.Shield || port.item2 == ItemType.Shield)
		{
			buttons[n].button.gameObject.SetActive(true);
			buttons[n].button.interactable = true;
			buttons[n].button.onClick.AddListener(OnClickBuyShield);
			buttons[n].check = CanBuyShield;
			buttons[n].name.text = "SHIELD";
			buttons[n].cost.text = $"{GameManager.instance.shieldCost}{currency}";
			n++;
		}


		// Boost
		if(port.item1 == ItemType.Boost || port.item2 == ItemType.Boost)
		{
			buttons[n].button.gameObject.SetActive(true);
			buttons[n].button.interactable = true;
			buttons[n].button.onClick.AddListener(OnClickBuyBoost);
			buttons[n].check = CanBuyBoost;
			buttons[n].name.text = "BOOST";
			buttons[n].cost.text = $"{GameManager.instance.shieldCost}{currency}";
			n++;
		}
	}


	public void Show(bool show)
	{
		if(show)
		{
			//gameObject.SetActive(true);
			//canvasGroup.alpha = 1.0f;
			canvasGroup.DOFade(1.0f, 0.5f);
		}
		else
		{
			//gameObject.SetActive(false);
			//canvasGroup.alpha = 0.0f;
			canvasGroup.DOFade(0.0f, 0.5f);
		}
	}

	bool CanBuySheat()
	{
		return (GameManager.instance.money >= GameManager.instance.seatCost);
	}
	bool CanFix()
	{
		float damage = player.maxHealth - player.health;
		return (damage > 0.0f && GameManager.instance.money >= GameManager.instance.fixCost);
	}
	bool CanBuyGas()
	{
		float freeSpace = player.maxGas - player.gas;
		return (freeSpace > 0.0f && GameManager.instance.money >= GameManager.instance.gasCost);
	}
	bool CanBuyShield()
	{
		return (!player.enableShield && GameManager.instance.money >= GameManager.instance.shieldCost);
	}
	bool CanBuyBoost()
	{
		return (player.boost < 1.0f && GameManager.instance.money >= GameManager.instance.boostCost);
	}

	public void OnClickFix()
	{
		//float damage = player.maxHealth - player.health;
		//float fixable = (GameManager.instance.money > damage)? damage : GameManager.instance.money;
		if(GameManager.instance.money >= 10)
		{
			GameManager.instance.money -= 10;
			player.health = Mathf.Clamp(player.health + 10.0f, 0, player.maxHealth);
			player.Fix();
			purchaseSound.Play();
			soundEffect.PlayOneShot(fixSound);
			Debug.Log("Fixed");
		}
	}


	public void OnClickBuyGas()
	{
		//float freeSpace = player.maxGas - player.gas;
		//float buyableGas = (GameManager.instance.money > freeSpace)? freeSpace : GameManager.instance.money;
		if(GameManager.instance.money >= 10)
		{
			GameManager.instance.money -= 10;
			player.Refuel(10f);
			purchaseSound.Play();
			soundEffect.PlayOneShot(gasSound);
			Debug.Log("Buy GAS");
		}
	}


	public void OnClickBuySeat()
	{
		if(GameManager.instance.money >= GameManager.instance.seatCost)
		{
			GameManager.instance.money -= GameManager.instance.seatCost;
			Debug.Log(GameManager.instance.seatCost);
			player.capacity++;
			purchaseSound.Play();
			soundEffect.PlayOneShot(upgradeSound);
			Debug.Log("Buy Seat");
		}
	}


	public void OnClickBuyShield()
	{
		if(GameManager.instance.money >= GameManager.instance.shieldCost)
		{
			GameManager.instance.money -= GameManager.instance.shieldCost;
			player.EquipShield();
			soundEffect.PlayOneShot(upgradeSound);
			UI.instance.HUD.EquipShield();
			Debug.Log("Buy Shield");
		}
	}


	public void OnClickBuyBoost()
	{
		if(GameManager.instance.money >= GameManager.instance.boostCost)
		{
			GameManager.instance.money -= GameManager.instance.boostCost;
			player.enableBoost = true;
			player.boost = 1.0f;
			soundEffect.PlayOneShot(upgradeSound);
			UI.instance.HUD.ShowBoost();
			Debug.Log("Buy Boost");
		}
	}
}
