using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TutorialData))]
public class TutorialDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TutorialData tutorial = (TutorialData)target;

        EditorGUILayout.LabelField("Tutorial Steps", EditorStyles.boldLabel);

        for (int i = 0; i < tutorial.steps.Count; i++)
        {
            var step = tutorial.steps[i];
            EditorGUILayout.BeginVertical("box");

            // --- Header ---
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Step " + (i + 1), EditorStyles.boldLabel);

            if (GUILayout.Button("▲", GUILayout.Width(25)))
            {
                if (i > 0)
                {
                    SwapSteps(tutorial.steps, i, i - 1);
                }
            }

            if (GUILayout.Button("▼", GUILayout.Width(25)))
            {
                if (i < tutorial.steps.Count - 1)
                {
                    SwapSteps(tutorial.steps, i, i + 1);
                }
            }

            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                tutorial.steps.RemoveAt(i);
                EditorUtility.SetDirty(tutorial);
                break;
            }
            EditorGUILayout.EndHorizontal();

            // --- Nội dung Step ---
            step.targetType = (TutorialTargetType)EditorGUILayout.EnumPopup("Target Type", step.targetType);

            switch (step.targetType)
            {
                case TutorialTargetType.Tile:
                    step.tilePos = EditorGUILayout.Vector2IntField("Tile Pos", step.tilePos);
                    break;
                case TutorialTargetType.Animal:
                    step.animalIndex = EditorGUILayout.IntField("Animal Index", step.animalIndex);
                    break;
                case TutorialTargetType.UIButton:
                    step.buttonName = EditorGUILayout.TextField("Button Name", step.buttonName);
                    break;
            }

            step.text_VN = EditorGUILayout.TextField("Text (VN)", step.text_VN);
            step.text_EN = EditorGUILayout.TextField("Text (EN)", step.text_EN);

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Thêm Step")) tutorial.steps.Add(new TutorialStep());

        if (GUI.changed) EditorUtility.SetDirty(tutorial);
    }

    private void SwapSteps(System.Collections.Generic.List<TutorialStep> list, int indexA, int indexB)
    {
        var temp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = temp;
        EditorUtility.SetDirty(target);
    }
}
