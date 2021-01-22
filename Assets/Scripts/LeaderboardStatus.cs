using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class LeaderboardStatus : MonoBehaviour
{
    [SerializeField] GameObject leaderboardItemPrefab;
    [SerializeField] Player player;
    [SerializeField] Sprite goldCrown;
    [SerializeField] Sprite silverCrown;
    [SerializeField] Sprite bronzeCrown;

    private GameObject goldName;
    private GameObject silverName;
    private GameObject bronzeName;

    private Navigator navigator;

    private TriggerAnimation exitButtonScript;

    private GameObject leaderboardScrollContent;

    private GameObject leaderboardScrollbar;

    // To send player data to server
    private class PlayerJson
    {
        public string playerId;
        public PlayerData playerData;
    }

    private (string name, int rank)[] topMock = {
        ("Player-100001", 1),
        ("Player-100002", 2),
        ("Player-100003", 3),
        ("Player-100004", 4),
        ("Player-100005", 5),
        ("Player-100006", 6),
        ("Player-100007", 7),
        ("Player-100008", 8),
        ("Player-100009", 9),
        ("Player-100010", 10)
    };

    private (string name, int rank)[] beforeMock = {
        ("Player-100188", 188),
        ("Player-100189", 189),
        ("Player-100190", 190)
    };

    private (string name, int rank)[] afterMock = {
        ("Player-100192", 192),
        ("Player-100193", 193),
        ("Player-100194", 194)
    };

    private (string name, int rank) youMock = ("Kamran", 191);

    private List<(string name, int rank)> before = new List<(string name, int rank)>();
    private List<(string name, int rank)> after = new List<(string name, int rank)>();
    private List<(string name, int rank)> top = new List<(string name, int rank)>();
    private (string name, int rank) you;

    void Awake()
    {
        leaderboardScrollContent = GameObject.Find("LeaderboardScrollContent");
        leaderboardScrollbar = GameObject.Find("LeaderboardScrollbar");
        navigator = FindObjectOfType<Navigator>();
        goldName = GameObject.Find("GoldName");
        silverName = GameObject.Find("SilverName");
        bronzeName = GameObject.Find("BronzeName");
        GameObject exitButtonObject = GameObject.Find("ExitButton");
        exitButtonScript = exitButtonObject.GetComponent<TriggerAnimation>();
    }

    void Start()
    {
        PopulateMockData();
        BuildUpList();
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

                // If the rank is in top 3, hide rank component and show icon component
                if (item.rank < 4)
                {
                    leaderboardItem.transform.Find("Icon").gameObject.SetActive(true);
                    leaderboardItem.transform.Find("Rank").gameObject.SetActive(false);
                }
                // Set icon for crown component based on the rank
                if (item.rank == 1)
                {
                    // Show golden crown
                    leaderboardItem.transform.Find("Icon").GetComponent<Image>().sprite = goldCrown;
                    // Zoom in the golden crown
                    leaderboardItem.transform.Find("Icon").localScale = new Vector3(1.2f, 1.2f, 1.2f);
                    // Set Rank 1 player on the podium
                    goldName.GetComponent<TextMeshProUGUI>().text = item.name;
                } else if (item.rank == 2)
                {
                    // Show silver crown
                    leaderboardItem.transform.Find("Icon").GetComponent<Image>().sprite = silverCrown;
                    // Set Rank 2 player on the podium
                    silverName.GetComponent<TextMeshProUGUI>().text = item.name;
                } else if (item.rank == 3)
                {
                    // Show bronze crown
                    leaderboardItem.transform.Find("Icon").GetComponent<Image>().sprite = bronzeCrown;
                    // Zoom out the bronze crown
                    leaderboardItem.transform.Find("Icon").localScale = new Vector3(0.8f, 0.8f, 0.8f);
                    // Set Rank 3 player on the podium
                    bronzeName.GetComponent<TextMeshProUGUI>().text = item.name;
                }

                // Set its name component text mesh pro value to name from top list
                leaderboardItem.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.name;
                // Set its rank component text mesh pro value to rank from top list converted to string
                leaderboardItem.transform.Find("Rank").GetComponent<TextMeshProUGUI>().text = item.rank.ToString();
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
            // Set its name component text mesh pro value to name from top list
            leaderboardItem.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.name;
            // Set its rank component text mesh pro value to rank from top list converted to string
            leaderboardItem.transform.Find("Rank").GetComponent<TextMeshProUGUI>().text = item.rank.ToString();
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
            // Set its name component text mesh pro value to name from top list
            leaderboardItem.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.name;
            // Set its rank component text mesh pro value to rank from top list converted to string
            leaderboardItem.transform.Find("Rank").GetComponent<TextMeshProUGUI>().text = item.rank.ToString();
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
    }

    private void CreateYourEntry()
    {
        // Create tripple dots to separate different lists
        GameObject leaderboardItem = Instantiate(leaderboardItemPrefab, transform.position, Quaternion.identity);
        // Set its parent to be scroll content, for scroll functionality to work properly
        leaderboardItem.transform.SetParent(leaderboardScrollContent.transform);
        // Show frame around your entry
        ShowYourEntryFrame(leaderboardItem);
        // Set its name component text mesh pro value to your name
        leaderboardItem.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = you.name;
        // Set its rank component text mesh pro value to your rank
        leaderboardItem.transform.Find("Rank").GetComponent<TextMeshProUGUI>().text = you.rank.ToString();
    }

    private void ShowYourEntryFrame(GameObject item)
    {
        // Show leaderboard frame for your entry
        item.transform.Find("Frame").gameObject.SetActive(true);
        // Show edit button for changing name
        item.transform.Find("Edit").gameObject.SetActive(true);
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

    // TEST UNITY INPUT FIELD

    // Typed in something
    public void OnValueChange ()
    {
        Debug.Log("typed");
    }

    // Touched the input field
    public void OnSelect()
    {
        Debug.Log("touched");
    }

    // Close Leadersboard Scene
    public void ClickExitButton()
    {
        // Trigge rthe click animation on exit button of the chest room top left
        exitButtonScript.Trigger();
        StartCoroutine(LoadCloseLeadersboard(0.2f));
    }

    public IEnumerator LoadCloseLeadersboard(float time)
    {
        yield return new WaitForSeconds(time);

        navigator.LoadMainScene();
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
