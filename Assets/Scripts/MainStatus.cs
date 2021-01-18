using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class MainStatus : MonoBehaviour
{
    GameObject hapticsButton;
    GameObject soundsButton;
    GameObject challengeButton;
    GameObject leaderboardButton;
    GameObject playButton;
    GameObject shopButton;
    TV tv;
    GameObject tvLight;
    GameObject tvSwitch;
    bool switchIsOn;

    float switchTurnSpeed = 2;

    [SerializeField] Player player;
    Navigator navigator;

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
        tvLight = GameObject.Find("Light");
        tvSwitch = GameObject.Find("Switch");
    }

    void Start()
    {
        player.LoadPlayer();

        // Set whether haptics and sound buttons are enabled or disabled initially
        SetButtonInitialState();

        // Send data stuff
        SendData();
    }

    void Update()
    {
        // -0.6 is off position, 0 is on position
        if (switchIsOn && tvSwitch.transform.rotation.z > -0.6)
        {
            // Turn switch clockwise
            tvSwitch.transform.Rotate(new Vector3(0, 0, -switchTurnSpeed));
        }
    }

    // Turn on the light and rotate the switch
    private void SwitchOnLightOn()
    {
        // Run the switch on animation, if switch is at off state
        if (!switchIsOn)
        {
            switchIsOn = true;
        }
        // Turn lights on to green
        tvLight.GetComponent<Image>().color = new Color32(0, 255, 0, 255);
    }

    // Turn on the light and rotate the switch
    private void SwitchOffLightOff()
    {
        // Turn switch to off state
        switchIsOn = false;
        // Turn lights on to red
        tvLight.GetComponent<Image>().color = new Color32(255, 0, 0, 255);
    }

    // Set initial states of haptics and sounds buttons based on player prefs
    private void SetButtonInitialState()
    {
        if (PlayerPrefs.GetInt("Haptics") == 1)
        {
            hapticsButton.GetComponent<TriggerButton>().SetButtonState(ButtonStates.Enable);
        }
        else
        {
            hapticsButton.GetComponent<TriggerButton>().SetButtonState(ButtonStates.Disable);
        }
        if (PlayerPrefs.GetInt("Sounds") == 1)
        {
            soundsButton.GetComponent<TriggerButton>().SetButtonState(ButtonStates.Enable);
        }
        else
        {
            soundsButton.GetComponent<TriggerButton>().SetButtonState(ButtonStates.Disable);
        }
    }

    public void ClickHapticsButton()
    {
        if (hapticsButton.GetComponent<Button>().IsInteractable())
        {
            if (PlayerPrefs.GetInt("Haptics") == 1)
            {
                // Run the click button animation
                hapticsButton.GetComponent<TriggerButton>().ClickButton(0.2f);
                // Set button state to disabled
                hapticsButton.GetComponent<TriggerButton>().SetButtonState(ButtonStates.Disable);
                // If haptics are turned on => turn them off
                PlayerPrefs.SetInt("Haptics", 0);
            }
            else
            {
                // Run the click button animation
                hapticsButton.GetComponent<TriggerButton>().ClickButton(0.2f);
                // Set button state to enabled
                hapticsButton.GetComponent<TriggerButton>().SetButtonState(ButtonStates.Enable);
                // If haptics are turned off => turn them on
                PlayerPrefs.SetInt("Haptics", 1);
            }
        }
    }

    public void ClickSoundsButton()
    {
        if (soundsButton.GetComponent<Button>().IsInteractable())
        {
            if (PlayerPrefs.GetInt("Sounds") == 1)
            {
                // Run the click button animation
                soundsButton.GetComponent<TriggerButton>().ClickButton(0.2f);
                // Set button state to disabled
                soundsButton.GetComponent<TriggerButton>().SetButtonState(ButtonStates.Disable);
                // If sounds are turned on => turn them off
                PlayerPrefs.SetInt("Sounds", 0);
            }
            else
            {
                // Run the click button animation
                soundsButton.GetComponent<TriggerButton>().ClickButton(0.2f);
                // Set button state to enabled
                soundsButton.GetComponent<TriggerButton>().SetButtonState(ButtonStates.Enable);
                // If sounds are turned off => turn them on
                PlayerPrefs.SetInt("Sounds", 1);
            }
        }
    }

    public void ClickChallengeButton()
    {
        if (challengeButton.GetComponent<Button>().IsInteractable())
        {
            // Run the trigger button animation and disable button for its duration
            challengeButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            // Approximately when animation is over, load the challenge scene
            StartCoroutine(LoadChallengeSceneCoroutine(0.2f));
        }
    }

    public void ClickLeaderboardButton()
    {
        if (leaderboardButton.GetComponent<Button>().IsInteractable())
        {
            // Run the trigger button animation and disable button for its duration
            leaderboardButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            // Approximately when animation is over, load the leaderboard scene
            StartCoroutine(LoadLeaderboardSceneCoroutine(0.2f));
        }
    }

    public void ClickPlayButton()
    {
        if (playButton.GetComponent<Button>().IsInteractable())
        {
            // Run the trigger button animation and disable button for its duration
            playButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            // Approximately when animation is over, load the game scene
            StartCoroutine(LoadGameSceneCoroutine(0.2f));
        }
    }

    // If shop button is interactable, run the click animation and load the shop scene
    public void ClickShopButton()
    {
        if (shopButton.GetComponent<Button>().IsInteractable())
        {
            // Disable button for the period of click animation, then enable again
            shopButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            // Approximately when animation is over, load the game scene
            StartCoroutine(LoadShopSceneCoroutine(0.2f));
        }
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
        yield return new WaitForSeconds(3);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                SwitchOffLightOff();
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                SwitchOnLightOn();
                Debug.Log(webRequest.downloadHandler.text);
                tv.SetAdLink(webRequest.downloadHandler.text);
                tv.transform.Find("ScreenAnimation").gameObject.SetActive(false);
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
            }
        }
    }
}
