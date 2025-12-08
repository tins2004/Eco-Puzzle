using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ThemeData))]
public class ThemeDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ThemeData themes = (ThemeData)target;

        EditorGUILayout.LabelField("Theme List", EditorStyles.boldLabel);

        for (int i = 0; i < themes.listTheme.Count; i++)
        {
            var theme = themes.listTheme[i];
            EditorGUILayout.BeginVertical("box");

            // --- Header ---
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Step " + (i + 1), EditorStyles.boldLabel);


            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                themes.listTheme.RemoveAt(i);
                EditorUtility.SetDirty(themes);
                break;
            }
            EditorGUILayout.EndHorizontal();

            // --- Nội dung Step ---
            theme.themeType = (ThemeType)EditorGUILayout.EnumPopup("Theme Type", theme.themeType);
            theme.T01 = (Sprite)EditorGUILayout.ObjectField("Theme T01", theme.T01, typeof(Sprite), false);
            theme.T02 = (Sprite)EditorGUILayout.ObjectField("Theme T02", theme.T02, typeof(Sprite), false);
            theme.T03 = (Sprite)EditorGUILayout.ObjectField("Theme T03", theme.T03, typeof(Sprite), false);
            theme.T04 = (Sprite)EditorGUILayout.ObjectField("Theme T04", theme.T04, typeof(Sprite), false);
            theme.T05 = (Sprite)EditorGUILayout.ObjectField("Theme T05", theme.T05, typeof(Sprite), false);
            theme.T06 = (Sprite)EditorGUILayout.ObjectField("Theme T06", theme.T06, typeof(Sprite), false);
            theme.T07 = (Sprite)EditorGUILayout.ObjectField("Theme T07", theme.T07, typeof(Sprite), false);
            theme.T08 = (Sprite)EditorGUILayout.ObjectField("Theme T08", theme.T08, typeof(Sprite), false);
            theme.T09 = (Sprite)EditorGUILayout.ObjectField("Theme T09", theme.T09, typeof(Sprite), false);
            

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Thêm Step")) themes.listTheme.Add(new ThemeSprite());

        if (GUI.changed) EditorUtility.SetDirty(themes);
    }
}
