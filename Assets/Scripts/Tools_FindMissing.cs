using UnityEditor;
using UnityEngine;

public class Tools_FindMissing : EditorWindow
{
    [MenuItem("Tools/Find Missing Scripts In Scene")]
    public static void Find()
    {
        foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType<GameObject>())
        {
            var components = obj.GetComponents<Component>();
            foreach (var c in components)
            {
                if (c == null)
                    Debug.LogError("Missing script on: " + GetPath(obj), obj);
            }
        }
    }

    static string GetPath(GameObject obj)
    {
        if (obj.transform.parent == null) return obj.name;
        return GetPath(obj.transform.parent.gameObject) + "/" + obj.name;
    }
}
