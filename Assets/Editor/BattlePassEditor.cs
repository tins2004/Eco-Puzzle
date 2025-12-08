using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BattlePassData))]
public class BattlePassEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BattlePassData reward = (BattlePassData)target;

        EditorGUILayout.LabelField("Battle Pass", EditorStyles.boldLabel);

        for (int i = 0; i < reward.steps.Count; i++)
        {
            var step = reward.steps[i];
            EditorGUILayout.BeginVertical("box");

            // --- Header ---
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Step " + (i + 1), EditorStyles.boldLabel);

            if (GUILayout.Button("▲", GUILayout.Width(25)))
            {
                if (i > 0)
                {
                    SwapSteps(reward.steps, i, i - 1);
                }
            }

            if (GUILayout.Button("▼", GUILayout.Width(25)))
            {
                if (i < reward.steps.Count - 1)
                {
                    SwapSteps(reward.steps, i, i + 1);
                }
            }

            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                reward.steps.RemoveAt(i);
                EditorUtility.SetDirty(reward);
                break;
            }
            EditorGUILayout.EndHorizontal();

            // --- Nội dung Step ---
            step.targetGem = EditorGUILayout.IntField("Target Gem", step.targetGem);

            step.freeRewardType = (RewardType)EditorGUILayout.EnumPopup("Free Reward Type", step.freeRewardType);
            // step.iconFree = (Sprite)EditorGUILayout.ObjectField("Icon Free", step.iconFree, typeof(Sprite), false);
            switch (step.freeRewardType)
            {
                case RewardType.R01_GEM:
                case RewardType.R02_BOOSTER_UPGRADE:
                case RewardType.R03_BOOSTER_SWAP:
                case RewardType.R04_BOOSTER_KEY:
                    step.quantityFree = EditorGUILayout.IntField("Quantity Free", step.quantityFree);
                    break;
                case RewardType.R05_THEME:
                    step.themeTypeFree = (ThemeType)EditorGUILayout.EnumPopup("Theme Type", step.themeTypeFree);
                    step.quantityFree = 1; 
                    break;
                case RewardType.R06_CHEST:
                    step.chestTypeFree = (ChestType)EditorGUILayout.EnumPopup("Chest Type", step.chestTypeFree);
                    step.quantityFree = 1; 
                    break;
            }

            step.vipRewardType = (RewardType)EditorGUILayout.EnumPopup("VIP Reward Type", step.vipRewardType);
            // step.iconVip = (Sprite)EditorGUILayout.ObjectField("Icon VIP", step.iconVip, typeof(Sprite), false);
            switch (step.vipRewardType)
            {
                case RewardType.R01_GEM:
                case RewardType.R02_BOOSTER_UPGRADE:
                case RewardType.R03_BOOSTER_SWAP:
                case RewardType.R04_BOOSTER_KEY:
                    step.quantityVip = EditorGUILayout.IntField("Quantity VIP", step.quantityVip);
                    break;
                case RewardType.R05_THEME:
                    step.themeTypeVip = (ThemeType)EditorGUILayout.EnumPopup("Theme Type", step.themeTypeVip);
                    step.quantityVip = 1; 
                    break;
                case RewardType.R06_CHEST:
                    step.chestTypeVip = (ChestType)EditorGUILayout.EnumPopup("Theme Type", step.chestTypeVip);
                    step.quantityVip = 1; 
                    break;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Thêm Step")) reward.steps.Add(new BattlePassStep());

        if (GUI.changed) EditorUtility.SetDirty(reward);
    }

    private void SwapSteps(System.Collections.Generic.List<BattlePassStep> list, int indexA, int indexB)
    {
        var temp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = temp;
        EditorUtility.SetDirty(target);
    }
}
