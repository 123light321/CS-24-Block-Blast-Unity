using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;


public class bl_PlayService : MonoBehaviour {

    public static bl_PlayService Instance;

    
    private bool userSignedIn = false;
    void Start() {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
         .Build();
        PlayGamesPlatform.InitializeInstance(config);
        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();

        requestSignin();
        
    }

    public void UnlockAchievement(string key, double score) {
        if (userSignedIn) {
            Social.ReportProgress(key, score, (bool success) => {
                if (success) {
                    print("Achievement unlocked");
                }
            });
        } else {
            requestSignin();
        }

    }

    public void IncrementAchievement (string key, int score) {
        if (userSignedIn) {

            Social.ReportProgress(key, score, (bool success) => {
                if (success) {
                    print("Achievement unlocked");
                }
            });
            // handle success or failure
        
        } else {
            requestSignin();
        }
    }

    public void ShotThisLeaderboard (string key, int score) {
        if (userSignedIn)
            Social.ReportScore(score, key, (bool success) => {
            // handle success or failure
        });
    }

    public void ShowAchievements () {

        if (userSignedIn)
            Social.ShowAchievementsUI();
        else
            requestSignin();
    }

    public void ShowLeaderboard (string key) {
        if (userSignedIn)
            Social.ShowLeaderboardUI();
        else
            requestSignin();
    }

    void requestSignin () {

        Social.localUser.Authenticate((bool success) => {

            if (success) {
                userSignedIn = true;
            } else {
                userSignedIn = false;
            }

        });

        /*if (!PlayGamesPlatform.Instance.HasPermission("email")) {
            PlayGamesPlatform.Instance.RequestPermission("email", result => {

                if (result == SignInStatus.Success) {
                    userSignedIn = true;
                } else if (result == SignInStatus.Canceled) {
                    userSignedIn = false;
                } else if (result == SignInStatus.Failed) {
                    userSignedIn = false;
                }

            });
        } else {
            userSignedIn = true;
        }*/
    }

}   

public class PlayServiceKey {

    //Leaderboard Key
    public static string leaderboard_ranking_free = "CgkIqsfiiMAOEAIQBg";
    public static string leaderboard_ranking_classic = "CgkIqsfiiMAOEAIQAA";

    //Achievements Key
    public static string achievement_reached_1 = "CgkIqsfiiMAOEAIQAQ";
    public static string achievement_reached_2 = "CgkIqsfiiMAOEAIQAg";
    public static string achievement_reached_3 = "CgkIqsfiiMAOEAIQAw";
    public static string achievement_reached_4 = "CgkIqsfiiMAOEAIQBA";
    public static string achievement_reached_5 = "CgkIqsfiiMAOEAIQBQ";

}
