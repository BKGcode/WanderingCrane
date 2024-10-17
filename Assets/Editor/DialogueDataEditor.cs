// DialogueDataEditor.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(DialogueData))]
public class DialogueDataEditor : Editor
{
    private SerializedProperty npcNameProp;
    private SerializedProperty npcAvatarProp;
    private SerializedProperty dialogueLinesProp;

    private bool showLines = true;
    private List<string> availableLanguages = new List<string>();
    private string newLanguageCode = "";

    private Vector2 scrollPosition;

    private void OnEnable()
    {
        npcNameProp = serializedObject.FindProperty("npcName");
        npcAvatarProp = serializedObject.FindProperty("npcAvatar");
        dialogueLinesProp = serializedObject.FindProperty("dialogueLines");

        UpdateAvailableLanguages();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(npcNameProp);
        EditorGUILayout.PropertyField(npcAvatarProp);

        EditorGUILayout.Space();
        DrawLanguageManagementSection();
        EditorGUILayout.Space();

        showLines = EditorGUILayout.Foldout(showLines, "Líneas de Diálogo");

        if (showLines)
        {
            EditorGUI.indentLevel++;
            DrawDialogueTable();
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawLanguageManagementSection()
    {
        EditorGUILayout.LabelField("Gestión de Idiomas", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        newLanguageCode = EditorGUILayout.TextField("Nuevo Idioma", newLanguageCode);
        if (GUILayout.Button("Añadir Idioma", GUILayout.Width(120)))
        {
            if (!string.IsNullOrEmpty(newLanguageCode) && !availableLanguages.Contains(newLanguageCode))
            {
                AddLanguageToAllLines(newLanguageCode);
                newLanguageCode = "";
                UpdateAvailableLanguages();
            }
            else
            {
                EditorUtility.DisplayDialog("Entrada Inválida", "Por favor, introduce un código de idioma único.", "OK");
            }
        }
        EditorGUILayout.EndHorizontal();

        if (availableLanguages.Count > 0)
        {
            EditorGUILayout.LabelField("Idiomas Disponibles:", string.Join(", ", availableLanguages));
        }
        else
        {
            EditorGUILayout.LabelField("No hay idiomas disponibles.");
        }
    }

    private void UpdateAvailableLanguages()
    {
        availableLanguages.Clear();

        for (int i = 0; i < dialogueLinesProp.arraySize; i++)
        {
            SerializedProperty lineProp = dialogueLinesProp.GetArrayElementAtIndex(i);
            SerializedProperty localizedTextsProp = lineProp.FindPropertyRelative("localizedTexts");

            for (int j = 0; j < localizedTextsProp.arraySize; j++)
            {
                SerializedProperty localizedTextProp = localizedTextsProp.GetArrayElementAtIndex(j);
                SerializedProperty languageCodeProp = localizedTextProp.FindPropertyRelative("languageCode");

                string languageCode = languageCodeProp.stringValue;
                if (!availableLanguages.Contains(languageCode))
                {
                    availableLanguages.Add(languageCode);
                }
            }
        }
    }

    private void AddLanguageToAllLines(string languageCode)
    {
        for (int i = 0; i < dialogueLinesProp.arraySize; i++)
        {
            SerializedProperty lineProp = dialogueLinesProp.GetArrayElementAtIndex(i);
            SerializedProperty localizedTextsProp = lineProp.FindPropertyRelative("localizedTexts");

            bool languageExists = false;
            for (int j = 0; j < localizedTextsProp.arraySize; j++)
            {
                SerializedProperty localizedTextProp = localizedTextsProp.GetArrayElementAtIndex(j);
                SerializedProperty languageCodeProp = localizedTextProp.FindPropertyRelative("languageCode");

                if (languageCodeProp.stringValue == languageCode)
                {
                    languageExists = true;
                    break;
                }
            }

            if (!languageExists)
            {
                localizedTextsProp.InsertArrayElementAtIndex(localizedTextsProp.arraySize);
                SerializedProperty newLocalizedTextProp = localizedTextsProp.GetArrayElementAtIndex(localizedTextsProp.arraySize - 1);
                newLocalizedTextProp.FindPropertyRelative("languageCode").stringValue = languageCode;
                newLocalizedTextProp.FindPropertyRelative("text").stringValue = "";
            }
        }
    }

    private void DrawDialogueTable()
    {
        EditorGUILayout.LabelField("Editor de Diálogos", EditorStyles.boldLabel);

        // Encabezados de la tabla
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Índice", EditorStyles.boldLabel, GUILayout.Width(50));
        EditorGUILayout.LabelField("Speaker", EditorStyles.boldLabel, GUILayout.Width(70));
        foreach (string language in availableLanguages)
        {
            EditorGUILayout.LabelField(language, EditorStyles.boldLabel);
        }
        EditorGUILayout.EndHorizontal();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));

        // Mostrar cada línea de diálogo
        for (int i = 0; i < dialogueLinesProp.arraySize; i++)
        {
            SerializedProperty lineProp = dialogueLinesProp.GetArrayElementAtIndex(i);
            SerializedProperty speakerProp = lineProp.FindPropertyRelative("speaker");
            SerializedProperty localizedTextsProp = lineProp.FindPropertyRelative("localizedTexts");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(50));

            EditorGUILayout.PropertyField(speakerProp, GUIContent.none, GUILayout.Width(70));

            // Crear un diccionario para acceder rápidamente a los textos localizados
            Dictionary<string, SerializedProperty> localizedTextsDict = new Dictionary<string, SerializedProperty>();
            for (int j = 0; j < localizedTextsProp.arraySize; j++)
            {
                SerializedProperty localizedTextProp = localizedTextsProp.GetArrayElementAtIndex(j);
                string langCode = localizedTextProp.FindPropertyRelative("languageCode").stringValue;
                localizedTextsDict[langCode] = localizedTextProp.FindPropertyRelative("text");
            }

            // Mostrar campos de texto para cada idioma disponible
            foreach (string language in availableLanguages)
            {
                SerializedProperty textProp;
                if (localizedTextsDict.TryGetValue(language, out textProp))
                {
                    textProp.stringValue = EditorGUILayout.TextField(textProp.stringValue);
                }
                else
                {
                    // Si no existe la traducción, agregarla
                    localizedTextsProp.InsertArrayElementAtIndex(localizedTextsProp.arraySize);
                    SerializedProperty newLocalizedTextProp = localizedTextsProp.GetArrayElementAtIndex(localizedTextsProp.arraySize - 1);
                    newLocalizedTextProp.FindPropertyRelative("languageCode").stringValue = language;
                    textProp = newLocalizedTextProp.FindPropertyRelative("text");
                    textProp.stringValue = EditorGUILayout.TextField("");
                }
            }

            // Botón para eliminar la línea de diálogo
            if (GUILayout.Button("Eliminar", GUILayout.Width(70)))
            {
                dialogueLinesProp.DeleteArrayElementAtIndex(i);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        // Botón para añadir una nueva línea de diálogo
        if (GUILayout.Button("Añadir Nueva Línea de Diálogo"))
        {
            dialogueLinesProp.InsertArrayElementAtIndex(dialogueLinesProp.arraySize);
            SerializedProperty newLineProp = dialogueLinesProp.GetArrayElementAtIndex(dialogueLinesProp.arraySize - 1);
            newLineProp.FindPropertyRelative("speaker").enumValueIndex = 0;
            SerializedProperty newLocalizedTextsProp = newLineProp.FindPropertyRelative("localizedTexts");
            newLocalizedTextsProp.ClearArray();

            // Añadir los idiomas disponibles a la nueva línea
            foreach (string language in availableLanguages)
            {
                newLocalizedTextsProp.InsertArrayElementAtIndex(newLocalizedTextsProp.arraySize);
                SerializedProperty newLocalizedTextProp = newLocalizedTextsProp.GetArrayElementAtIndex(newLocalizedTextsProp.arraySize - 1);
                newLocalizedTextProp.FindPropertyRelative("languageCode").stringValue = language;
                newLocalizedTextProp.FindPropertyRelative("text").stringValue = "";
            }
        }
    }
}
