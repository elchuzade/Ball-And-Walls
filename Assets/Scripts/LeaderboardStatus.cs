using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
// Bring classes from Server
using static Server;

public class LeaderboardStatus : MonoBehaviour
{
    // To send player data to server
    private class PlayerJson
    {
        public string playerId;
        public PlayerData playerData;
    }

    Player player;
    Navigator navigator;
    Server server;
    Scoreboard scoreboard;

    // This one should be based on indexes
    [SerializeField] Sprite[] ballSprites;
    // Single line of leadersboard
    [SerializeField] GameObject leaderboardItemPrefab;

    GameObject goldName;
    GameObject silverName;
    GameObject bronzeName;

    GameObject goldBall;
    GameObject silverBall;
    GameObject bronzeBall;

    // This is for saving the name before it has been changed, so receive diamonds
    GameObject changeNameGetDiamondsButton;
    // This is for saving the name after it has been changed
    GameObject changeNameSaveButton;
    // This is for closing the change name window by tapping anywhere outside
    GameObject changeNameCloseButton;
    // This is the window
    GameObject changeName;
    // Close the leaderboard window
    GameObject exitButton;
    GameObject changeNameCanvasButton;
    
    GameObject leaderboardScrollContent;

    GameObject leaderboardScrollbar;

    InputField nameInput;

    // To point at change name button
    GameObject arrow;

    // The object where the name typed into the field is held
    GameObject inputField;

    void Awake()
    {
        server = FindObjectOfType<Server>();
        navigator = FindObjectOfType<Navigator>();
        scoreboard = FindObjectOfType<Scoreboard>();

        arrow = GameObject.Find("Arrow");
        inputField = GameObject.Find("InputField");
        goldBall = GameObject.Find("GoldBall");
        silverBall = GameObject.Find("SilverBall");
        bronzeBall = GameObject.Find("BronzeBall");

        changeNameGetDiamondsButton = GameObject.Find("GetDiamondsButton");
        changeNameSaveButton = GameObject.Find("SaveButton");
        changeName = GameObject.Find("ChangeName");
        // Invisible button behind change name window
        changeNameCloseButton = GameObject.Find("CloseButton");
        leaderboardScrollContent = GameObject.Find("LeaderboardScrollContent");
        leaderboardScrollbar = GameObject.Find("LeaderboardScrollbar");
        goldName = GameObject.Find("GoldName");
        silverName = GameObject.Find("SilverName");
        bronzeName = GameObject.Find("BronzeName");
        exitButton = GameObject.Find("ExitButton");
        changeNameCanvasButton = GameObject.Find("EditButton");
        nameInput = inputField.GetComponent<InputField>();
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

        // Hide point arrow until server has replied
        arrow.SetActive(false);

        scoreboard.SetDiamonds(player.diamonds);

        SwapSaveButton();

        // Widen name input field and hide it
        changeName.transform.localScale = new Vector3(1, 1, 1);
        changeName.SetActive(false);

        SetButtonFunctions();
    }

    private void SwapSaveButton()
    {
        // Fetch data from server and choose the button to show
        if (player.nameChanged)
        {
            // The name sent from the server
            arrow.SetActive(false);
            changeNameSaveButton.SetActive(true);
            changeNameGetDiamondsButton.SetActive(false);
        }
        else
        {
            arrow.SetActive(true);
            changeNameSaveButton.SetActive(false);
            changeNameGetDiamondsButton.SetActive(true);
        }
    }

    private void SetButtonFunctions()
    {
        // Set edit button function and close edit window button
        changeNameCanvasButton.GetComponent<Button>().onClick.AddListener(() => ClickEditButton());
        changeNameCloseButton.GetComponent<Button>().onClick.AddListener(() => { });
        changeNameSaveButton.GetComponent<Button>().onClick.AddListener(() => { });
        changeNameGetDiamondsButton.GetComponent<Button>().onClick.AddListener(() => { });

        exitButton.GetComponent<Button>().onClick.AddListener(() => ClickExitButton());
    }

    private Sprite GetBallSprite(string ballName)
    {
        for (int i = 0; i < ballSprites.Length; i++)
        {
            if (ballSprites[i].name == ballName)
            {
                return ballSprites[i];
            }
        }

        return null;
    }

    private void CreateTrippleDotsEntry()
    {
        // Create tripple dots to separate different lists
        GameObject leaderboardItem = Instantiate(leaderboardItemPrefab, transform.position, Quaternion.identity);
        // Set its parent to be scroll content, for scroll functionality to work properly
        leaderboardItem.transform.SetParent(leaderboardScrollContent.transform);
        // Set its name component text mesh pro value to ...
        leaderboardItem.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = "...";
        // Set its rank component text mesh pro value to ...
        leaderboardItem.transform.Find("Rank").GetComponent<TextMeshProUGUI>().text = "...";
        // Set its ball icon based on data from server and indexes of balls
        leaderboardItem.transform.Find("Ball").gameObject.SetActive(false);
    }

    private void ShowYourEntryFrame(GameObject item)
    {
        // Show leaderboard frame for your entry
        item.transform.Find("Frame").gameObject.SetActive(true);
    }

    // Close Leadersboard Scene
    public void ClickExitButton()
    {
        navigator.LoadMainScene();
    }

    // Close Leadersboard Scene
    public void ClickEditButton()
    {
        changeName.SetActive(true);
    }
}
