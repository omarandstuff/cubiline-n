using UnityEngine;
using System.Collections;

public class InputMedaitor : MonoBehaviour
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////

	public static InputMedaitor singleton;

	//////////////////////////////////////////////////////////////
	//////////////////////// PARAMETERS //////////////////////////
	//////////////////////////////////////////////////////////////

	public enum INPUT_KIND { KEYBOAR_MOUSE, TOUCH, JOYSTICK }
	public INPUT_KIND currentInput;

	//////////////////////////////////////////////////////////////
	////////////////////// CONTROL VARIABLES /////////////////////
	//////////////////////////////////////////////////////////////

	public static bool loaded = false;

	void Awake()
	{
		if (loaded)
		{
			Destroy(gameObject);
			return;
		}
		loaded = true;
		DontDestroyOnLoad(transform.gameObject);
		singleton = this;

		if (SystemInfo.deviceType == DeviceType.Desktop)
			currentInput = INPUT_KIND.KEYBOAR_MOUSE;
		else if (SystemInfo.deviceType == DeviceType.Handheld)
			currentInput = INPUT_KIND.TOUCH;
	}

	void OnGUI()
	{
		if (Input.touchCount > 0)
			currentInput = INPUT_KIND.TOUCH;
		else if (Input.anyKey)
			currentInput = INPUT_KIND.KEYBOAR_MOUSE;
		else if (Input.GetJoystickNames().Length > 0)
			currentInput = INPUT_KIND.JOYSTICK;
	}
}
