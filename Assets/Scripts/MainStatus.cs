using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainStatus : MonoBehaviour
{
    GameObject hapticsButton;
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
        challengeButton = GameObject.Find("ChallengeButton");
        leaderboardButton = GameObject.Find("LeaderboardButton");

        tv = FindObjectOfType<TV>();
        tvLight = GameObject.Find("Light");
        tvSwitch = GameObject.Find("Switch");
    }

    void Start()
    {
        if ((float)Screen.width / Screen.height > 0.7)
        {
            GameObject canvas = GameObject.Find("Canvas");
            canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
        }

        player = FindObjectOfType<Player>();

        //player.ResetPlayer();

        //AdManager.ShowBanner();
        player.LoadPlayer();
        server.GetVideoLink();

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

    // Save data was successful
    public void SavePlayerDataSuccess()
    {
        Debug.Log("save player success");
    }

    // Save data was successful
    public void SavePlayerDataError()
    {
        Debug.Log("save player failed");
    }

    // Set video link from server file
    public void SetVideoLinkSuccess(string response)
    {
        SwitchOnLightOn();

        tv.SetAdLink(response);
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
}
