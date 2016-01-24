﻿using UnityEngine;
using System.Collections;

public class ExitMenu : MonoBehaviour
{
	public GameObject exitPromtPrefab;

	private bool menuKey;
	private GameObject exitPromt;

	private bool lastColliderState;

	void OnGUI()
	{
		Event e = Event.current;
		if (e.type == EventType.KeyDown)
		{
			if (e.keyCode == KeyCode.Escape && !menuKey) // Menu
			{
				menuKey = true;

				if (exitPromt != null)
				{
					exitPromt.GetComponent<ExitPromt>().GoOut();
					Destroy(exitPromt.gameObject, 0.5f);
					GetComponent<Collider>().enabled = lastColliderState;
					GetComponent<MenuController>().navigationLeft.GetComponent<Collider>().enabled = lastColliderState;
					GetComponent<MenuController>().navigationRight.GetComponent<Collider>().enabled = lastColliderState;
				}
				else
				{
					exitPromt = Instantiate(exitPromtPrefab);
					exitPromt.GetComponent<ExitPromt>().exitMenu = this;
					lastColliderState = GetComponent<Collider>().enabled;
					GetComponent<Collider>().enabled = false;
					GetComponent<MenuController>().navigationLeft.GetComponent<Collider>().enabled = false;
					GetComponent<MenuController>().navigationRight.GetComponent<Collider>().enabled = false;
				}
			}
		}
		else if (e.type == EventType.keyUp)
		{
			if (e.keyCode == KeyCode.Escape)
				menuKey = false;
		}
	}

	public void CancelAction()
	{
		menuKey = false;
		exitPromt.GetComponent<ExitPromt>().GoOut();
		Destroy(exitPromt.gameObject, 0.5f);
		GetComponent<Collider>().enabled = lastColliderState;
		GetComponent<MenuController>().navigationLeft.GetComponent<Collider>().enabled = lastColliderState;
		GetComponent<MenuController>().navigationRight.GetComponent<Collider>().enabled = lastColliderState;
	}

	public void OkAction()
	{
		Application.Quit();
	}
}
