using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Server : MonoBehaviour
{
    public class ChallengeLevelMini
    {
        public int tried;
        public int solved;
        public int coins;
        public int diamonds;
        public string screenshot;
        public string background;
    }

    public class PlayerName
    {
        public string deviceId;
        public string name;
    }

    public class LeaderboardItem
    {
        public string name;
        public int rank;
        public string currentBall;
    }

    private class PlayerJson
    {
        public string deviceOS;
        public string deviceId;
    }

    public class VideoJson
    {
        public string video;
        public string name;
        public string website;
    }

    private List<LeaderboardItem> top = new List<LeaderboardItem>();
    private List<LeaderboardItem> before = new List<LeaderboardItem>();
    private List<LeaderboardItem> after = new List<LeaderboardItem>();
    private LeaderboardItem you = new LeaderboardItem();

    // To send response to corresponding files
    [SerializeField] MainStatus mainStatus;
    // This is to call the functions in load scene
    [SerializeField] LoadStatus loadStatus;
    // This is to call the functions in leaderboard scene
    [SerializeField] LeaderboardStatus leaderboardStatus;
    // This is to call the functions in challenges scene
    [SerializeField] ChallengesStatus challengesStatus;

    /* ---------- CHALLENGES SCENE ---------- */

    // This one is for the big challenge in challenges scene
    // This does not have data to build up a challenge level
    // Only teasing stuff, like screenshot, rewards, try-solve, background
    private IEnumerator GetLatestChallengeMiniCoroutine(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Send request and wait for the desired reqsponse.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                // Set the error of video link received from the server
                challengesStatus.LatestChallengeMiniError();
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                // Parse the response from server to retrieve all data fields
                string levelData = JsonHelper.GetJsonObject(webRequest.downloadHandler.text, "data");

                ChallengeLevelMini challengeLevel = JsonUtility.FromJson<ChallengeLevelMini>(levelData);

                challengesStatus.LatestChallengeMiniSuccess(challengeLevel);

                StartCoroutine(ExtractLatestChallengeMiniScreenshotImage(challengeLevel));
            }
        }
    }

    public void GetLatestChallengeMini()
    {
        string latestChallengeMiniUrl = "http://localhost:5001/api/v1/challenges/latest-mini";
        StartCoroutine(GetLatestChallengeMiniCoroutine(latestChallengeMiniUrl));
    }

    private IEnumerator ExtractLatestChallengeMiniScreenshotImage(ChallengeLevelMini challengeLevel)
    {
        if (challengeLevel.screenshot != null)
        {
            WWW www = new WWW(challengeLevel.screenshot);

            yield return www;
            if (www.texture != null)
            {
                Sprite sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));

                challengesStatus.SetScreenshotImage(sprite);
            }
        }
    }

    /* ---------- LOAD SCENE ---------- */

    // This one is called when the game is just launched
    // Either create a new player or move on
    private IEnumerator CreatePlayerCoroutine(string url, string player)
    {
        var jsonBinary = System.Text.Encoding.UTF8.GetBytes(player);
        DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();
        UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(jsonBinary);
        uploadHandlerRaw.contentType = "application/json";

        UnityWebRequest webRequest =
            new UnityWebRequest(url, "POST", downloadHandlerBuffer, uploadHandlerRaw);

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError)
        {
            // Set the error received from creating a player
            loadStatus.CreatePlayerError();
        } 
        else
        {
            // Make the success actions received from creating a player
            loadStatus.CreatePlayerSuccess();
        }
    }

    public void CreatePlayer()
    {
        string playerUrl = "http://localhost:5001/api/v1/players/player";

        PlayerJson playerObject = new PlayerJson();
        playerObject.deviceId = SystemInfo.deviceUniqueIdentifier;
        playerObject.deviceOS = SystemInfo.operatingSystem;

        string playerJson = JsonUtility.ToJson(playerObject);

        StartCoroutine(CreatePlayerCoroutine(playerUrl, playerJson));
    }

    /* ---------- MAIN SCENE ---------- */

    // This one is for TV in main scene
    // Get the latest video link, for now in general, in future personal based on the DeviceId
    private IEnumerator GetAdLinkCoroutine(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Send request and wait for the desired reqsponse.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
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

    public void GetVideoLink()
    {
        string videoUrl = "http://localhost:5001/api/v1/videos";
        StartCoroutine(GetAdLinkCoroutine(videoUrl));
    }

    /* ---------- LEADERBOARD SCENE ---------- */

    // Change player name
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
            Debug.Log(webRequest.error);
            // Set the error received from creating a player
            leaderboardStatus.ChangeNameError();
        }
        else
        {
            Debug.Log(webRequest.downloadHandler.text);
            // Make the success actions received from creating a player
            leaderboardStatus.ChangeNameSuccess();
        }
    }

    // Get leaderboard data and populate it into the scroll list
    private IEnumerator LeaderboardCoroutine(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Send request and wait for the desired reqsponse.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                // Set the error of leaderboard data received from the server
            }
            else
            {
                // Parse the response from server to retrieve all data fields
                PopulateData(webRequest.downloadHandler.text);
            }
        }
    }

    public void ChangePlayerName(string name)
    {
        string id = SystemInfo.deviceUniqueIdentifier;

        string nameUrl = "http://localhost:5001/api/v1/players/name/";

        PlayerName nameObject = new PlayerName();
        nameObject.deviceId = id;
        nameObject.name = name;

        string nameJson = JsonUtility.ToJson(nameObject);

        StartCoroutine(ChangeNameCoroutine(nameUrl, nameJson));
    }

    public void GetLeaderboard()
    {
        string id = SystemInfo.deviceUniqueIdentifier;

        string leaderboardUrl = "http://localhost:5001/api/v1/players/leaderboard/" + id;
        StartCoroutine(LeaderboardCoroutine(leaderboardUrl));
    }

    private void PopulateData(string jsonData)
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

        // Parse top data to leaderboard item to populate the list
        for (int i = 0; i < topData.Length; i++)
        {
            LeaderboardItem item = JsonUtility.FromJson<LeaderboardItem>(topData[i]);
            top.Add(item);
        }
        // Parse before data
        for (int i = 0; i < beforeData.Length; i++)
        {
            LeaderboardItem item = JsonUtility.FromJson<LeaderboardItem>(beforeData[i]);
            before.Add(item);
        }
        // Parse you data
        you = JsonUtility.FromJson<LeaderboardItem>(youData);
        // Parse after data
        for (int i = 0; i < afterData.Length; i++)
        {
            LeaderboardItem item = JsonUtility.FromJson<LeaderboardItem>(afterData[i]);
            after.Add(item);
        }
        // Send leaderboard data to leaderboard scene
        leaderboardStatus.SetLeaderboardData(top, before, you, after);
    }
}
