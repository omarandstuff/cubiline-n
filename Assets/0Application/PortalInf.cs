using UnityEngine;
using System.Collections;

public class PortalInf : MonoBehaviour
{
	public GameObject back;
	public GameObject backColor;
	public GameObject text;
	public GameObject icon;
	

	public void TakeOut()
	{
		Destroy(gameObject, 1.0f);
		back.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.OUT;
		if (backColor != null) backColor.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		if(icon != null) icon.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		text.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
	}
}
