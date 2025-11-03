using UnityEngine;

public class BeginManager : MonoBehaviour
{
    [SerializeField] public SceneTransition sceneTransition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sceneTransition.OpenEffect("Home Scene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
