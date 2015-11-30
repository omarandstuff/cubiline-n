using UnityEngine;
using System.Collections;

public class LogoScreenController : MonoBehaviour
{
	public EaseImageOpasity logo;
	public EaseImageOpasity backLogo;

	void Start ()
	{
		StartCoroutine(WaitLogoScreen());
	}

	IEnumerator WaitLogoScreen()
	{
		yield return new WaitForSeconds(3.0f);
		backLogo.easeFace = EaseFloat.EASE_FACE.OUT;
		yield return new WaitForSeconds(1.0f);
		logo.easeFace = EaseFloat.EASE_FACE.OUT;
		yield return new WaitForSeconds(1.0f);
		Application.LoadLevel("main_menu_scene");
	}
}
