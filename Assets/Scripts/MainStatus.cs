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


    Player player;
    Navigator navigator;

    void Awake()
    {
        //player = FindObjectOfType<Player>();
        //navigator = FindObjectOfType<Navigator>();

        hapticsButton = GameObject.Find("HapticsButton");
        soundsButton = GameObject.Find("SoundsButton");
        challengeButton = GameObject.Find("ChallengeButton");
        leaderboardButton = GameObject.Find("LeaderboardButton");
        playButton = GameObject.Find("PlayButton");
        shopButton = GameObject.Find("ShopButton");
    }

    void Start()
    {
        AdManager.ShowBanner();

        player.LoadPlayer();

        // Set whether haptics and sound buttons are enabled or disabled initially
        SetButtonInitialState();
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

    public void ClickShopButton()
    {
        // If shop button is interactable, run the click animation and load the shop scene
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

        navigator.LoadLeaderboardScene();
    }
}
