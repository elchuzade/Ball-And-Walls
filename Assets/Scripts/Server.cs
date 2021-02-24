﻿using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using static Utilities;

public class Server : MonoBehaviour
{
    /* CHALLENGE */

    public class Header
    {
        public string deviceId;
        public string deviceOS;
    }

    /* LEADERBOARD */

    // Change player name
    public class PlayerName
    {
        public string deviceId;
        public string playerName;
    }
    // Each row of leaderboard
    public class LeaderboardItem
    {
        public string playerName;
        public int rank;
        public string currentBall;
    }

    /* LOADING */

    // Create a new player
    private class PlayerJson
    {
        public string deviceOS;
        public string deviceId;
    }

    /* MAIN */

    // Video Link
    public class VideoJson
    {
        public string video;
        public string name;
        public string website;
    }

    public class PlayerData
    {
        public int coins;
        public int diamonds;
        public int keys;
        public int nextLevelIndex;
        public string currentBall;
        public List<string> unlockedBalls;
        public string deviceId;
    }

    // LOCAL TESTING
    string abboxAdsApi = "http://localhost:5002";
    string ballAndWallsApi = "http://localhost:5001";

    // STAGING
    //string abboxAdsApi = "https://staging.ads.abbox.com";
    //string ballAndWallsApi = "https://staging.ballandwalls.abboxgames.com";

    // PRODUCTION
    //string abboxAdsApi = "https://ads.abbox.com";
    //string ballAndWallsApi = "https://ballandwalls.abboxgames.com";

    List<LeaderboardItem> top = new List<LeaderboardItem>();
    List<LeaderboardItem> before = new List<LeaderboardItem>();
    List<LeaderboardItem> after = new List<LeaderboardItem>();
    LeaderboardItem you = new LeaderboardItem();

    // To send response to corresponding files
    [SerializeField] MainStatus mainStatus;
    // This is to call the functions in load scene
    [SerializeField] LoadStatus loadStatus;
    // This is to call the functions in leaderboard scene
    [SerializeField] LeaderboardStatus leaderboardStatus;

    Header header = new Header();

    void Awake()
    {
        header.deviceId = SystemInfo.deviceUniqueIdentifier;
        header.deviceOS = SystemInfo.operatingSystem;
    }

    /* ---------- LOAD SCENE ---------- */

    // CREATE NEW PLAYER
    public void CreatePlayer()
    {
        string playerUrl = ballAndWallsApi + "/api/v1/players/player";

        StartCoroutine(CreatePlayerCoroutine(playerUrl));
    }

