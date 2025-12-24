using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Event7DaysData))]
public class Event7DaysEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Event7DaysData reward = (Event7DaysData)target;


        // ---- Item ----
        EditorGUILayout.LabelField("Reward Items 7 Days", EditorStyles.boldLabel);

        if (reward.rewardItems.Count < 7)
        {
            if (GUILayout.Button("Thêm Item")) 
                reward.rewardItems.Add(new RewardOfDay() { dayNumber = reward.rewardItems.Count + 1 });
        }

        for (int i = 0; i < reward.rewardItems.Count; i++)
        {
            var item = reward.rewardItems[i];
            EditorGUILayout.BeginVertical("box");

            // --- Header ---
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Day " + (i + 1), EditorStyles.boldLabel);

            if (GUILayout.Button("▲", GUILayout.Width(25)))
            {
                if (i > 0)
                {
                    SwapSteps(reward.rewardItems, i, i - 1);
                }
            }

            if (GUILayout.Button("▼", GUILayout.Width(25)))
            {
                if (i < reward.rewardItems.Count - 1)
                {
                    SwapSteps(reward.rewardItems, i, i + 1);
                }
            }
            EditorGUILayout.EndHorizontal();

            // --- Nội dung Step ---
            item.dayNumber = i + 1;
            item.rewardType = (RewardType)EditorGUILayout.EnumPopup("Item Type", item.rewardType);

            if (item.rewardType == RewardType.R06_CHEST)
            {
                item.chestType = (ChestType)EditorGUILayout.EnumPopup("Chest Type", item.chestType);

                item.quantity = 1;
            }
            else if (item.rewardType == RewardType.R05_THEME)
            {
                item.quantity = 1;
                item.themeType = (ThemeType)EditorGUILayout.EnumPopup("Theme Type", item.themeType);
            }
            else
                item.quantity = EditorGUILayout.IntField("Quantity", item.quantity);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        // if (GUILayout.Button("Thêm Item")) reward.rewardItems.Add(new RewardItem());

        if (GUI.changed) EditorUtility.SetDirty(reward);
    }

    private void SwapSteps(System.Collections.Generic.List<RewardOfDay> list, int indexA, int indexB)
    {
        var temp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = temp;
        EditorUtility.SetDirty(target);
    }
}
