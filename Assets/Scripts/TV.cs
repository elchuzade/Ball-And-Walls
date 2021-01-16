using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TV : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    void Awake()
    {
        videoPlayer = transform.Find("VideoPlayer").GetComponent<VideoPlayer>();    
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAdLink(string url)
    {
        videoPlayer.url = url;
        videoPlayer.Play();
    }
}
