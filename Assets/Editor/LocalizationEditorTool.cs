using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class LocalizationEditorTool : EditorWindow
{
    private LocalizationData localizationData;
    private Vector2 scrollPosition;
    private string newLanguageCode = "";
    private string newLanguageName = "";
    private string newKey = "";
    private Dictionary<string, string> newTranslations = new Dictionary<string, string>();
    private string csvPath = "";

    [MenuItem("Tools/Localization Editor")]
    public static void ShowWindow()
    {
        GetWindow<LocalizationEditorTool>("Localization Editor");
    }

    private void OnGUI()
    {
        if (localizationData == null)
        {
            DrawLocalizationDataSelector();
            return;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        DrawLanguageList();
        DrawAddLanguageSection();
        DrawKeyList();
        DrawAddKeySection();
        DrawCSVImportExport();

        EditorGUILayout.EndScrollView();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(localizationData);
            AssetDatabase.SaveAssets();
        }
    }

    private void DrawLocalizationDataSelector()
    {
        EditorGUILayout.HelpBox("Please select a LocalizationData asset.", MessageType.Info);
        localizationData = EditorGUILayout.ObjectField("Localization Data", localizationData, typeof(LocalizationData), false) as LocalizationData;
    }

    private void DrawLanguageList()
    {
        EditorGUILayout.LabelField("Supported Languages", EditorStyles.boldLabel);
        foreach (var language in localizationData.SupportedLanguages)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{language.LanguageName} ({language.LanguageCode})");
            if (GUILayout.Button("Remove", GUILayout.Width(100)))
            {
                localizationData.SupportedLanguages.Remove(language);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawAddLanguageSection()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Add New Language", EditorStyles.boldLabel);
        newLanguageCode = EditorGUILayout.TextField("Language Code", newLanguageCode);
        newLanguageName = EditorGUILayout.TextField("Language Name", newLanguageName);
        if (GUILayout.Button("Add Language"))
        {
            if (!string.IsNullOrEmpty(newLanguageCode) && !string.IsNullOrEmpty(newLanguageName))
            {
                localizationData.SupportedLanguages.Add(new LocalizationData.Language
                {
                    LanguageCode = newLanguageCode,
                    LanguageName = newLanguageName
                });
                newLanguageCode = "";
                newLanguageName = "";
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid Input", "Please enter both language code and name.", "OK");
            }
        }
    }

    private void DrawKeyList()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Localization Keys and Translations", EditorStyles.boldLabel);
        
        if (localizationData.SupportedLanguages.Count == 0)
        {
            EditorGUILayout.HelpBox("Add at least one language before adding keys.", MessageType.Warning);
            return;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Key", EditorStyles.boldLabel, GUILayout.Width(200));
        foreach (var language in localizationData.SupportedLanguages)
        {
            EditorGUILayout.LabelField(language.LanguageCode, EditorStyles.boldLabel, GUILayout.Width(100));
        }
        EditorGUILayout.EndHorizontal();

        var allKeys = new HashSet<string>();
        foreach (var language in localizationData.SupportedLanguages)
        {
            foreach (var entry in language.LocalizationEntries)
            {
                allKeys.Add(entry.Key);
            }
        }

        foreach (var key in allKeys)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(key, GUILayout.Width(200));
            foreach (var language in localizationData.SupportedLanguages)
            {
                var entry = language.LocalizationEntries.Find(e => e.Key == key);
                string translation = entry != null ? entry.TranslatedText : "";
                string newTranslation = EditorGUILayout.TextField(translation, GUILayout.Width(100));
                if (newTranslation != translation)
                {
                    if (entry != null)
                    {
                        entry.TranslatedText = newTranslation;
                    }
                    else
                    {
                        language.LocalizationEntries.Add(new LocalizationData.LocalizationEntry
                        {
                            Key = key,
                            TranslatedText = newTranslation
                        });
                    }
                }
            }
            if (GUILayout.Button("Remove", GUILayout.Width(100)))
            {
                foreach (var language in localizationData.SupportedLanguages)
                {
                    language.LocalizationEntries.RemoveAll(e => e.Key == key);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawAddKeySection()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Add New Key", EditorStyles.boldLabel);
        newKey = EditorGUILayout.TextField("Key", newKey);
        
        foreach (var language in localizationData.SupportedLanguages)
        {
            if (!newTranslations.ContainsKey(language.LanguageCode))
            {
                newTranslations[language.LanguageCode] = "";
            }
            newTranslations[language.LanguageCode] = EditorGUILayout.TextField(language.LanguageCode, newTranslations[language.LanguageCode]);
        }

        if (GUILayout.Button("Add Key and Translations"))
        {
            if (!string.IsNullOrEmpty(newKey))
            {
                foreach (var language in localizationData.SupportedLanguages)
                {
                    language.LocalizationEntries.Add(new LocalizationData.LocalizationEntry
                    {
                        Key = newKey,
                        TranslatedText = newTranslations.ContainsKey(language.LanguageCode) ? newTranslations[language.LanguageCode] : ""
                    });
                }
                newKey = "";
                newTranslations.Clear();
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid Input", "Please enter a key.", "OK");
            }
        }
    }

    private void DrawCSVImportExport()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("CSV Import/Export", EditorStyles.boldLabel);
        
        csvPath = EditorGUILayout.TextField("CSV Path", csvPath);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Import CSV"))
        {
            ImportCSV();
        }
        if (GUILayout.Button("Export CSV"))
        {
            ExportCSV();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void ImportCSV()
    {
        if (string.IsNullOrEmpty(csvPath))
        {
            EditorUtility.DisplayDialog("Invalid Path", "Please enter a valid CSV path.", "OK");
            return;
        }

        try
        {
            string[] lines = File.ReadAllLines(csvPath);
            string[] headers = lines[0].Split(',');

            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(',');
                string key = values[0];

                for (int j = 1; j < headers.Length; j++)
                {
                    string languageCode = headers[j];
                    string translation = values[j];

                    var language = localizationData.SupportedLanguages.Find(l => l.LanguageCode == languageCode);
                    if (language == null)
                    {
                        language = new LocalizationData.Language { LanguageCode = languageCode, LanguageName = languageCode };
                        localizationData.SupportedLanguages.Add(language);
                    }

                    var entry = language.LocalizationEntries.Find(e => e.Key == key);
                    if (entry == null)
                    {
                        entry = new LocalizationData.LocalizationEntry { Key = key };
                        language.LocalizationEntries.Add(entry);
                    }
                    entry.TranslatedText = translation;
                }
            }

            EditorUtility.SetDirty(localizationData);
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Import Successful", "CSV data has been imported successfully.", "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Import Failed", $"Failed to import CSV: {e.Message}", "OK");
        }
    }

    private void ExportCSV()
    {
        if (string.IsNullOrEmpty(csvPath))
        {
            EditorUtility.DisplayDialog("Invalid Path", "Please enter a valid CSV path.", "OK");
            return;
        }

        try
        {
            using (StreamWriter writer = new StreamWriter(csvPath))
            {
                // Write headers
                writer.Write("Key");
                foreach (var language in localizationData.SupportedLanguages)
                {
                    writer.Write($",{language.LanguageCode}");
                }
                writer.WriteLine();

                // Write data
                var allKeys = new HashSet<string>();
                foreach (var language in localizationData.SupportedLanguages)
                {
                    foreach (var entry in language.LocalizationEntries)
                    {
                        allKeys.Add(entry.Key);
                    }
                }

                foreach (var key in allKeys)
                {
                    writer.Write(key);
                    foreach (var language in localizationData.SupportedLanguages)
                    {
                        var entry = language.LocalizationEntries.Find(e => e.Key == key);
                        string translation = entry != null ? entry.TranslatedText : "";
                        writer.Write($",{translation}");
                    }
                    writer.WriteLine();
                }
            }

            EditorUtility.DisplayDialog("Export Successful", "Data has been exported to CSV successfully.", "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Export Failed", $"Failed to export CSV: {e.Message}", "OK");
        }
    }
}