    // This one is called when the game is just launched
    // Either create a new player or move on
    private IEnumerator CreatePlayerCoroutine(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            string message = JsonUtility.ToJson(header);
            string headerMessage = BuildHeaders(message);
            webRequest.SetRequestHeader("token", headerMessage);

            // Send request and wait for the desired response.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(webRequest.downloadHandler.text);
                // Set the error received from creating a player
                loadStatus.CreatePlayerError();
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                // Make the success actions received from creating a player
                loadStatus.CreatePlayerSuccess();
            }
        }
    }

    /* ---------- MAIN SCENE ---------- */

    // SAVE PLAYER DATA
    public void SavePlayerData(Player player)
    {
        string playerDataUrl = ballAndWallsApi + "/api/v1/players/data";

        PlayerData playerData = new PlayerData();
        playerData.coins = player.coins;
        playerData.diamonds = player.diamonds;
        playerData.keys = player.keys;
        playerData.nextLevelIndex = player.nextLevelIndex;
        playerData.currentBall = player.currentBall;
        playerData.deviceId = header.deviceId;
        playerData.unlockedBalls = new List<string>();

        player.unlockedBalls.ForEach(b =>
        {
            playerData.unlockedBalls.Add(b);
        });

        string playerDataJson = JsonUtility.ToJson(playerData);

        StartCoroutine(SavePlayerDataCoroutine(playerDataUrl, playerDataJson));
    }

    private IEnumerator SavePlayerDataCoroutine(string url, string playerData)
    {
        var jsonBinary = System.Text.Encoding.UTF8.GetBytes(playerData);
        DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();
        UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(jsonBinary);
        uploadHandlerRaw.contentType = "application/json";

        UnityWebRequest webRequest =
            new UnityWebRequest(url, "POST", downloadHandlerBuffer, uploadHandlerRaw);

        string message = JsonUtility.ToJson(header);
        string headerMessage = BuildHeaders(message);
        webRequest.SetRequestHeader("token", headerMessage);

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError)
        {
            Debug.Log(webRequest.downloadHandler.text);
            // Set the error received from creating a player
            mainStatus.SavePlayerDataError();
        }
        else
        {
            Debug.Log(webRequest.downloadHandler.text);
            // Make the success actions received from creating a player
            mainStatus.SavePlayerDataSuccess();
        }
    }

    public void GetVideoLink()
    {
        string videoUrl = abboxAdsApi + "/api/v1/videos";
        StartCoroutine(GetAdLinkCoroutine(videoUrl));
    }

    // This one is for TV in main scene
    // Get the latest video link, for now in general, in future personal based on the DeviceId
    private IEnumerator GetAdLinkCoroutine(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            string message = JsonUtility.ToJson(header);
            string headerMessage = BuildHeaders(message);
            webRequest.SetRequestHeader("token", headerMessage);

            // Send request and wait for the desired response.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(webRequest.downloadHandler.text);
                // Set the error of video link received from the server
                mainStatus.SetVideoLinkError(webRequest.error);
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                // Parse the response from server to retrieve all data fields
                VideoJson videoInfo = JsonUtility.FromJson<VideoJson>(webRequest.downloadHandler.text);

                // Set the video link received from the server
                mainStatus.SetVideoLinkSuccess(videoInfo.video);
            }
        }
    }

    /* ---------- LEADERBOARD SCENE ---------- */

    // CHANGE PLAYER NAME
    public void ChangePlayerName(string playerName)
    {
        string nameUrl = ballAndWallsApi + "/api/v1/players/name/";

        PlayerName nameObject = new PlayerName();
        nameObject.deviceId = header.deviceId;
        nameObject.playerName = playerName;

        string nameJson = JsonUtility.ToJson(nameObject);

        StartCoroutine(ChangeNameCoroutine(nameUrl, nameJson));
    }

    // CHANGE PLAYER NAME
    private IEnumerator ChangeNameCoroutine(string url, string playerName)
    {
        var jsonBinary = System.Text.Encoding.UTF8.GetBytes(playerName);
        DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();
        UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(jsonBinary);
        uploadHandlerRaw.contentType = "application/json";

        UnityWebRequest webRequest =
            new UnityWebRequest(url, "POST", downloadHandlerBuffer, uploadHandlerRaw);

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError)
        {
            Debug.Log(webRequest.downloadHandler.text);
            // Set the error received from creating a player
            leaderboardStatus.ChangeNameError();
        }
        else
        {
            Debug.Log(webRequest.downloadHandler.text);
            // Make the success actions received from creating a player
            leaderboardStatus.ChangeNameSuccess(webRequest.downloadHandler.text);
        }
    }


    // GET LEADERBOARD LIST
    public void GetLeaderboard()
    {
        string leaderboardUrl = ballAndWallsApi + "/api/v1/players/leaderboard/device";
        StartCoroutine(LeaderboardCoroutine(leaderboardUrl));
    }

    // Get leaderboard data and populate it into the scroll list
    private IEnumerator LeaderboardCoroutine(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            string message = JsonUtility.ToJson(header);
            string headerMessage = BuildHeaders(message);
            webRequest.SetRequestHeader("token", headerMessage);

            // Send request and wait for the desired response.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                // Set the error of leaderboard data received from the server
                Debug.Log(webRequest.downloadHandler.text);
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                // Parse the response from server to retrieve all data fields
                PopulateLeaderboardData(webRequest.downloadHandler.text);
            }
        }
    }

    private void PopulateLeaderboardData(string jsonData)
    {
        // Clear the lists incase they already had data in them
        top.Clear();
        before.Clear();
        after.Clear();
        // Extract string arrays of top, before, after and stirng of you data
        string[] topData = JsonHelper.GetJsonObjectArray(jsonData, "top");
        string[] beforeData = JsonHelper.GetJsonObjectArray(jsonData, "before");
        string youData = JsonHelper.GetJsonObject(jsonData, "you");
        string[] afterData = JsonHelper.GetJsonObjectArray(jsonData, "after");

        if (topData != null)
        {
            // Parse top data to leaderboard item to populate the list
            for (int i = 0; i < topData.Length; i++)
            {
                LeaderboardItem item = JsonUtility.FromJson<LeaderboardItem>(topData[i]);
                top.Add(item);
            }
        }

        if (beforeData != null)
        {
            // Parse before data
            for (int i = 0; i < beforeData.Length; i++)
            {
                LeaderboardItem item = JsonUtility.FromJson<LeaderboardItem>(beforeData[i]);
                before.Add(item);
            }
        }

        // Parse you data
        you = JsonUtility.FromJson<LeaderboardItem>(youData);

        if (afterData != null)
        {
            // Parse after data
            for (int i = 0; i < afterData.Length; i++)
            {
                LeaderboardItem item = JsonUtility.FromJson<LeaderboardItem>(afterData[i]);
                after.Add(item);
            }
        }

        // Send leaderboard data to leaderboard scene
        leaderboardStatus.SetLeaderboardData(top, before, you, after);
    }
}
