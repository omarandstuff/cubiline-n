using UnityEngine;
using System.Collections;

public class PreloadScreen : MonoBehaviour
{

	void Start()
	{
		Application.LoadLevelAsync("main_menu_scene");
	}
	
	void Update()
	{
	
	}
}
