using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamepadSprite : MonoBehaviour
{
	public static GamepadSprite instance;

	[System.Serializable]
	public struct GamepadSpriteSet
	{
		public Sprite north;
		public Sprite west;
		public Sprite east;
		public Sprite south;
	}
	public GamepadSpriteSet xbox = new GamepadSpriteSet();
	public GamepadSpriteSet ps4 = new GamepadSpriteSet();

	public Sprite north{get{return xbox.north;}}
	public Sprite west{get{return xbox.west;}}
	public Sprite east{get{return xbox.east;}}
	public Sprite south{get{return xbox.south;}}

	void Start()
	{
		instance = this;
	}
}
