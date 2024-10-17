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
    private const string PASSWORD = "ThisIsASecurePassword123!"; // Define una contraseña segura y consistente
    private static readonly byte[] SALT = Encoding.UTF8.GetBytes("UniqueSaltValue123"); // Define una sal única y consistente

    /// <summary>
    /// Guarda los datos del juego en un archivo cifrado.
    /// </summary>
    /// <param name="data">Datos del juego a guardar.</param>
    public static void SaveGame(GameData data)
    {
        if (data == null)
        {
            Debug.LogError("GameData es null. No se puede guardar el juego.");
            return;
        }

        try
        {
            string json = JsonUtility.ToJson(data);
            string encryptedJson = EncryptString(json);
            File.WriteAllText(GetFilePath(), encryptedJson);
            Debug.Log("Juego guardado correctamente.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error al guardar el juego: {ex.Message}");
        }
    }

    /// <summary>
    /// Carga los datos del juego desde un archivo cifrado.
    /// </summary>
    /// <returns>Datos del juego si existe un archivo guardado; de lo contrario, null.</returns>
    public static GameData LoadGame()
    {
        string path = GetFilePath();
        if (File.Exists(path))
        {
            try
            {
                string encryptedJson = File.ReadAllText(path);
                if (string.IsNullOrEmpty(encryptedJson))
                {
                    Debug.LogWarning("El archivo de guardado está vacío.");
                    return null;
                }

                string json = DecryptString(encryptedJson);
                GameData data = JsonUtility.FromJson<GameData>(json);
                return data;
            }
            catch (CryptographicException ex)
            {
                Debug.LogError($"Error de cifrado al cargar el juego: {ex.Message}");
                Debug.LogWarning("El archivo de guardado está corrupto o la clave/sal han cambiado.");
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error al cargar el juego: {ex.Message}");
                return null;
            }
        }
        else
        {
            Debug.LogWarning("No se encontró un archivo de guardado.");
            return null;
        }
    }

    /// <summary>
    /// Obtiene la ruta completa del archivo de guardado.
    /// </summary>
    /// <returns>Ruta del archivo de guardado.</returns>
    private static string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, FILE_NAME);
    }

    /// <summary>
    /// Elimina el archivo de guardado si existe.
    /// </summary>
    public static void DeleteSaveFile()
    {
        string path = GetFilePath();
        if (File.Exists(path))
        {
            try
            {
                File.Delete(path);
                Debug.Log("Archivo de guardado eliminado.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error al eliminar el archivo de guardado: {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró un archivo de guardado para eliminar.");
        }
    }

    /// <summary>
    /// Encripta una cadena de texto utilizando AES con una clave derivada.
    /// </summary>
    /// <param name="text">Texto a encriptar.</param>
    /// <returns>Texto encriptado en Base64.</returns>
    private static string EncryptString(string text)
    {
        using (Aes aes = Aes.Create())
        {
            // Derivar clave y IV usando Rfc2898DeriveBytes
            using (var key = new Rfc2898DeriveBytes(PASSWORD, SALT, 10000))
            {
                aes.Key = key.GetBytes(32); // 32 bytes para AES-256
                aes.IV = key.GetBytes(16);  // 16 bytes para IV
            }

            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(text);
                    }

                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }
    }

    /// <summary>
    /// Desencripta una cadena de texto en Base64 utilizando AES con una clave derivada.
    /// </summary>
    /// <param name="cipherText">Texto encriptado en Base64.</param>
    /// <returns>Texto desencriptado.</returns>
    private static string DecryptString(string cipherText)
    {
        using (Aes aes = Aes.Create())
        {
            // Derivar clave y IV usando Rfc2898DeriveBytes
            using (var key = new Rfc2898DeriveBytes(PASSWORD, SALT, 10000))
            {
                aes.Key = key.GetBytes(32); // 32 bytes para AES-256
                aes.IV = key.GetBytes(16);  // 16 bytes para IV
            }

            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] buffer = Convert.FromBase64String(cipherText);

            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader(cryptoStream))
                    {
                        string decryptedText = streamReader.ReadToEnd();
                        return decryptedText;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Verifica si existe un archivo de guardado.
    /// </summary>
    /// <returns>True si existe el archivo de guardado; de lo contrario, false.</returns>
    public static bool SaveFileExists()
    {
        return File.Exists(GetFilePath());
    }

    /// <summary>
    /// Crea una copia de seguridad del archivo de guardado.
    /// </summary>
    public static void BackupSaveFile()
    {
        string originalPath = GetFilePath();
        string backupPath = originalPath + ".backup";

        if (File.Exists(originalPath))
        {
            try
            {
                File.Copy(originalPath, backupPath, true);
                Debug.Log("Copia de seguridad del archivo de guardado creada.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error al crear la copia de seguridad: {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró un archivo de guardado para crear una copia de seguridad.");
        }
    }

    /// <summary>
    /// Restaura el archivo de guardado desde la copia de seguridad si existe.
    /// </summary>
    /// <returns>True si se restauró la copia de seguridad; de lo contrario, false.</returns>
    public static bool RestoreBackup()
    {
        string originalPath = GetFilePath();
        string backupPath = originalPath + ".backup";

        if (File.Exists(backupPath))
        {
            try
            {
                File.Copy(backupPath, originalPath, true);
                Debug.Log("Archivo de guardado restaurado desde la copia de seguridad.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error al restaurar la copia de seguridad: {ex.Message}");
                return false;
            }
        }
        else
        {
            Debug.LogWarning("No se encontró una copia de seguridad para restaurar.");
            return false;
        }
    }
}
