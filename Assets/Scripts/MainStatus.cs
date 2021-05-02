using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using static Server;

public class MainStatus : MonoBehaviour
{
    [SerializeField] GameObject privacyWindow;
    [SerializeField] GameObject quitWindow;
    GameObject hapticsButton;
    GameObject soundsButton;
    GameObject challengeButton;
    GameObject leaderboardButton;

    Player player;
    Navigator navigator;
    Scoreboard scoreboard;

    TV tv;
    GameObject tvLight;
    GameObject tvSwitch;
    bool switchIsOn;

    float switchTurnSpeed = 2;

    Server server;

    void Awake()
    {
        scoreboard = FindObjectOfType<Scoreboard>();
        server = FindObjectOfType<Server>();
        navigator = FindObjectOfType<Navigator>();

        hapticsButton = GameObject.Find("HapticsButton");
        soundsButton = GameObject.Find("SoundsButton");
        challengeButton = GameObject.Find("ChallengeButton");
        leaderboardButton = GameObject.Find("LeaderboardButton");

        tv = FindObjectOfType<TV>();
        tvLight = GameObject.Find("Light");
        tvSwitch = GameObject.Find("Switch");

        privacyWindow.transform.localScale = new Vector3(1, 1, 1);
        quitWindow.transform.localScale = new Vector3(1, 1, 1);
        quitWindow.SetActive(false);
    }

    void Start()
    {
        if ((float)Screen.width / Screen.height > 0.7)
        {
            GameObject canvas = GameObject.Find("Canvas");
            canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
        }

        player = FindObjectOfType<Player>();
        player.LoadPlayer();

        if (player.privacyPolicyAccepted)
        {
            privacyWindow.SetActive(false);
            leaderboardButton.GetComponent<Button>().onClick.AddListener(() => ClickLeaderboardButton());
            
            if (!player.playerCreated)
            {
                server.CreatePlayer(player);
            } else
            {
                server.SavePlayerData(player);
            }
        }
        else
        {
            leaderboardButton.GetComponent<Button>().onClick.AddListener(() => ShowPrivacyPolicy());
            leaderboardButton.transform.Find("Components").Find("Frame").GetComponent<Image>().color = new Color32(255, 197, 158, 100);
            leaderboardButton.transform.Find("Components").Find("Icon").GetComponent<Image>().color = new Color32(255, 255, 255, 100);
        }

        if (player.privacyPolicyDeclined)
        {
            // First time entering the game
            privacyWindow.SetActive(false);
        }

        AdManager.ShowBanner();
        server.GetVideoLink(player.privacyPolicyAccepted);

        // Set whether haptics and sound buttons are enabled or disabled initially
        SetButtonInitialState();

        scoreboard.SetCoins(player.coins);
        scoreboard.SetDiamonds(player.diamonds);
    }

    void Update()
    {
        // -0.6 is off position, 0 is on position
        if (switchIsOn && tvSwitch.transform.rotation.z > -0.6)
        {
            // Turn switch clockwise
            tvSwitch.transform.Rotate(new Vector3(0, 0, -switchTurnSpeed));
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            quitWindow.SetActive(true);
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

    // Set video link from server file
    public void SetVideoLinkSuccess(VideoJson response)
    {
        SwitchOnLightOn();

        tv.SetAdLink(response.video);
        tv.SetAdButton(response.website);

        tv.transform.Find("ScreenAnimation").gameObject.SetActive(false);
    }

    // Set error actions of video link from server file
    public void SetVideoLinkError(string response)
    {
        SwitchOffLightOff();
    }

    // Set initial states of haptics button based on player prefs
    private void SetButtonInitialState()
    {
        if (PlayerPrefs.GetInt("Haptics") == 1)
        {
            hapticsButton.transform.Find("Disabled").gameObject.SetActive(false);
        }
        else
        {
            hapticsButton.transform.Find("Disabled").gameObject.SetActive(true);
        }
        if (PlayerPrefs.GetInt("Sounds") == 1)
        {
            soundsButton.transform.Find("Disabled").gameObject.SetActive(false);
        }
        else
        {
            soundsButton.transform.Find("Disabled").gameObject.SetActive(true);
        }
    }

    public void ClickHapticsButton()
    {
        if (PlayerPrefs.GetInt("Haptics") == 1)
        {
            // Set button state to disabled
            hapticsButton.transform.Find("Disabled").gameObject.SetActive(true);
            // If haptics are turned on => turn them off
            PlayerPrefs.SetInt("Haptics", 0);
        }
        else
        {
            // Set button state to enabled
            hapticsButton.transform.Find("Disabled").gameObject.SetActive(false);
            // If haptics are turned off => turn them on
            PlayerPrefs.SetInt("Haptics", 1);
        }
    }

    public void ClickSoundsButton()
    {
        if (PlayerPrefs.GetInt("Sounds") == 1)
        {
            // Set button state to disabled
            soundsButton.transform.Find("Disabled").gameObject.SetActive(true);
            // If sounds are turned on => turn them off
            PlayerPrefs.SetInt("Sounds", 0);
        }
        else
        {
            // Set button state to enabled
            soundsButton.transform.Find("Disabled").gameObject.SetActive(false);
            // If sounds are turned off => turn them on
            PlayerPrefs.SetInt("Sounds", 1);
        }
    }

    public void ClickPlayButton()
    {
        navigator.LoadNextLevel(player.nextLevelIndex);
    }

    public void ClickShopButton()
    {
        navigator.LoadShop();
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

    private IEnumerator LoadChallengeSceneCoroutine(float time)
    {
        // Wait for given time and load the challenge scene
        yield return new WaitForSeconds(time);

        navigator.LoadChallengesScene();
    }

    private IEnumerator LoadLeaderboardSceneCoroutine(float time)
    {
        // Wait for given time and load the leaderboard scene
        yield return new WaitForSeconds(time);

        navigator.LoadLeaderboardScene();
    }

    public void ClickTermsOfUse()
    {
        Application.OpenURL("https://abboxgames.com/terms-of-use");
    }

    public void ClickPrivacyPolicy()
    {
        Application.OpenURL("https://abboxgames.com/privacy-policy");
    }

    public void AcceptPrivacy()
    {
        leaderboardButton.transform.Find("Components").Find("Frame").GetComponent<Image>().color = new Color32(255, 197, 158, 255);
        leaderboardButton.transform.Find("Components").Find("Icon").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        leaderboardButton.GetComponent<Button>().onClick.AddListener(() => ClickLeaderboardButton());

        privacyWindow.transform.localScale = new Vector3(0, 1, 1);
        privacyWindow.SetActive(false);
        player.privacyPolicyDeclined = false;
        player.privacyPolicyAccepted = true;
        player.SavePlayer();

        server.CreatePlayer(player);
    }

    public void ShowPrivacyPolicy()
    {
        privacyWindow.SetActive(true);
    }

    public void DeclinePrivacy()
    {
        leaderboardButton.GetComponent<Button>().onClick.AddListener(() => privacyWindow.SetActive(true));
        privacyWindow.SetActive(false);
        player.privacyPolicyDeclined = true;
        player.SavePlayer();
    }

    // Create a new player
    public void CreatePlayerSuccess()
    {
        player.playerCreated = true;
        player.SavePlayer();
        server.SavePlayerData(player);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ContinuePlaying()
    {
        // If you opened quit game window by accident
        quitWindow.SetActive(false);
    }
}
