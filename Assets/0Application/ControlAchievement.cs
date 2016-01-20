﻿using UnityEngine;
using System.Collections;

public class ControlAchievement : MonoBehaviour
{
	public enum ACHIEVEMNT { BLUE_COLOR, ORANGE_COLOR, GREEN_COLOR, YELLOW_COLOR, RED_COLOR, PURPLE_COLOR, SCORE_COLOR, LENGTH_COLOR, FILL_COLOR }
	public ACHIEVEMNT achievement;
	
	public void Check()
	{
		if (achievement == ACHIEVEMNT.BLUE_COLOR)
			CubilineApplication.singleton.CheckBlueColorAchievement();
		else if (achievement == ACHIEVEMNT.ORANGE_COLOR)
			CubilineApplication.singleton.CheckOrangeColorAchievement();
		else if (achievement == ACHIEVEMNT.GREEN_COLOR)
			CubilineApplication.singleton.CheckGreenColorAchievement();
		else if (achievement == ACHIEVEMNT.YELLOW_COLOR)
			CubilineApplication.singleton.CheckYellowColorAchievement();
		else if (achievement == ACHIEVEMNT.RED_COLOR)
			CubilineApplication.singleton.CheckRedColorAchievement();
		else if (achievement == ACHIEVEMNT.PURPLE_COLOR)
			CubilineApplication.singleton.CheckPurpleColorAchievement();
		else if (achievement == ACHIEVEMNT.SCORE_COLOR)
			CubilineApplication.singleton.CheckScoreColorAchievement();
		else if (achievement == ACHIEVEMNT.LENGTH_COLOR)
			CubilineApplication.singleton.CheckLengthColorAchievement();
		else if (achievement == ACHIEVEMNT.FILL_COLOR)
			CubilineApplication.singleton.CheckFillColorAchievement();

	}
}
