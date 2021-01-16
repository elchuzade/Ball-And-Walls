using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
//using System.Collections.Generic;

public class MainStatus : MonoBehaviour
{
    private GameObject hapticsButton;
    private GameObject soundsButton;
    private GameObject challengeButton;
    private GameObject leaderboardButton;
    private GameObject playButton;
    private GameObject shopButton;
    private TV tv;

    [SerializeField] Player player;
    private Navigator navigator;

    // To send player data to server
    private class PlayerJson
    {
        public string playerId;
        public PlayerData playerData;
    }

    void Awake()
    {
        tv = FindObjectOfType<TV>();
        navigator = FindObjectOfType<Navigator>();
        hapticsButton = GameObject.Find("HapticsButton");
        soundsButton = GameObject.Find("SoundsButton");
        challengeButton = GameObject.Find("ChallengeButton");
        leaderboardButton = GameObject.Find("LeaderboardButton");
        playButton = GameObject.Find("PlayButton");
        shopButton = GameObject.Find("ShopButton");
    }

    void Start()
    {
        player.LoadPlayer();

        // Set whether haptics and sound buttons are enabled or disabled initially
        SetButtonInitialState();

        // Send data stuff
        SendData();
    }

    // Set initial states of haptics and sounds buttons based on player prefs
    private void SetButtonInitialState()
    {
        if (PlayerPrefs.GetInt("Haptics") == 1)
        {
            hapticsButton.GetComponent<IconDisableButton>().SetButtonInitialState(ButtonStates.Enable);
        }
        else
        {
            hapticsButton.GetComponent<IconDisableButton>().SetButtonInitialState(ButtonStates.Disable);
        }
        if (PlayerPrefs.GetInt("Sounds") == 1)
        {
            soundsButton.GetComponent<IconDisableButton>().SetButtonInitialState(ButtonStates.Enable);
        }
        else
        {
            soundsButton.GetComponent<IconDisableButton>().SetButtonInitialState(ButtonStates.Disable);
        }
    }

    public void ClickHapticsButton()
    {
        if (PlayerPrefs.GetInt("Haptics") == 1)
        {
            // Run the click button animation, disable interactible feature until the end of animation
            // Set button state to disabled
            hapticsButton.GetComponent<IconDisableButton>().ClickButton(ButtonStates.Disable);
            // If haptics are turned on => turn them off
            PlayerPrefs.SetInt("Haptics", 0);
        }
        else
        {
            // Run the click button animation, disable interactible feature until the end of animation
            // Set button state to enabled
            hapticsButton.GetComponent<IconDisableButton>().ClickButton(ButtonStates.Enable);
            // If haptics are turned off => turn them on
            PlayerPrefs.SetInt("Haptics", 1);
        }
    }

    public void ClickSoundsButton()
    {
        if (PlayerPrefs.GetInt("Sounds") == 1)
        {
            // Run the click button animation, disable interactible feature until the end of animation
            // Set button state to disabled
            soundsButton.GetComponent<IconDisableButton>().ClickButton(ButtonStates.Disable);
            // If sounds are turned on => turn them off
            PlayerPrefs.SetInt("Sounds", 0);
        }
        else
        {
            // Run the click button animation, disable interactible feature until the end of animation
            // Set button state to enabled
            soundsButton.GetComponent<IconDisableButton>().ClickButton(ButtonStates.Enable);
            // If sounds are turned off => turn them on
            PlayerPrefs.SetInt("Sounds", 1);
        }
    }

    public void ClickChallengeButton()
    {
        // Play the animation of challenge button clikcing
        challengeButton.GetComponent<TriggerAnimation>().Trigger();
        // Disable button for the period of click animation, then enable again
        challengeButton.GetComponent<IconButton>().ClickButton();
        // Approximately when animation is over, load the challenge scene
        StartCoroutine(LoadChallengeSceneCoroutine(0.2f));
    }

    public void ClickLeaderboardButton()
    {
        // Play the animation of leaderboard button clikcing
        leaderboardButton.GetComponent<TriggerAnimation>().Trigger();
        // Disable button for the period of click animation, then enable again
        leaderboardButton.GetComponent<IconButton>().ClickButton();
        // Approximately when animation is over, load the leaderboard scene
        StartCoroutine(LoadLeaderboardSceneCoroutine(0.2f));
    }

    public void ClickPlayButton()
    {
        // Play the animation of play button clikcing
        playButton.GetComponent<TriggerAnimation>().Trigger();
        // Disable button for the period of click animation, then enable again
        playButton.GetComponent<IconButton>().ClickButton();
        // Approximately when animation is over, load the game scene
        StartCoroutine(LoadGameSceneCoroutine(0.2f));
    }

    public void ClickShopButton()
    {
        // Play the animation of play button clikcing
        shopButton.GetComponent<TriggerAnimation>().Trigger();
        // Disable button for the period of click animation, then enable again
        shopButton.GetComponent<IconButton>().ClickButton();
        // Approximately when animation is over, load the game scene
        StartCoroutine(LoadShopSceneCoroutine(0.2f));
    }

    public IEnumerator LoadGameSceneCoroutine(float time)
    {
        // Wait for given time and load the game scene
        yield return new WaitForSeconds(time);

        navigator.LoadNextLevel(player.nextLevelIndex);
    }

    public IEnumerator LoadShopSceneCoroutine(float time)
    {
        // Wait for given time and load the shop scene
        yield return new WaitForSeconds(time);

        navigator.LoadShop();
    }

    private IEnumerator LoadChallengeSceneCoroutine(float time)
    {
        // Wait for given time and load the challenge scene
        yield return new WaitForSeconds(time);

        navigator.LoadChallengeScene();
    }

    private IEnumerator LoadLeaderboardSceneCoroutine(float time)
    {
        // Wait for given time and load the leaderboard scene
        yield return new WaitForSeconds(time);

        navigator.LoadChallengeScene();
    }

    private void SendData()
    {
        PlayerData newPlayer = new PlayerData(player);

        //string url = "https://abboxgames.com/1/v1/save";

        //string url = "http://localhost:5001/1/v1/save";

        string adurl = "http://localhost:5001/1/v1/adlink";

        PlayerJson playerJson = new PlayerJson();
        playerJson.playerId = SystemInfo.deviceUniqueIdentifier;
        playerJson.playerData = newPlayer;

        string json = JsonUtility.ToJson(playerJson);

        //StartCoroutine(PostRequestCoroutine(url, json));

        StartCoroutine(GetAdLinkCoroutine(adurl));
    }

    private IEnumerator PostRequestCoroutine(string url, string json)
    {
        var jsonBinary = System.Text.Encoding.UTF8.GetBytes(json);

        DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();

        UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(jsonBinary);
        uploadHandlerRaw.contentType = "application/json";

        UnityWebRequest www =
            new UnityWebRequest(url, "POST", downloadHandlerBuffer, uploadHandlerRaw);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
            Debug.LogError(string.Format("{0}: {1}", www.url, www.error));
        else
            Debug.Log(string.Format("Response: {0}", www.downloadHandler.text));
    }

    IEnumerator GetAdLinkCoroutine(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                tv.SetAdLink(webRequest.downloadHandler.text);
                //Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
            }
        }
    }
}
