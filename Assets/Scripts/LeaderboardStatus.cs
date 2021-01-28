using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class LeaderboardStatus : MonoBehaviour
{
    Player player;
    Navigator navigator;

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

    // To point at change name button
    GameObject arrow;

    // The object where the name typed into the field is held
    GameObject inputField;

    // This is to see if the diamonds should be given, fetch this from server
    bool nameIsChanged;

    // Name of the player to change when typing, this will be changed to name if the server sends name changed bool true
    string playerName;

    // To send player data to server
    private class PlayerJson
    {
        public string playerId;
        public PlayerData playerData;
    }

    private (string name, int rank, int ballIndex)[] topMock = {
        ("Player-100001", 1, 3),
        ("Player-100002", 2, 1),
        ("Player-100003", 3, 6),
        ("Player-100004", 4, 4),
        ("Player-100005", 5, 3),
        ("Player-100006", 6, 7),
        ("Player-100007", 7, 25),
        ("Player-100008", 8, 23),
        ("Player-100009", 9, 31),
        ("Player-100010", 10, 19)
    };

    private (string name, int rank, int ballIndex)[] beforeMock = {
        ("Player-100188", 188, 30),
        ("Player-100189", 189, 34),
        ("Player-100190", 190, 15)
    };

    private (string name, int rank, int ballIndex)[] afterMock = {
        ("Player-100192", 192, 21),
        ("Player-100193", 193, 22),
        ("Player-100194", 194, 29)
    };

    private (string name, int rank, int ballIndex) youMock = ("Kamran", 191, 33);

    private List<(string name, int rank, int ballIndex)> before = new List<(string name, int rank, int ballIndex)>();
    private List<(string name, int rank, int ballIndex)> after = new List<(string name, int rank, int ballIndex)>();
    private List<(string name, int rank, int ballIndex)> top = new List<(string name, int rank, int ballIndex)>();
    private (string name, int rank, int ballIndex) you;

    void Awake()
    {
        //player = FindObjectOfType<Player>();
        //navigator = FindObjectOfType<Navigator>();

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
    }

    void Start()
    {
        // Hide point arrow until server has replied
        arrow.SetActive(false);

        // Fetch data from server and choose the button to show
        if (nameIsChanged)
        {
            // The name sent from the server
            playerName = "BlaBla";
            changeNameGetDiamondsButton.SetActive(false);
        } else
        {
            arrow.SetActive(true);
            changeNameSaveButton.SetActive(false);
        }
        var input = inputField.GetComponent<InputField>();
        input.onEndEdit.AddListener(SubmitName);  // This also works

        // Widen name input field and hide it
        changeName.transform.localScale = new Vector3(1, 1, 1);
        changeName.SetActive(false);

        SetButtonFunctions();

        PopulateMockData();
        BuildUpList();
    }

    private void SetButtonFunctions()
    {
        // Set edit button function and close edit window button
        changeNameCanvasButton.GetComponent<Button>().onClick.AddListener(() => ClickChangeNameButton());
        changeNameCloseButton.GetComponent<Button>().onClick.AddListener(() => CloseChangeName());
        changeNameSaveButton.GetComponent<Button>().onClick.AddListener(() => ClickSaveName());
        changeNameGetDiamondsButton.GetComponent<Button>().onClick.AddListener(() => ClickGetDiamonds());

        exitButton.GetComponent<Button>().onClick.AddListener(() => ClickExitButton());
    }

    private void ScrollListToPlayer()
    {
        // Combine all the values from all three lists top, before after
        int total = top.Count + before.Count + after.Count;
        // Increase by one for your rank if it is outside of top ten
        if (you.rank != 0)
        {
            total++;
        }
        // baed on total find where to place the scroll
        if (total > 10)
        {
            // If total is greater than 10, it is safe to show the bottom 5 players to make sure you are also visible
            leaderboardScrollbar.GetComponent<Scrollbar>().value = 0.001f;
        }
        else
        {
            // If you are in the top ten, then if you are in top 5, show first 5 players to make sure you are also visible
            if (you.rank < 6)
            {
                leaderboardScrollbar.GetComponent<Scrollbar>().value = 0.999f;
            } else
            {
                // Otherwise your rank is in the range of 5-10, so it is safe to show middle 5 players to make sure you are also visible
                leaderboardScrollbar.GetComponent<Scrollbar>().value = 0.5f;
            }
        }
    }

    private void PopulateMockData()
    {
        // Loop though top ten list provided by the server and add them to local list
        for (int i = 0; i < topMock.Length; i++)
        {
            top.Add(topMock[i]);
        }
        // Loop though up to 3 players before you list provided by the server
        // and if they have not yet been added to the list add them
        for (int i = 0; i < beforeMock.Length; i++)
        {
            if (!CheckIfExists(beforeMock[i].rank))
            {
                before.Add(beforeMock[i]);
            }
        }
        // Check if your rank has already been added to the list if not add it
        if (!CheckIfExists(youMock.rank))
        {
            you = youMock;
        }
        // Loop though up to 3 players after you list provided by the server
        // and if they have not yet been added to the list add them
        for (int i = 0; i < afterMock.Length; i++)
        {
            if (!CheckIfExists(afterMock[i].rank))
            {
                after.Add(afterMock[i]);
            }
        }
    }

    // Loop through top ten, 3 before and 3 after lists to find if give data exists not to repeat
    private bool CheckIfExists(int rank)
    {
        if (CheckIfExistInTop(rank) ||
            CheckIfExistInBefore(rank) ||
            CheckIfExistInAfter(rank))
        {
            return true;
        }
        return false;
    }
    // Loop through the list of players ranked in top ten and see if iven data exists
    private bool CheckIfExistInTop(int rank)
    {
        for (int i = 0; i < top.Count; i++)
        {
            if (top[i].rank == rank)
            {
                return true;
            }
        }
        return false;
    }
    // Loop through the list of players ranked before you and see if iven data exists
    private bool CheckIfExistInBefore(int rank)
    {
        for (int i = 0; i < before.Count; i++)
        {
            if (before[i].rank == rank)
            {
                return true;
            }
        }
        return false;
    }

    private void BuildUpList()
    {
        // Loop through top ten players and instantiate an item object
        top.ForEach(item =>
        {
            if (item.rank > 3)
            {
                GameObject leaderboardItem = Instantiate(leaderboardItemPrefab, transform.position, Quaternion.identity);
                // Set its parent to be scroll content, for scroll functionality to work properly
                leaderboardItem.transform.SetParent(leaderboardScrollContent.transform);

                // Compare item from top ten with your rank incase you are in top ten
                if (item.rank == youMock.rank)
                {
                    // Show frame around your entry
                    ShowYourEntryFrame(leaderboardItem);
                }

                // Set its name component text mesh pro value to name from top list
                leaderboardItem.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.name;
                // Set its rank component text mesh pro value to rank from top list converted to string
                leaderboardItem.transform.Find("Rank").GetComponent<TextMeshProUGUI>().text = item.rank.ToString();
                // Set its ball icon based on data from server and indexes of balls
                leaderboardItem.transform.Find("Ball").GetComponent<Image>().sprite = ballSprites[item.ballIndex];
            } else
            {
                if (item.rank == 1)
                {
                    // Set golden podium
                    goldName.GetComponent<TextMeshProUGUI>().text = item.name;
                    goldBall.GetComponent<Image>().sprite = ballSprites[item.ballIndex];
                }
                else if (item.rank == 2)
                {
                    // Set silver podium
                    silverName.GetComponent<TextMeshProUGUI>().text = item.name;
                    silverBall.GetComponent<Image>().sprite = ballSprites[item.ballIndex];
                } else
                {
                    // Set bronze podium
                    bronzeName.GetComponent<TextMeshProUGUI>().text = item.name;
                    bronzeBall.GetComponent<Image>().sprite = ballSprites[item.ballIndex];
                }
            }
        });

        // Add tripple dots after top ten only if your rank is > 14,
        // since at 14 the the top ten and 3 before you become continuous, so no need for dots in between
        if (you.rank > 14)
        {
            CreateTrippleDotsEntry();
        }

        // Loop through before players and instantiate an item object
        before.ForEach(item =>
        {
            GameObject leaderboardItem = Instantiate(leaderboardItemPrefab, transform.position, Quaternion.identity);
            // Set its parent to be scroll content, for scroll functionality to work properly
            leaderboardItem.transform.SetParent(leaderboardScrollContent.transform);
            SetItemEntry(leaderboardItem, item);
        });

        // Create your entry item only if your rank is not in top ten
        // 0 is assigned by default if there is no value
        if (you.rank != 0)
        {
            CreateYourEntry();
        }

        // Loop through after players and instantiate an item object
        after.ForEach(item =>
        {
            GameObject leaderboardItem = Instantiate(leaderboardItemPrefab, transform.position, Quaternion.identity);
            // Set its parent to be scroll content, for scroll functionality to work properly
            leaderboardItem.transform.SetParent(leaderboardScrollContent.transform);
            SetItemEntry(leaderboardItem, item);
        });

        CreateTrippleDotsEntry();

        // Add the scroll value after all the data is populated
        ScrollListToPlayer();
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

    private void CreateYourEntry()
    {
        // Create tripple dots to separate different lists
        GameObject leaderboardItem = Instantiate(leaderboardItemPrefab, transform.position, Quaternion.identity);
        // Set its parent to be scroll content, for scroll functionality to work properly
        leaderboardItem.transform.SetParent(leaderboardScrollContent.transform);
        // Show frame around your entry
        ShowYourEntryFrame(leaderboardItem);
        SetItemEntry(leaderboardItem, you);
    }

    private void SetItemEntry(GameObject item, (string name, int rank, int ballIndex) value)
    {
        // Set its name component text mesh pro value to your name
        item.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = value.name;
        // Set its rank component text mesh pro value to your rank
        item.transform.Find("Rank").GetComponent<TextMeshProUGUI>().text = value.rank.ToString();
        // Set its ball icon based on data from server and indexes of balls
        item.transform.Find("Ball").GetComponent<Image>().sprite = ballSprites[value.ballIndex];
    }

    private void ShowYourEntryFrame(GameObject item)
    {
        // Show leaderboard frame for your entry
        item.transform.Find("Frame").gameObject.SetActive(true);
    }

    // Loop through the list of players ranked after you and see if iven data exists
    private bool CheckIfExistInAfter(int rank)
    {
        for (int i = 0; i < after.Count; i++)
        {
            if (after[i].rank == rank)
            {
                return true;
            }
        }
        return false;
    }

    // Take text inside input field
    private void SubmitName(string value)
    {
        // Take the text and pass it to the post request
        Debug.Log(value);
    }

    // Close Leadersboard Scene
    public void ClickExitButton()
    {
        if (exitButton.GetComponent<Button>().IsInteractable())
        {
            // Trigge rthe click animation on exit button of the chest room top left
            exitButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            StartCoroutine(LoadCloseLeadersboardCoroutine(0.2f));
        }
    }

    // Close Leadersboard Scene
    public void ClickChangeNameButton()
    {
        if (changeNameCanvasButton.GetComponent<Button>().IsInteractable())
        {
            // Trigge rthe click animation on exit button of the chest room top left
            changeNameCanvasButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            StartCoroutine(LoadChangeNameCoroutine(0.2f));
        }
    }

    public IEnumerator LoadChangeNameCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        changeName.SetActive(!changeName.activeSelf);
    }

    public IEnumerator LoadCloseLeadersboardCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        navigator.LoadMainScene();
    }

    public void ClickSaveName()
    {
        // When name is changed and save button is shown
        if (changeNameSaveButton.GetComponent<Button>().IsInteractable())
        {
            changeNameSaveButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            StartCoroutine(LoadSaveNameCoroutine(0.2f));
        }
    }

    public void ClickGetDiamonds()
    {
        if (changeNameGetDiamondsButton.GetComponent<Button>().IsInteractable())
        {
            changeNameGetDiamondsButton.GetComponent<TriggerButton>().ClickButton(0.2f);
            StartCoroutine(LoadGetDiamondsCoroutine(0.2f));
        }
    }

    public IEnumerator LoadSaveNameCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        SaveName();
    }

    public IEnumerator LoadGetDiamondsCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        GetDiamonds();
    }

    public void SendData()
    {
        PlayerData newPlayer = new PlayerData(player);

        //string url = "https://abboxgames.com/1/v1/save";

        string url = "http://localhost:5001/1/v1/save";

        PlayerJson playerJson = new PlayerJson();
        playerJson.playerId = SystemInfo.deviceUniqueIdentifier;
        playerJson.playerData = newPlayer;

        string json = JsonUtility.ToJson(playerJson);

        StartCoroutine(PostRequestCoroutine(url, json));
    }

    // This is for invisible button that covers the rest of the screen when modal is open
    public void CloseChangeName()
    {
        changeName.SetActive(false);
    }

    // Save name and get 3 diamonds
    public void SaveName()
    {
        Debug.Log("Save Name");
    }

    // Save name and get 3 diamonds
    public void GetDiamonds()
    {
        Debug.Log("Get Diamonds");
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
}
