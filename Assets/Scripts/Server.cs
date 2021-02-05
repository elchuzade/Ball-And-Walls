using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Server : MonoBehaviour
{
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

    private List<LeaderboardItem> top = new List<LeaderboardItem>();
    private List<LeaderboardItem> before = new List<LeaderboardItem>();
    private List<LeaderboardItem> after = new List<LeaderboardItem>();
    private LeaderboardItem you = new LeaderboardItem();

    // To send player data to server
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

    // To send response to corresponding files
    [SerializeField] MainStatus mainStatus;
    // This is to call the functions in load scene
    [SerializeField] LoadStatus loadStatus;
    // This is to call the functions in leaderboard scene
    [SerializeField] LeaderboardStatus leaderboardStatus;

    void Awake()
    {
    }

    void Start()
    {
        // Send data stuff
        //SendData();
    }

    void Update()
    {

    }

    private void SendData()
    {
        //PlayerData newPlayer = new PlayerData(player);

        //string url = "https://abboxgames.com/1/v1/save";

        //string url = "http://localhost:5001/v1/player";

        //PlayerJson playerJson = new PlayerJson();
        //playerJson.playerId = SystemInfo.deviceUniqueIdentifier;
        //playerJson.playerData = newPlayer;

        //string json = JsonUtility.ToJson(playerJson);

        //StartCoroutine(PostRequestCoroutine(url, json));
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

    public void GetVideoLink()
    {
        string videoUrl = "http://localhost:5001/api/v1/videos";
        StartCoroutine(GetAdLinkCoroutine(videoUrl));
    }

    public void CreatePlayer()
    {
        string playerUrl = "http://localhost:5001/api/v1/players/player";

        PlayerJson playerObject = new PlayerJson();
        playerObject.deviceId = SystemInfo.deviceUniqueIdentifier;
        playerObject.deviceOS = SystemInfo.operatingSystem;

        string playerJson = JsonUtility.ToJson(playerObject);

        Debug.Log(playerUrl);
        Debug.Log(playerJson);

        StartCoroutine(CreatePlayerCoroutine(playerUrl, playerJson));
    }

    private IEnumerator PostRequestCoroutine(string url, string json)
    {
        var jsonBinary = System.Text.Encoding.UTF8.GetBytes(json);
        DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();
        UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(jsonBinary);
        uploadHandlerRaw.contentType = "application/json";

        UnityWebRequest www =
            new UnityWebRequest(url, "POST", downloadHandlerBuffer, uploadHandlerRaw);

        //www.SetRequestHeader("", "");

        yield return www.SendWebRequest();

        if (www.isNetworkError)
            Debug.LogError(string.Format("{0}: {1}", www.url, www.error));
        else
            Debug.Log(string.Format("Response: {0}", www.downloadHandler.text));
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
            loadStatus.CreatePlayerError(webRequest.error);
        } 
        else
        {
            // Make the success actions received from creating a player
            loadStatus.CreatePlayerSuccess(webRequest.downloadHandler.text);
        }
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
