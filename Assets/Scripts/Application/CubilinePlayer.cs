using System;
using UnityEngine;
using Facebook.Unity;
using System.Collections.Generic;

public class CubilinePlayer
{
	public static bool playerLogedIn;


	private static Action<bool> logInCallback = null;

	////////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////////////////// LOG IN /////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public static void LogInPlayer(Action<bool> callback = null)
	{
		if (playerLogedIn) return;
		logInCallback = callback;
#if UNITY_WEBGL && !UNITY_EDITOR
		if (!FB.IsInitialized)
		{
			// Initialize the Facebook SDK
			FB.Init(OnInitComplete, OnHideUnity);
		}
		else
		{
			// Already initialized, signal an app activation App Event
			FB.ActivateApp();
		}
#endif
	}

	private void OnInitComplete()
	{
		Debug.Log(string.Format("OnInitCompleteCalled IsLoggedIn='{0}' IsInitialized='{1}'", FB.IsLoggedIn, FB.IsInitialized));

		// LogIn
		if(FB.IsInitialized)
			FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, HandleResult);
	}

	private void OnHideUnity(bool isGameShown)
	{
		Debug.Log(string.Format("Success Response: OnHideUnity Called {0}\n", isGameShown));
	}

	private void HandleResult(ILoginResult result)
	{
		if (result == null)
		{
			Debug.Log("Null Response");
			return;
		}

		// Some platforms return the empty string instead of null.
		if (!string.IsNullOrEmpty(result.Error))
		{
			Debug.Log("Error Response: " + result.Error);
		}
		else if (result.Cancelled)
		{
			Debug.Log("Cancelled Response: " + result.RawResult);
		}
		else if (!string.IsNullOrEmpty(result.RawResult))
		{
			Debug.Log("Success Response: " + result.RawResult);
		}
		else
		{
			Debug.Log("Empty Response");
		}
		if (FB.IsLoggedIn)
		{
			// AccessToken class will have session details
			var aToken = AccessToken.CurrentAccessToken;
			// Print current access token's User ID
			Debug.Log(aToken.UserId);

			// Print current access token's granted permissions
			foreach (string perm in aToken.Permissions)
			{
				Debug.Log(perm);
			}

			playerLogedIn = true;
		}
		else
		{
			Debug.Log("User cancelled login");
			playerLogedIn = false;
		}
		logInCallback(playerLogedIn);
	}
}
