using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class LocalizationEditorTool : EditorWindow
{
    private TextAsset localizationCSV;
    private Dictionary<string, Dictionary<string, string>> localizationData = new Dictionary<string, Dictionary<string, string>>();
    private Vector2 scrollPosition;
    private string newKey = "";
    private Dictionary<string, string> newTranslations = new Dictionary<string, string>();
    private List<string> languages = new List<string>();

    [MenuItem("Tools/Localization Editor")]
    public static void ShowWindow()
    {
        GetWindow<LocalizationEditorTool>("Localization Editor");
    }

    private void OnEnable()
    {
        LoadLocalizationCSV();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        localizationCSV = EditorGUILayout.ObjectField("Localization CSV", localizationCSV, typeof(TextAsset), false) as TextAsset;
        if (GUILayout.Button("Reload"))
        {
            LoadLocalizationCSV();
        }
        EditorGUILayout.EndHorizontal();

        if (localizationCSV == null)
        {
            EditorGUILayout.HelpBox("Por favor, asigna un archivo CSV de localización.", MessageType.Info);
            return;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        DrawLocalizationTable();
        DrawAddKeySection();

        EditorGUILayout.EndScrollView();

        if (GUI.changed)
        {
            SaveLocalizationCSV();
        }
    }

    private void LoadLocalizationCSV()
    {
        localizationData.Clear();
        languages.Clear();

        if (localizationCSV == null)
            return;

        string[] lines = localizationCSV.text.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2)
        {
            Debug.LogError("El archivo CSV de localización está vacío o mal formateado.");
            return;
        }

        string[] headers = lines[0].Split(',');

        for (int i = 1; i < headers.Length; i++)
        {
            string languageCode = headers[i].Trim();
            languages.Add(languageCode);
        }

        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(',');
            if (fields.Length < 2) continue;

            string key = fields[0].Trim();

            if (!localizationData.ContainsKey(key))
            {
                localizationData[key] = new Dictionary<string, string>();
            }

            for (int j = 1; j < fields.Length; j++)
            {
                string languageCode = languages[j - 1];
                string value = fields[j].Trim().Replace("\\n", "\n");
                localizationData[key][languageCode] = value;
            }
        }
    }

    private void DrawLocalizationTable()
    {
        EditorGUILayout.LabelField("Claves y Traducciones", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Clave", EditorStyles.boldLabel, GUILayout.Width(200));
        foreach (var language in languages)
        {
            EditorGUILayout.LabelField(language, EditorStyles.boldLabel, GUILayout.Width(150));
        }
        EditorGUILayout.EndHorizontal();

        foreach (var keyEntry in localizationData)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(keyEntry.Key, GUILayout.Width(200));

            foreach (var language in languages)
            {
                string translation = keyEntry.Value.ContainsKey(language) ? keyEntry.Value[language] : "";
                string newTranslation = EditorGUILayout.TextField(translation, GUILayout.Width(150));
                if (newTranslation != translation)
                {
                    keyEntry.Value[language] = newTranslation;
                }
            }

            if (GUILayout.Button("Eliminar", GUILayout.Width(100)))
            {
                localizationData.Remove(keyEntry.Key);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawAddKeySection()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Añadir Nueva Clave", EditorStyles.boldLabel);
        newKey = EditorGUILayout.TextField("Clave", newKey);

        foreach (var language in languages)
        {
            if (!newTranslations.ContainsKey(language))
            {
                newTranslations[language] = "";
            }
            newTranslations[language] = EditorGUILayout.TextField(language, newTranslations[language]);
        }

        if (GUILayout.Button("Añadir Clave y Traducciones"))
        {
            if (!string.IsNullOrEmpty(newKey) && !localizationData.ContainsKey(newKey))
            {
                Dictionary<string, string> translations = new Dictionary<string, string>();
                foreach (var language in languages)
                {
                    translations[language] = newTranslations.ContainsKey(language) ? newTranslations[language] : "";
                }
                localizationData[newKey] = translations;
                newKey = "";
                newTranslations.Clear();
            }
            else
            {
                EditorUtility.DisplayDialog("Entrada Inválida", "Por favor, introduce una clave única.", "OK");
            }
        }
    }

    private void SaveLocalizationCSV()
    {
        if (localizationCSV == null)
            return;

        string assetPath = AssetDatabase.GetAssetPath(localizationCSV);
        if (string.IsNullOrEmpty(assetPath))
            return;

        using (StreamWriter writer = new StreamWriter(assetPath))
        {
            // Escribir encabezados
            writer.Write("Key");
            foreach (var language in languages)
            {
                writer.Write($",{language}");
            }
            writer.WriteLine();

            // Escribir datos
            foreach (var keyEntry in localizationData)
            {
                writer.Write(keyEntry.Key);
                foreach (var language in languages)
                {
                    string translation = keyEntry.Value.ContainsKey(language) ? keyEntry.Value[language] : "";
                    writer.Write($",{translation}");
                }
                writer.WriteLine();
            }
        }

        AssetDatabase.Refresh();
    }
}
