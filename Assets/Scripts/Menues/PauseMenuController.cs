using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
	public enum STATUS { NONE, CONTINUE, MAIN_MENU};
	public STATUS status;

	public EaseImageOpasity back;
	public GameObject panel;
	public EaseImageOpasity[] buttons;
	public EaseTextOpasity[] texts;

	void FixedUpdate()
	{
		if(status != STATUS.NONE)
		{
			back.easeFace = EaseFloat.EASE_FACE.OUT;
			panel.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
			panel.GetComponent<EaseScale>().easeFace = EaseVector3.EASE_FACE.OUT;
			foreach(EaseImageOpasity io in buttons)
				io.easeFace = EaseFloat.EASE_FACE.OUT;
			foreach (EaseTextOpasity to in texts)
				to.easeFace = EaseFloat.EASE_FACE.OUT;
		}
	}

	public void Continue()
	{
		status = STATUS.CONTINUE;
	}

	public void MainMenu()
	{
		status = STATUS.MAIN_MENU;
	}
}
