using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

[Serializable]
public class GameData
{
    public float time;
    public int coins;
    public Vector3 ballPosition;
    public Vector2 ballVelocity;
    // Puedes añadir más campos aquí según sea necesario para tu juego
}

public static class SaveSystem
{
    private const string FILE_NAME = "gameData.json";
    private const string ENCRYPTION_KEY = "YourSecretKeyHere123"; // Cambiado a 16, 24 o 32 bytes

    public static void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(data);
        string encryptedJson = EncryptString(json);
        File.WriteAllText(GetFilePath(), encryptedJson);
    }

    public static GameData LoadGame()
    {
        string path = GetFilePath();
        if (File.Exists(path))
        {
            string encryptedJson = File.ReadAllText(path);
            string json = DecryptString(encryptedJson);
            return JsonUtility.FromJson<GameData>(json);
        }
        return null;
    }

    private static string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, FILE_NAME);
    }

    public static void DeleteSaveFile()
    {
        string path = GetFilePath();
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private static string EncryptString(string text)
    {
        byte[] iv = new byte[16];
        byte[] array;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(ENCRYPTION_KEY);
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                    {
                        streamWriter.Write(text);
                    }

                    array = memoryStream.ToArray();
                }
            }
        }

        return Convert.ToBase64String(array);
    }

    private static string DecryptString(string cipherText)
    {
        byte[] iv = new byte[16];
        byte[] buffer = Convert.FromBase64String(cipherText);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(ENCRYPTION_KEY);
            aes.IV = iv;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }
    }

    public static bool SaveFileExists()
    {
        return File.Exists(GetFilePath());
    }

    public static void BackupSaveFile()
    {
        string originalPath = GetFilePath();
        string backupPath = originalPath + ".backup";

        if (File.Exists(originalPath))
        {
            File.Copy(originalPath, backupPath, true);
        }
    }

    public static bool RestoreBackup()
    {
        string originalPath = GetFilePath();
        string backupPath = originalPath + ".backup";

        if (File.Exists(backupPath))
        {
            File.Copy(backupPath, originalPath, true);
            return true;
        }
        return false;
    }
}