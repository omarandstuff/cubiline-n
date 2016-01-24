using UnityEngine;
using System.Collections;

public class ExitPromt : MonoBehaviour
{
	public ExitMenu exitMenu;
	public GameObject bigBack;
	public GameObject back;
	public GameObject title;
	public GameObject okButton;
	public GameObject cancelButton;

	
	public void GoOut()
	{
		bigBack.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		back.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		back.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.OUT;
		title.GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		okButton.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		okButton.transform.GetChild(0).GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		cancelButton.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		cancelButton.transform.GetChild(0).GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
	}

	public void CancelAction()
	{
		exitMenu.CancelAction();
	}

	public void OkAction()
	{
		exitMenu.OkAction();
	}
}
