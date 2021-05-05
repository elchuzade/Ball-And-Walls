using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdMobManager : MonoBehaviour
{
    private BannerView bannerAd;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

#if UNITY_ANDROID
    string bannerId = "ca-app-pub-3940256099942544/6300978111";
    // ca-app-pub-5721177105818446/5276477219
    string interstitialId = "ca-app-pub-3940256099942544/1033173712";
    // ca-app-pub-5721177105818446/4852971237
    string rewardedId = "ca-app-pub-3940256099942544/5224354917";
    // ca-app-pub-5721177105818446/4696240329
    //string gameId = "ca-app-pub-5721177105818446~9247511819";

#elif UNITY_IPHONE
    string bannerId = "ca-app-pub-3940256099942544/2934735716";
    // ca-app-pub-5721177105818446/8449435465
    string interstitialId = "ca-app-pub-3940256099942544/4411468910";
    // ca-app-pub-5721177105818446/6529849132
    string rewardedId = "ca-app-pub-3940256099942544/1712485313";
    // ca-app-pub-5721177105818446/8772869092
    //string gameId = "ca-app-pub-5721177105818446~8001060516";
#else
    string adUnitId = "unexpected_platform";
    string interstitialId = "unexpected_platform";
    string rewardedId = "unexpected_platform";
#endif

    public static AdMobManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });
    }

    #region Banner
    public void ShowAdmobBanner()
    {
        this.RequestBanner();
    }

    public void HideAdmobBanner()
    {
        this.bannerAd.Hide();
    }

    private void RequestBanner()
    {
        // Create a 320x50 banner at the top of the screen.
        this.bannerAd = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);

        this.bannerAd.OnAdLoaded += this.HandleOnAdLoaded;
        this.bannerAd.OnAdFailedToLoad += this.HandleOnAdFailedToLoad;

        AdRequest request = new AdRequest.Builder().Build();

        this.bannerAd.LoadAd(request);
    }
    #endregion

    #region Interstitial
    public void ShowAdmobStandardAd()
    {
        this.RequestInterstitial();

        if (this.interstitialAd.IsLoaded())
        {
            this.interstitialAd.Show();
        }
    }

    private void RequestInterstitial()
    {
        this.interstitialAd = new InterstitialAd(interstitialId);

        this.interstitialAd.OnAdLoaded += this.HandleOnAdLoaded;

        this.interstitialAd.OnAdFailedToLoad += this.HandleOnAdFailedToLoad;

        this.interstitialAd.OnAdClosed += this.HandleOnAdClosed;

        this.interstitialAd.OnAdLeavingApplication += this.HandleOnAdLeavingApplication;

        AdRequest request = new AdRequest.Builder().Build();

        this.interstitialAd.LoadAd(request);
    }
    #endregion

    #region Rewarded
    public void ShowAdmobRewardedAd()
    {
        this.RequestRewarded();
        Debug.Log("rewarded");
        if (this.rewardedAd.IsLoaded())
        {
            this.rewardedAd.Show();
        }
    }

    private void RequestRewarded()
    {
        rewardedAd = new RewardedAd(rewardedId);

        AdRequest request = new AdRequest.Builder().Build();

        rewardedAd.LoadAd(request);

        rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        rewardedAd.OnAdClosed += HandleRewardedAdClosed;
        rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToShow;
        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
    }

    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardedAdLoaded event received");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        Debug.Log(
            "HandleRewardedAdFailedToLoad event received with message: "
                             + args.Message);
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        Debug.Log(
            "HandleRewardedAdFailedToShow event received with message: "
                             + args.Message);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardedAdClosed event received");
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        Debug.Log(
            "HandleRewardedAdRewarded event received for "
                        + amount.ToString() + " " + type);
    }
    #endregion

    #region Ad Delegates
    public void HandleOnAdLoaded(object sender, EventArgs args)
{
    Debug.Log("Ad Loaded");
}

public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
{
    Debug.Log("couldnt load ad" + args.Message);
}

public void HandleOnAdOpened(object sender, EventArgs args)
{
    Debug.Log("Handle ad opened event received");
}

public void HandleOnAdClosed(object sender, EventArgs args)
{
    Debug.Log("Handle ad closed event received");
}

public void HandleOnAdLeavingApplication(object sender, EventArgs args)
{
    Debug.Log("Leaving application event received");
}
#endregion
}
