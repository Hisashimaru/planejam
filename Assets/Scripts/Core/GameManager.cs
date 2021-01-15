using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.InputSystem.Controls;

public enum GameState
{
	Title,
	Wait,
	Game,
	GameOver,
	Pause,
}

[DefaultExecutionOrder(-2)]
public class GameManager : MonoBehaviour
{
	public static GameManager instance = null;
	public GameState state;
	GameState lastState;
	public GameObject airplanePrefab;
	public float baseFlyingHeight = 5.0f;
	[System.NonSerialized]
	public Planet planet = null;
	SaveData saveData;
	[System.NonSerialized]
	public AirplanePlayer player = null;

	[System.NonSerialized]
	//public List<Airport> airportList;
	public AirportManager airportManager;


	[System.NonSerialized]
	public int money;
	[System.NonSerialized]
	public int score;
	public int hiscore{get{return saveData.hiscore;} set{saveData.hiscore = value;}}
	int lastScore;
	float retryDelay = float.MaxValue;
	InputControl inputControl;
	InputAction pressAnyKeyAction =  new InputAction(type: InputActionType.PassThrough, binding: "*/<Button>", interactions: "Press");
	InputAction pauseAction;
	public AudioClip music;
	public float musicVolume = 0.5f;
	public Cinemachine.CinemachineBrain cinemachineBrain;

	[Header("Shop")]
	public int fixCost = 10;
	public int gasCost = 10;
	public int seatCost = 10;
	public int shieldCost = 25;
	public int boostCost = 25;

	CinemachineVirtualCamera gameCamera;

	public float flyingHeight{get{return planet.size + baseFlyingHeight;}}
	public float groundHeight{get{return planet.size;}}
	public int availableAirportCount
	{
		get
		{
			int cnt = 0;
			foreach(Airport p in airportManager.airportList)
				if(p.unlockValue <= score){cnt++;}

			return cnt;
		}
	}

	public bool isPause
	{
		get{return state == GameState.Pause;}
	}
	public bool isPlaying
	{
		get{return state == GameState.Game;}
	}

	void Awake()
	{
		if(instance != null)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
		saveData = new SaveData();
		saveData.Load();
		SceneManager.sceneLoaded += _OnLoadedScene;
		pressAnyKeyAction.Enable();
		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		inputControl = new InputControl();
		inputControl.Enable();
		inputControl.Player.Pause.performed += OnPause;

		SoundManager.instance.PlayBGM(music, musicVolume);
		//money = 100;
		
		// Settings
		SoundManager.instance.bgmVolume = saveData.bgm;
		SoundManager.instance.sfxVolume = saveData.sfx;
	}

	static void _OnLoadedScene(Scene scene, LoadSceneMode sceneMode)
	{
		instance.OnLoadedScene();
	}

	void OnLoadedScene()
	{
		gameCamera = GameScene.instance.mainCamera;
		player = GameScene.instance.player;
		planet = GameScene.instance.planet;
		planet.Build();

		// Reset UI
		UI.instance.HidePause();
		if(state != GameState.Title)
		{
			GameScene.instance.titleCamera.enabled = false;
			UI.instance.HideTitle();
			UI.instance.ShowHUD();
		}
		UI.instance.FadeIn(2f);
		UI.instance.HUD.ResetHUD();
		Debug.Log("Reset UI");
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void RunOnStart()
	{
		SceneManager.sceneLoaded -= _OnLoadedScene;
		instance = null;
	}

	public void Restart()
	{
		StartCoroutine(_Restart());
	}

	IEnumerator _Restart()
	{
		state = GameState.Wait;
		UI.instance.FadeOut(1f);
		yield return new WaitForSecondsRealtime(1f);
		UI.instance.HideGameOver();
		state = GameState.Game;
		score = 0;
		money = 0;
		lastScore = 0;
		retryDelay = float.MaxValue;
		SceneManager.LoadScene("game");
		Time.timeScale = 1.0f;
		gameCamera.Priority = 20;
	}

	void Update()
	{
		switch(state)
		{
			case GameState.Title:
				Title();
				break;
			case GameState.Game:
				Game();
				break;
			case GameState.GameOver:
				GameOver();
				break;
		}
	}

	void OnPause(InputAction.CallbackContext context)
	{
		Pause();
	}

	public void Pause()
	{
		if(state != GameState.Game && state != GameState.Pause)
			return;

		if(isPause)
		{
			// Resume
			Time.timeScale = 1.0f;
			state = lastState;
			UI.instance.HidePause();

			// Save settings
			saveData.bgm = SoundManager.instance.bgmVolume;
			saveData.sfx = SoundManager.instance.sfxVolume;
			saveData.Save();
		}
		else
		{
			// Pause
			lastState = state;
			state = GameState.Pause;
			Time.timeScale = 0.0f;
			UI.instance.ShowPause();
		}
	}

	void Title()
	{
		if(pressAnyKeyAction.triggered)
		{
			gameCamera.Priority = 20;
			UI.instance.HideTitle();
			StartCoroutine(StartGame());
		}
	}

	IEnumerator StartGame()
	{
		yield return new WaitForSecondsRealtime(1.2f);
		state = GameState.Game;
		cinemachineBrain.m_IgnoreTimeScale = false;
		UI.instance.ShowHUD();
	}

	void Game()
	{
		// Check player death
		if(!player && retryDelay == float.MaxValue)
		{
			retryDelay = Time.time + 1.0f;
			state = GameState.GameOver;			
			StartCoroutine(ShowGameOver());
		}

		if(Keyboard.current.rKey.wasPressedThisFrame)
		{
			Restart();
		}

		// Check unlock airport
		if(score != lastScore)
		{
			foreach(Airport airport in airportManager.airportList)
			{
				if(airport.unlockValue > lastScore && airport.unlockValue <= score)
				{
					airport.gameObject.SetActive(true);
				}
			}
			lastScore = score;
		}
	}

	void GameOver()
	{
		if(retryDelay <= Time.time)
		{
			// Retry
			if(Keyboard.current.spaceKey.wasPressedThisFrame)
			{
				saveData.Save();
				Restart();
			}
		}
	}

	public void AddScore(int value)
	{
		score += value;
		money += value;
		if(saveData.hiscore < score)
		{
			saveData.hiscore = score;
		}
	}

	IEnumerator ShowGameOver()
	{
		yield return new WaitForSeconds(1f);
		UI.instance.ShowGameOver();
		UI.instance.HideHUD();
	}
}
