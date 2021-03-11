using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour
{
    // Video Link
    public class VideoJson
    {
        public string video; // link to video
        public string name; // product title
        public string website; // link to follow on click
    }

    // LOCAL TESTING
    //string abboxAdsApi = "http://localhost:5002";

    // STAGING
    //string abboxAdsApi = "https://staging.ads.abbox.com";

    // PRODUCTION
    string abboxAdsApi = "https://ads.abbox.com";

    // To send response to corresponding files
    [SerializeField] MainStatus mainStatus;

    public void GetVideoLink()
    {
        string videoUrl = abboxAdsApi + "/api/v1/videos";
        StartCoroutine(GetAdLinkCoroutine(videoUrl));
    }

    // This one is for TV in main scene
    // Get the latest video link, for now in general, in future personal based on the DeviceId
    private IEnumerator GetAdLinkCoroutine(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Send request and wait for the desired response.
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
}
