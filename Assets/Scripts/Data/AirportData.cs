using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
	None,
	Fix,
	Gas,
	Boost,
	Shield,
}

[CreateAssetMenu(menuName = "ScriptableObject/AirportData")]
public class AirportData : ScriptableObject
{
	public Color color = Color.white;
	public Color materialColor = Color.white;
	public int unlockValue;
	public ItemType item1;
	public ItemType item2;
}
