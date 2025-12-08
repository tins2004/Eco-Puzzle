using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(RewardData))]
public class RewardDataEditor : Editor
{

    public override void OnInspectorGUI()
    {
        RewardData reward = (RewardData)target;


        // ---- Item ----
        EditorGUILayout.LabelField("Reward Items", EditorStyles.boldLabel);
        for (int i = 0; i < reward.rewardItems.Count; i++)
        {
            var item = reward.rewardItems[i];
            EditorGUILayout.BeginVertical("box");

            // --- Header ---
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Step " + (i + 1), EditorStyles.boldLabel);

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

            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                reward.rewardItems.RemoveAt(i);
                EditorUtility.SetDirty(reward);
                break;
            }
            EditorGUILayout.EndHorizontal();

            // --- Nội dung Step ---
            item.itemType = (RewardType)EditorGUILayout.EnumPopup("Item Type", item.itemType);

            if (item.itemType == RewardType.R06_CHEST)
            {
                item.chestType = (ChestType)EditorGUILayout.EnumPopup("Chest Type", item.chestType);

                for (int j = 0; j < item.chestData.Count; j++)
                {
                    var chest = item.chestData[j];
                    EditorGUILayout.BeginVertical("Box");

                    // --- Header ---
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("item " + (j + 1) + " in chest", EditorStyles.boldLabel);

                    if (GUILayout.Button("X", GUILayout.Width(25)))
                    {
                        item.chestData.RemoveAt(j);
                        EditorUtility.SetDirty(reward);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();

                    // --- Nội dung Step ---
                    chest.rewardType = (RewardType)EditorGUILayout.EnumPopup("Reward Type", chest.rewardType);
                    chest.numberBetween = EditorGUILayout.Vector2IntField("Value Type", chest.numberBetween);

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }

                if (GUILayout.Button("Thêm Item cho Chest")) item.chestData.Add(new ChestData());
            }
            else if (item.itemType ==  RewardType.R05_THEME)
                item.themeType = (ThemeType)EditorGUILayout.EnumPopup("Theme Type", item.themeType);
            
            item.itemIcon = (Sprite)EditorGUILayout.ObjectField("Icon", item.itemIcon, typeof(Sprite), false);

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Thêm Item")) reward.rewardItems.Add(new RewardItem());

        if (GUI.changed) EditorUtility.SetDirty(reward);
    }

    private void SwapSteps(System.Collections.Generic.List<RewardItem> list, int indexA, int indexB)
    {
        var temp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = temp;
        EditorUtility.SetDirty(target);
    }
}
