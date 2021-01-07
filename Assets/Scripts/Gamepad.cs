using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace NS
{
public class Gamepad : MonoBehaviour
{
	struct MotorImpulse
	{
		public float lowFrequency;
		public float hightFrequency;
		public float time;
	}
	static List<MotorImpulse> impulseList = new List<MotorImpulse>();
	static bool _useGamepad = false;
	public static bool useGamepad
	{
		get{return _useGamepad;}
		//set{_useGamepad = value;}
	}
	static Vector2 _motorSpeeds;
	public static Vector2 motorSpeeds
	{	get{return _motorSpeeds;}
		set
		{
			_motorSpeeds.x = Mathf.Clamp01(value.x);
			_motorSpeeds.y = Mathf.Clamp01(value.y);
		}
	}

	void Awake()
	{
		InputUser.onChange += onInputDeviceChange;
	}

	void OnDisable()
	{
		UnityEngine.InputSystem.Gamepad.current.SetMotorSpeeds(0f, 0f);
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void RunOnStart()
	{
		_motorSpeeds = new Vector2();
		impulseList = new List<MotorImpulse>();
		_useGamepad = false;
		InputUser.onChange -= onInputDeviceChange;
	}

	static void onInputDeviceChange(InputUser user, InputUserChange change, InputDevice device)
	{
		if(change != InputUserChange.ControlsChanged)
			return;

		if(user.controlScheme.Value.name == "Pad")
		{
			_useGamepad = true;
		}
		else
		{
			_useGamepad = false;
		}
	}


	public static void AddImpulse(float lowFrequency, float highFrequency, float time)
	{
		MotorImpulse impulse = new MotorImpulse();
		impulse.lowFrequency = lowFrequency;
		impulse.hightFrequency = highFrequency;
		impulse.time = Time.time + time;
		impulseList.Add(impulse);
	}

	void LateUpdate()
	{
		if(!_useGamepad)
			return;

		Vector2 motor = motorSpeeds;

		// Add impulse
		for(int i=0; i<impulseList.Count; i++)
		{
			MotorImpulse impulse = impulseList[i];
			if(impulse.time > Time.time)
			{
				motor.x += impulse.lowFrequency;
				motor.y += impulse.hightFrequency;
			}
			else
			{
				impulseList.RemoveAt(i);
				i--;
			}
		}

		motor.x = Mathf.Clamp01(motor.x);
		motor.y = Mathf.Clamp01(motor.y);
		UnityEngine.InputSystem.Gamepad.current.SetMotorSpeeds(motor.x, motor.y);
		motorSpeeds = new Vector2();	// reset motor
		//Debug.Log(motor);
	}

}
}
