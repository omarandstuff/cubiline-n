using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
	public enum STATUS { NONE, CONTINUE, MAIN_MENU};
	public STATUS status;

	public EaseOpasity back;
	public EaseOpasity panel;
	public EaseTransform panelTransform;
	public EaseOpasity panelContent;

	void FixedUpdate()
	{
		if(status != STATUS.NONE)
		{
			back.easeFace = EaseOpasity.EASE_FACE.OUT;
			panel.easeFace = EaseOpasity.EASE_FACE.OUT;
			panelTransform.easeFace = EaseTransform.EASE_FACE.OUT;
			panelContent.easeFace = EaseOpasity.EASE_FACE.OUT;
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
