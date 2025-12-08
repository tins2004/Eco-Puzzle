using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class BeginManager : MonoBehaviour
{
    [SerializeField] public SceneTransition sceneTransition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameData.SetCurrentScene("Begin Scene");
        
        int maxLevel = GameData.GetMaxLevel();
        if (maxLevel == 0)
        {
            sceneTransition.OpenEffect("Game Scene");
        }
        else
        {
            sceneTransition.OpenEffect("Home Scene");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
