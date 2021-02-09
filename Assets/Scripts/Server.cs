using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Server : MonoBehaviour
{
    /* CHALLENGE */

    // Challenge data
    public class ChallengeWall
    {
        public string type;
        public Color32 color;
        public Vector3 position;
        public Vector3 rotation;
    }
    public class ChallengeBarrier
    {
        public string type;
        public Color32 color;
        public Vector3 position;
        public Vector3 rotation;
    }
    public class ChallengePortal
    {
        public string type;
        public Vector3 position;
        public Vector3 rotation;
    }
    public class ChallengeBallCatcher
    {
        public Vector3 position;
    }
    public class ChallengeBall
    {
        // East, West, North, South
        public string direction;
        public Vector3 position;
    }
    // Current challenge to play
    public class ChallengeLevel
    {
        public ChallengeBall ball;
        public ChallengeBallCatcher ballCatcher;
        public ChallengeWall[] walls;
        public ChallengeBarrier[] barriers;
        public ChallengePortal[] portals;
        public string background;
    }

    /* CHALLENGES */

    // Every challenge in the list including current challenge
    public class PastChallenge
    {
        public int tried;
        public int solved;
        public int coins;
        public int diamonds;
        public int lives;
        public string status;
        public string screenshot;
        public string id;
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
    string abboxAdsApi = "http://localhost:5001";
    string abboxMessengerApi = "http://localhost:5001";
    string ballAndWallsApi = "http://localhost:5001";

    // STAGING
    //private string abboxAdsApi = "https://staging.ads.abbox.com";
    //private string abboxMessengerApi = "https://staging.messenger.abbox.com";
    //private string ballAndWallsApi = "https://staging.ballandwalls.abboxgames.com";

    // PRODUCTION
    //private string abboxAdsApi = "https://ads.abbox.com";
    //private string abboxMessengerApi = "https://messenger.abbox.com";
    //private string ballAndWallsApi = "https://ballandwalls.abboxgames.com";

    List<ChallengeWall> walls = new List<ChallengeWall>();
    List<ChallengeBarrier> barriers = new List<ChallengeBarrier>();
    List<ChallengePortal> portals = new List<ChallengePortal>();
    ChallengeBall ball = new ChallengeBall();
    ChallengeBallCatcher ballCatcher = new ChallengeBallCatcher();

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
    // This is to call the functions in challenges scene
    [SerializeField] ChallengesStatus challengesStatus;
    // This is to call the functions in challenge scene
    [SerializeField] ChallengeStatus challengeStatus;

    List<PastChallenge> pastChallenges = new List<PastChallenge>();

    string deviceId;
    string deviceOS;

    void Awake()
    {
        deviceId = SystemInfo.deviceUniqueIdentifier;
        deviceOS = SystemInfo.operatingSystem;
    }

    /* ---------- CHALLENGES SCENE ---------- */

    // GET CHALLENGE DATA BY ID
    public void GetCurrentChallenge(string challengeId)
    {
        string currentChallengeUrl = ballAndWallsApi + "/api/v1/challenges/" + challengeId;
        StartCoroutine(GetCurrentChallengeCoroutine(currentChallengeUrl));
    }

    // This one is for the current challenge level
    private IEnumerator GetCurrentChallengeCoroutine(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Send request and wait for the desired reqsponse.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                // Set the error of video link received from the server
                challengeStatus.CurrentChallengeError();
            }
            else
            {
                // Parse the response from server to retrieve all data fields
                string levelData = JsonHelper.GetJsonObject(webRequest.downloadHandler.text, "data");

                PopulateChallengeData(levelData);
            }
        }
    }

    private void PopulateChallengeData(string jsonData)
    {
        // Clear the lists incase they already had data in them
        walls.Clear();
        barriers.Clear();
        portals.Clear();
        // Extract string arrays of top, before, after and stirng of you data
        string[] wallsData = JsonHelper.GetJsonObjectArray(jsonData, "walls");
        string[] barriersData = JsonHelper.GetJsonObjectArray(jsonData, "barriers");
        string[] portalsData = JsonHelper.GetJsonObjectArray(jsonData, "portals");
        string ballData = JsonHelper.GetJsonObject(jsonData, "ball");
        string ballCatcherData = JsonHelper.GetJsonObject(jsonData, "ballCatcher");

        // Set challenge level background
        //ChallengeLevel challengeLevel = JsonUtility.FromJson<ChallengeLevel>(jsonData);
        //StartCoroutine(ExtractCurrentChallengeBackgroundImage(challengeLevel));

        // Parse walls data
        for (int i = 0; i < wallsData.Length; i++)
        {
            ChallengeWall wall = JsonUtility.FromJson<ChallengeWall>(wallsData[i]);
            walls.Add(wall);
        }
        // Parse barriers data
        for (int i = 0; i < barriersData.Length; i++)
        {
            ChallengeBarrier barrier = JsonUtility.FromJson<ChallengeBarrier>(barriersData[i]);
            barriers.Add(barrier);
        }
        // Parse ball data
        ball = JsonUtility.FromJson<ChallengeBall>(ballData);
        // Parse ball catcher data
        ballCatcher = JsonUtility.FromJson<ChallengeBallCatcher>(ballCatcherData);
        // Parse after data
        for (int i = 0; i < portalsData.Length; i++)
        {
            ChallengePortal portal = JsonUtility.FromJson<ChallengePortal>(portalsData[i]);
            portals.Add(portal);
        }
        // Send leaderboard data to leaderboard scene
        challengeStatus.CurrentChallengeSuccess(walls, barriers, portals, ball, ballCatcher);
    }

    private IEnumerator ExtractCurrentChallengeBackgroundImage(ChallengeLevel challengeLevel)
    {
        if (challengeLevel.background != null)
        {
            WWW www = new WWW(challengeLevel.background);

            yield return www;
            if (www.texture != null)
            {
                Sprite sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));

                challengesStatus.SetScreenshotImage(sprite);
            }
        }
    }

    // GET PAST CHALLENGES
    public void GetPastChallenges()
    {
        string pastChallengesUrl = ballAndWallsApi + "/api/v1/challenges/past/" + deviceId;
        StartCoroutine(GetPastChallengesCoroutine(pastChallengesUrl));
    }

    // This one is for the big challenge in challenges scene
    // This does not have data to build up a challenge level
    // Only teasing stuff, like screenshot, rewards, try-solve, background
    private IEnumerator GetPastChallengesCoroutine(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Send request and wait for the desired reqsponse.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(webRequest.downloadHandler.text);
                // Set the error of video link received from the server
                challengesStatus.PastChallengesError();
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                // Parse the response from server to retrieve all data fields
                string[] levelsData = JsonHelper.GetJsonObjectArray(webRequest.downloadHandler.text, "data");

                if (levelsData != null)
                {
                    // Parse top data to leaderboard item to populate the list
                    for (int i = 0; i < levelsData.Length; i++)
                    {
                        PastChallenge pastChallengeLevel = JsonUtility.FromJson<PastChallenge>(levelsData[i]);
                        pastChallenges.Add(pastChallengeLevel);
                    }
                }

                // Set all past challenges
                challengesStatus.PastChallengesSuccess(pastChallenges);
                // Set first past challenge as current challenge since it is the latest
                challengesStatus.SetSelectedChallenge(pastChallenges[0]);
                // Extract screenshot of first past challenge
                StartCoroutine(ExtractPastChallengeScreenshotImage(pastChallenges[0]));
            }
        }
    }

    public IEnumerator ExtractPastChallengeScreenshotImage(PastChallenge challengeLevel)
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

    // CREATE NEW PLAYER
    public void CreatePlayer()
    {
        string playerUrl = ballAndWallsApi + "/api/v1/players/player";

        PlayerJson playerObject = new PlayerJson();
        playerObject.deviceId = deviceId;
        playerObject.deviceOS = deviceOS;

        string playerJson = JsonUtility.ToJson(playerObject);

        StartCoroutine(CreatePlayerCoroutine(playerUrl, playerJson));
    }

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
        playerData.deviceId = deviceId;
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

    public void GetVideoLink()
    {
        string videoUrl = "https://ads.abbox.com" + "/api/v1/videos";
        StartCoroutine(GetAdLinkCoroutine(videoUrl));
    }

    /* ---------- LEADERBOARD SCENE ---------- */

    // CHANGE PLAYER NAME
    public void ChangePlayerName(string playerName)
    {
        string nameUrl = ballAndWallsApi + "/api/v1/players/name/";

        PlayerName nameObject = new PlayerName();
        nameObject.deviceId = deviceId;
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
        string leaderboardUrl = ballAndWallsApi + "/api/v1/players/leaderboard/" + deviceId;
        StartCoroutine(LeaderboardCoroutine(leaderboardUrl));
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
