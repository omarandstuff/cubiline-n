using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Security.Cryptography;

[Serializable]
public class UserScores
{
	public uint match;
	public uint length;
}

public class CubilineApplication
{
	//////////////////////////////////////////////////////////////
	///////////////////////// Singleton //////////////////////////
	//////////////////////////////////////////////////////////////
	private static CubilineApplication instance;

	private CubilineApplication() { }

	public static CubilineApplication Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new CubilineApplication();
			}
			return instance;
		}
	}

	//////////////////////////////////////////////////////////////
	//////////////////////// PARAMETERS //////////////////////////
	//////////////////////////////////////////////////////////////
	public UserScores bestScores
	{
		get
		{
			return _bestScores;
		}
	}

	//////////////////////////////////////////////////////////////
	//////////////////// CONTROL VARIABLES ///////////////////////
	//////////////////////////////////////////////////////////////

	private string encryptionKey = "chocorroles_marinela";
	private string encryptionIV = "plativolos_marinela";
	private UserScores _bestScores;


	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////// SAVE / LOAD //////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public void SaveScores()
	{
		DESCryptoServiceProvider des = new DESCryptoServiceProvider();

		// Encryption
		byte[] encryptionKeyBytes = new byte[encryptionKey.Length * sizeof(char)];
		System.Buffer.BlockCopy(encryptionKey.ToCharArray(), 0, encryptionKeyBytes, 0, encryptionKeyBytes.Length);

		byte[] encryptionIVBytes = new byte[encryptionIV.Length * sizeof(char)];
		System.Buffer.BlockCopy(encryptionIV.ToCharArray(), 0, encryptionIVBytes, 0, encryptionIVBytes.Length);

		using (var fs = new FileStream(Application.persistentDataPath + "scores.dat", FileMode.Create, FileAccess.Write))
		using (var cryptoStream = new CryptoStream(fs, des.CreateEncryptor(encryptionKeyBytes, encryptionIVBytes), CryptoStreamMode.Write))
		{
			BinaryFormatter formatter = new BinaryFormatter();

			// Serialize the scores.
			formatter.Serialize(cryptoStream, _bestScores);
		}
	}

	public void LoadScores()
	{
		if (!File.Exists(Application.persistentDataPath + "scores.dat")) return;
		DESCryptoServiceProvider des = new DESCryptoServiceProvider();

		// Decryption
		byte[] encryptionKeyBytes = new byte[encryptionKey.Length * sizeof(char)];
		System.Buffer.BlockCopy(encryptionKey.ToCharArray(), 0, encryptionKeyBytes, 0, encryptionKeyBytes.Length);

		byte[] encryptionIVBytes = new byte[encryptionIV.Length * sizeof(char)];
		System.Buffer.BlockCopy(encryptionIV.ToCharArray(), 0, encryptionIVBytes, 0, encryptionIVBytes.Length);

		using (var fs = new FileStream(Application.persistentDataPath + "scores.dat", FileMode.Open, FileAccess.Read))
		using (var cryptoStream = new CryptoStream(fs, des.CreateDecryptor(encryptionKeyBytes, encryptionIVBytes), CryptoStreamMode.Read))
		{
			BinaryFormatter formatter = new BinaryFormatter();

			// Deserialize the scores.
			_bestScores = (UserScores)formatter.Deserialize(cryptoStream);
		}
	}

	public bool submitGameScore(UserScores scores)
	{
		bool newRecord = false;
		if(scores.match > _bestScores.match)
		{
			_bestScores.match = scores.match;
			newRecord = true;
		}

		bestScores.length += scores.length;

		return newRecord;
	}
}
