using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BuildSceneAttribute))]
public sealed class BuildSceneDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.LabelField(position, label.text, "[BuildScene] requires a string field.");
            return;
        }

        string[] sceneNames = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => Path.GetFileNameWithoutExtension(scene.path))
            .ToArray();

        if (sceneNames.Length == 0)
        {
            EditorGUI.LabelField(position, label.text, "No enabled scenes in the build list.");
            return;
        }

        int currentIndex = System.Array.IndexOf(sceneNames, property.stringValue);
        if (currentIndex < 0)
        {
            // Keep a missing value visible instead of silently switching scenes.
            string missing = string.IsNullOrEmpty(property.stringValue) ? "(none)" : $"{property.stringValue} (missing)";
            sceneNames = new[] { missing }.Concat(sceneNames).ToArray();
            currentIndex = 0;
        }

        EditorGUI.BeginProperty(position, label, property);
        int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, sceneNames);
        if (selectedIndex != currentIndex)
        {
            property.stringValue = sceneNames[selectedIndex];
        }
        EditorGUI.EndProperty();
    }
}
