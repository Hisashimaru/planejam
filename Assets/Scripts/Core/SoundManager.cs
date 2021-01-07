using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SoundManager : MonoBehaviour
{
	public static SoundManager instance;
	public AudioMixerGroup musicGroup;
	public AudioMixerGroup SFXGroup;
	public int maxSoundEffects = 32;

	Transform[] soundEffects;
	AudioSource[] soundEffectSources;
	AudioSource bgmSource;

	public float bgmVolume
	{
		get
		{
			float volume = 0.0f;
			musicGroup.audioMixer.GetFloat("BGMVolume", out volume);
			return Mathf.Pow(10.0f, volume/20.0f);	// decibel to linear
		}

		set
		{
			float db = -80.0f;
			if(value > 0.0f)
			{
				db = 20.0f * Mathf.Log10(value);	// linear to decibel
			}
			musicGroup.audioMixer.SetFloat("BGMVolume", db);
		}
	}


	public float sfxVolume
	{
		get
		{
			float volume = 0.0f;
			musicGroup.audioMixer.GetFloat("SFXVolume", out volume);
			return Mathf.Pow(10.0f, volume/20.0f);	// decibel to linear
		}

		set
		{
			float db = -80.0f;
			if(value > 0.0f)
			{
				db = 20.0f * Mathf.Log10(value);	// linear to decibel
			}
			musicGroup.audioMixer.SetFloat("SFXVolume", db);
		}
	}


	public float sfxInternalVolume
	{
		get
		{
			float volume = 0.0f;
			musicGroup.audioMixer.GetFloat("SFXInternalVolume", out volume);
			return Mathf.Pow(10.0f, volume/20.0f);	// decibel to linear
		}

		set
		{
			float db = -80.0f;
			if(value > 0.0f)
			{
				db = 20.0f * Mathf.Log10(value);	// linear to decibel
			}
			musicGroup.audioMixer.SetFloat("SFXInternalVolume", db);
		}
	}


	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			DestroyImmediate(gameObject);
			return;
		}

		// Sound effects sources
		soundEffects = new Transform[maxSoundEffects];
		soundEffectSources = new AudioSource[maxSoundEffects];
		for (int i = 0; i < maxSoundEffects; i++)
		{
			soundEffects[i] = new GameObject("SoundEffect").transform;
			soundEffects[i].parent = transform;
			soundEffectSources[i] = soundEffects[i].gameObject.AddComponent<AudioSource>();
			soundEffectSources[i].rolloffMode = AudioRolloffMode.Custom;
			soundEffectSources[i].maxDistance = 40.0f;
			soundEffectSources[i].outputAudioMixerGroup = SFXGroup;
		}


		// BGM sources
		bgmSource = gameObject.AddComponent<AudioSource>();
		bgmSource.outputAudioMixerGroup = musicGroup;
		bgmSource.loop = true;

		bgmVolume = 1.0f;
		sfxVolume = 1.0f;
	}


	void Start()
	{
		//sfxVolume = GameManager.instance.saveData.sfxVolue;
		//bgmVolume = GameManager.instance.saveData.bgmVolume;
	}


	// Play 3D Sound
	public void Play(string name, Vector3 pos, float volume=1.0f, float pitch=1.0f)
	{
		// Load a audio clip
		AudioClip clip = Resources.Load("Sounds/" + name) as AudioClip;
		if(clip == null)
		{
			Debug.LogWarning("Resource Not Found :" + "Sounds/" + name);
			return;
		}

		Play(clip, pos, volume, pitch);
	}

	public void Play(AudioClip clip, Vector3 pos, float volume=1.0f, float pitch=1.0f)
	{
		// Find idle audio source
		foreach(AudioSource src in soundEffectSources)
		{
			if(src.isPlaying == false)
			{
				src.spatialBlend = 1.0f;	// 3D sound
				src.clip = clip;
				src.transform.position = pos;
				src.volume = volume;
				src.pitch = pitch;
				src.Play();
				return;
			}
		}
	}


	// Play 2D Sound
	public void Play(string name, float volume=1.0f, float pitch=1.0f)
	{
		// Load a audio clip
		AudioClip clip = Resources.Load("Sounds/" + name) as AudioClip;
		if(clip == null)
		{
			Debug.LogWarning("Resource Not Found :" + "Sounds/" + name);
			return;
		}

		Play(clip, volume, pitch);
	}

	public void Play(AudioClip clip, float volume=1.0f, float pitch=1.0f)
	{
		if(clip == null)
			return;

		// Find idle audio source
		foreach(AudioSource src in soundEffectSources)
		{
			if(src.isPlaying == false)
			{
				src.spatialBlend = 0.0f;	// 2D sound
				src.clip = clip;
				src.volume = volume;
				src.pitch = pitch;
				src.Play();
				return;
			}
		}
	}


	public void PlayBGM(string name)
	{
		// Load user music
		string localpath = System.Environment.CurrentDirectory + "/Musics/music.ogg";
		if(File.Exists(localpath))
		{
			StartCoroutine("LoadLocalFile", "file://" + localpath);
		}
		else
		{
			AudioClip clip = Resources.Load("Sounds/" + name) as AudioClip;
			if(clip == null)
			{
				Debug.LogWarning("Resource Not Found :" + "Sounds/" + name);
				return;
			}
			bgmSource.clip = clip;
			bgmSource.Play();
		}
	}


	public void PlayBGM(AudioClip clip, float volume=1f)
	{
		// Load user music
		string localpath = System.Environment.CurrentDirectory + "/Musics/music.ogg";
		if(File.Exists(localpath))
		{
			StartCoroutine("LoadLocalFile", "file://" + localpath);
			return;
		}

		if(clip == null)
			return;

		bgmSource.clip = clip;
		bgmSource.volume = volume;
		bgmSource.Play();		
	}

	private IEnumerator LoadLocalFile(string path)
	{
		using(UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.OGGVORBIS))
		{
			((DownloadHandlerAudioClip)www.downloadHandler).streamAudio = true;
			yield return www.SendWebRequest();
			if(www.isHttpError || www.isNetworkError)
			{
				Debug.Log(www.error);
			}
			else
			{
				AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
				bgmSource.clip = clip;
				bgmSource.Play();
			}
		}
	}
}