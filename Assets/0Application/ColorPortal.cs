using UnityEngine;
using System.Collections;

public class ColorPortal : MonoBehaviour
{
	public GameObject back;
	public GameObject text;
	public GameObject unlocked;
	public GameObject color;
	

	IEnumerator Start ()
	{
		yield return new WaitForSeconds(6);
		Destroy(gameObject, 1.0f);
		back.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.OUT;
		back.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		color.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		unlocked.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		text.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		CubilineApplication.singleton.waithStack.Pop();
		CubilineApplication.singleton.ShowPortal();
	}
}
