using UnityEngine;
using GoogleMobileAds.Api;
using System;
using System.Collections;

public class AdMobManager : MonoBehaviour
{
    private Action adSuccess;
    private Action adSkipped;
    private Action adFailed;

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
    string gameId = "ca-app-pub-5721177105818446~9247511819";

#elif UNITY_IPHONE
    string bannerId = "ca-app-pub-3940256099942544/2934735716";
    // ca-app-pub-5721177105818446/8449435465
    string interstitialId = "ca-app-pub-3940256099942544/4411468910";
    // ca-app-pub-5721177105818446/6529849132
    string rewardedId = "ca-app-pub-3940256099942544/1712485313";
    //string rewardedId = "940256099942544/1712485313";
    // ca-app-pub-5721177105818446/8772869092
    string gameId = "ca-app-pub-5721177105818446~8001060516";
#else
    string adUnitId = "unexpected_platform";
    string interstitialId = "unexpected_platform";
    string rewardedId = "unexpected_platform";
#endif

    public static AdMobManager instance;

    void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            // Initialize the Google Mobile Ads SDK.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        MobileAds.Initialize(initStatus => { });
    }

    #region Banner
    public static void ShowAdmobBanner()
    {
        instance.RequestBanner();
    }

    public void HideAdmobBanner()
    {
        this.bannerAd.Hide();
    }

    private void RequestBanner()
    {
        // Create a 320x50 banner at the top of the screen.
        this.bannerAd = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);

        this.bannerAd.OnAdFailedToLoad += this.HandleOnBannerAdFailedToLoad;

        AdRequest request = new AdRequest.Builder().Build();

        this.bannerAd.LoadAd(request);
    }

    public void HandleOnBannerAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        AdManager.ShowBanner();
    }
    #endregion

    #region Interstitial
    public void ShowAdmobStandardAd(Action success, Action skipped, Action failed)
    {
        this.adFailed = failed;
        this.adSkipped = skipped;
        this.adSuccess = success;

        this.interstitialAd = new InterstitialAd(interstitialId);

        //this.interstitialAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;

        AdRequest request = new AdRequest.Builder().Build();

        this.interstitialAd.LoadAd(request);

        if (instance.interstitialAd.IsLoaded())
        {
            instance.interstitialAd.Show();
        }
    }
    #endregion

    #region Rewarded
    public void ShowAdmobRewardedAd(Action success, Action skipped, Action failed)
    {
        this.adSuccess = success;
        this.adFailed = failed;
        this.adSkipped = skipped;

        this.rewardedAd = new RewardedAd(rewardedId);
        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);

        //StartCoroutine(TryShowingRewardedAd());
    }



    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdLoaded event received");
        this.rewardedAd.Show();
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToLoad event received with message: "
                             + args.Message);
        // If Admob failed to load an ad, try loading unity ads
        AdManager.ShowStandardAd(this.adSuccess, this.adSkipped, this.adFailed);
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToShow event received with message: "
                             + args.Message);
        // If Admob failed to load an ad, try loading unity ads
        AdManager.ShowStandardAd(this.adSuccess, this.adSkipped, this.adFailed);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdClosed event received");
        this.adSkipped();
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print(
            "HandleRewardedAdRewarded event received for "
                        + amount.ToString() + " " + type);
        this.adSuccess();
    }
    #endregion
}
