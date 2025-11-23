using UnityEditor;
using UnityEngine;

public class Tools_FindMissing_InProject
{
    [MenuItem("Tools/Find Missing Scripts In Project")]
    public static void FindInProject()
    {
        string[] allPrefabs = AssetDatabase.FindAssets("t:Prefab");

        foreach (var guid in allPrefabs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            var components = prefab.GetComponentsInChildren<Component>(true);
            foreach (var c in components)
            {
                if (c == null)
                    Debug.LogError("Missing script in prefab: " + path, prefab);
            }
        }
    }
}
