using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RewardProgressData))]
public class RewardProgressDataEditor : Editor
{
    private SerializedProperty tiers;

    private void OnEnable()
    {
        tiers = serializedObject.FindProperty("tiers");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Reward Progression Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("➕ Add New Tier"))
        {
            tiers.InsertArrayElementAtIndex(tiers.arraySize);
        }

        EditorGUILayout.Space(10);

        // Tính maxGem
        int maxGem = 0;
        for (int i = 0; i < tiers.arraySize; i++)
        {
            SerializedProperty tier = tiers.GetArrayElementAtIndex(i);
            maxGem = Mathf.Max(maxGem, tier.FindPropertyRelative("requiredGem").intValue);
        }

        // Vẽ thanh tiến trình dựa trên Tier
        DrawTierProgressBar(maxGem);

        EditorGUILayout.Space(10);

        for (int i = 0; i < tiers.arraySize; i++)
        {
            SerializedProperty tier = tiers.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField($"Tier {i + 1}", EditorStyles.boldLabel);

            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                tiers.DeleteArrayElementAtIndex(i);
                break;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(
                tier.FindPropertyRelative("requiredGem"),
                new GUIContent("Required Gem")
            );

            EditorGUILayout.PropertyField(
                tier.FindPropertyRelative("freeRewardPrefab"),
                new GUIContent("Free Reward Prefab")
            );

            EditorGUILayout.PropertyField(
                tier.FindPropertyRelative("vipRewardPrefab"),
                new GUIContent("VIP Reward Prefab")
            );

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawTierProgressBar(int maxGem)
    {
        if (tiers.arraySize == 0) return;

        Rect rect = GUILayoutUtility.GetRect(200, 20);
        EditorGUI.DrawRect(rect, Color.gray); // nền

        // Vẽ mốc Tier
        for (int i = 0; i < tiers.arraySize; i++)
        {
            SerializedProperty tier = tiers.GetArrayElementAtIndex(i);
            int gem = tier.FindPropertyRelative("requiredGem").intValue;
            float percent = (float)gem / maxGem;
            float x = rect.x + rect.width * percent;

            Rect marker = new Rect(x - 1, rect.y, 2, rect.height);
            EditorGUI.DrawRect(marker, Color.yellow);
        }

        EditorGUILayout.LabelField($"Max Gem: {maxGem}");
    }
}
