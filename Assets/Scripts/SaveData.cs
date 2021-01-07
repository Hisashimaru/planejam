using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
	public int hiscore = 0;
	public float sfx = 1.0f;
	public float bgm = 1.0f;

	public void Load(){Load(Application.persistentDataPath + "/save.dat");}
	public void Load(string path)
	{
		if(!File.Exists(path))
			return;
		string json = File.ReadAllText(path);
		JsonUtility.FromJsonOverwrite(json, this);
	}

	public void Save(){Save(Application.persistentDataPath + "/save.dat");}
	public void Save(string path)
	{
		File.WriteAllText(path, JsonUtility.ToJson(this));
	}
}
