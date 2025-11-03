using Firebase;
using Firebase.Extensions;
using UnityEngine;

public class FireBaseAnalytics : MonoBehaviour
{
    public static FireBaseAnalytics Instance;
    private bool isFirebaseReady = false;
    private Firebase.FirebaseApp app;
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;
                FirebaseApp.LogLevel = LogLevel.Debug; //-----------------------------------------------------------------------
                isFirebaseReady = true;
                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    public void LogLevelStart(int levelNumber)
    {
        if (!isFirebaseReady) return;

        bool isReplay = GameData.GetMaxLevel() >= levelNumber;
        // Debug.Log($"LogLevelStart: cur level {levelNumber}, max level {GameData.GetMaxLevel()}, isReplay(max>=cur) {isReplay}");

        Firebase.Analytics.FirebaseAnalytics.LogEvent("level_start",
            new Firebase.Analytics.Parameter("level_number", levelNumber),
            new Firebase.Analytics.Parameter("is_replay", isReplay ? 1 : 0));
    }

    public void LogLevelComplete(int levelNumber, int score)
    {
        if (!isFirebaseReady) return;
        bool isReplay = GameData.GetMaxLevel() >= levelNumber;
        // Debug.Log($"LogLevelComplete: cur level {levelNumber}, max level {GameData.GetMaxLevel()}, isReplay(max>=cur) {isReplay}");

        Firebase.Analytics.FirebaseAnalytics.LogEvent("level_complete",
            new Firebase.Analytics.Parameter("level_number", levelNumber),
            new Firebase.Analytics.Parameter("level_score", score),
            new Firebase.Analytics.Parameter("is_replay", isReplay ? 1 : 0));

    }

    public void LogLevelRetry(int levelNumber)
    {
        if (!isFirebaseReady) return;
        if (GameData.GetMaxLevel() < levelNumber) return; //Tránh trường hợp chơi lại

        Firebase.Analytics.FirebaseAnalytics.LogEvent("level_retry",
            new Firebase.Analytics.Parameter("level_number", levelNumber));
    }
}
