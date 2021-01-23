using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Server : MonoBehaviour
{
    // To send player data to server
    private class PlayerJson
    {
        public string playerId;
        public PlayerData playerData;
    }

    public static Server instance;

    Player player;
    TV tv;
    GameObject tvLight;
    GameObject tvSwitch;
    bool switchIsOn;

    float switchTurnSpeed = 2;

    void Awake()
    {
        player = FindObjectOfType<Player>();

        // Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        tv = FindObjectOfType<TV>();
        tvLight = GameObject.Find("Light");
        tvSwitch = GameObject.Find("Switch");
    }

    void Start()
    {
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

    private void SendData()
    {
        PlayerData newPlayer = new PlayerData(player);

        //string url = "https://abboxgames.com/1/v1/save";

        string url = "http://localhost:5001/v1/players";

        //string adurl = "http://localhost:5001/1/v1/adlink";

        PlayerJson playerJson = new PlayerJson();
        playerJson.playerId = SystemInfo.deviceUniqueIdentifier;
        playerJson.playerData = newPlayer;

        string json = JsonUtility.ToJson(playerJson);

        StartCoroutine(PostRequestCoroutine(url, json));

        //StartCoroutine(GetAdLinkCoroutine(adurl));
    }

    public IEnumerator PostRequestCoroutine(string url, string json)
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

    public IEnumerator GetAdLinkCoroutine(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = url.Split('/');
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
