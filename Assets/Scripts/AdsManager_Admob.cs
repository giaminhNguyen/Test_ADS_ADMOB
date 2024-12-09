using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
#if USE_ADS_ADMOB
using System.Threading.Tasks;
using GoogleMobileAds.Api;
#endif
public class AdsManager_Admob : MonoBehaviour
{
    #if USE_ADS_ADMOB

    #region Properties
    
    public static AdsManager_Admob Instance;
    public        bool             HasRewardedVideo => _rewardedAd?.CanShowAd() ?? false;
    public        bool             HasInterstitial  => _interstitialAd?.CanShowAd() ?? false;
    public        bool             HasBanner        { get; set; }
    
    #if UNITY_ANDROID
    [SerializeField]
    private string bannerId;
    [SerializeField]
    private string interId;
    [SerializeField]
    private string rewardedId;
    
    private BannerView     _bannerView;
    private InterstitialAd _interstitialAd;
    private RewardedAd     _rewardedAd;
    
    
    [Header("Banner")]
    public bool autoLoadBanner = true;
    public AdSize                  bannerSize     = AdSize.Banner;
    public AdPosition bannerPosition = AdPosition.Bottom;
    
    #endif
    
    [Header("Other")]
    public int delayTimeShowRewardedMs     = 500;
    public int delayTimeShowInterstitialMs = 500;
    
    private Action _actionDoneRewarded;
    private Action _actionDoneInterstitial;
    
    #endregion

    #region Unity Func
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize(initStatus =>
        {
            LoadBannerAd();
            LoadInterstitialAd();
            LoadRewardedAd();
        });
    }

    #endregion
    

    #region Public Func
    
    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(rewardedId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            // if error is not null, the load request failed.
            if (error != null || ad == null)
            {
                DelayAction(0.1f, LoadRewardedAd);
                return;
            }

            _rewardedAd = ad;
            RewaredEvent(_rewardedAd);
        });
    }
    public void ShowRewardedAd(Action actionDone = null,Action actionFailed = null)
    {
        #if UNITY_EDITOR
        actionDone?.Invoke();
        return;
        #endif
        

        if (!HasRewardedVideo)
        {
            actionFailed?.Invoke();
            return;
        }
        
        _actionDoneRewarded = actionDone;
        
        _rewardedAd.Show((Reward reward) =>
        {
        });
    }
    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }


        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(interId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            // if error is not null, the load request failed.
            if (error != null || ad == null)
            {
                DelayAction(0.1f, LoadInterstitialAd);
                return;
            }
            _interstitialAd = ad;
            InterstitialEvent(_interstitialAd);
        });
    }
    public void ShowInterstitialAd(Action actionDone = null,Action actionFailed = null)
    {
        
        #if UNITY_EDITOR
        actionDone?.Invoke();
        return;
        #endif
        
        if(!HasInterstitial)
        {
            actionFailed?.Invoke();
            return;
        }
        
        _actionDoneInterstitial = actionDone;
        _interstitialAd.Show();
    }
    public void LoadBannerAd()
    {
        // create an instance of a banner view first.
        if (_bannerView == null)
        {
            CreateBannerView();
        }
        BannerAdEvents();

        // create our request used to load the ad.
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        // send the request to load the ad.
        _bannerView.LoadAd(adRequest);
    }
    public void DestroyBannerView()
    {
        if (_bannerView != null)
        {
            _bannerView.Destroy();
            _bannerView = null;
        }
    }

    #endregion
    
    
    
    #region Banner Ads

    

    private void CreateBannerView()
    {
        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            DestroyBannerView();
        }

        // Create a 320x50 banner at top of the screen
        _bannerView = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);
    }

    
    private void BannerAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += OnBannerAdLoaded;
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += OnBannerAdLoadFailed;
        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += OnBannerAdPaid;
        // Raised when an impression is recorded for an ad.
        _bannerView.OnAdImpressionRecorded += OnBannerAdImpressionRecorded;
        // Raised when a click is recorded for an ad.
        _bannerView.OnAdClicked += OnBannerAdClicked;
        // Raised when an ad opened full screen content.
        _bannerView.OnAdFullScreenContentOpened += OnBannerAdFullScreenContentOpened;
        // Raised when the ad closed full screen content.
        _bannerView.OnAdFullScreenContentClosed += OnBannerAdFullScreenContentClosed;
    }

    private void OnBannerAdLoaded()
    {
    }

    private void OnBannerAdLoadFailed(LoadAdError obj)
    {
    }

    private void OnBannerAdPaid(AdValue obj)
    {
    }

    private void OnBannerAdImpressionRecorded()
    {
    }

    private void OnBannerAdClicked()
    {
    }

    private void OnBannerAdFullScreenContentOpened()
    {
    }

    private void OnBannerAdFullScreenContentClosed()
    {
    }

    #endregion

    #region Interstitial Ads
    

    private void InterstitialEvent(InterstitialAd interstitialAd)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += OnInterAdPaid;
        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += OnInterAdImpressionRecorded;
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += OnInterAdClicked;
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += OnInterAdFullScreenContentOpened;
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += OnInterAdFullScreenContentClosed;
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += OnInterAdFullScreenContentFailed;
    }

    private void OnInterAdPaid(AdValue obj)
    {
        ShowMessage("Interstitial ad paid " + obj.Value + " " + obj.CurrencyCode);
    }

    private void OnInterAdImpressionRecorded()
    {
        ShowMessage("Interstitial ad recorded an impression.");
    }

    private void OnInterAdClicked()
    {
        ShowMessage("Interstitial ad was clicked.");
    }

    private void OnInterAdFullScreenContentOpened()
    {
        
    }

    private void OnInterAdFullScreenContentClosed()
    {
    }

    private void OnInterAdFullScreenContentFailed(AdError obj)
    {
    }

    #endregion

    #region Rewareded Ads
    
    private void RewaredEvent(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += OnRewardAdPaid;
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += OnRewardAdImpressionRecorded;
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += OnRewardAdClicked;
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += OnRewardAdFullScreenContentOpened;
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += OnRewardAdFullScreenContentClosed;
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += OnRewardAdFullScreenContentFailed;
    }

    private void OnRewardAdPaid(AdValue obj)
    {
        ShowMessage("Rewarded ad paid " + obj.Value + " " + obj.CurrencyCode);
    }

    private void OnRewardAdImpressionRecorded()
    {
        ShowMessage("Rewarded ad recorded an impression.");
    }

    private void OnRewardAdClicked()
    {
        ShowMessage("Rewarded ad was clicked.");
    }

    private void OnRewardAdFullScreenContentOpened()
    {
        ShowMessage("Rewarded ad full screen content opened.");
    }

    private void OnRewardAdFullScreenContentClosed()
    {
        ShowMessage("Rewarded ad full screen content closed.");
        
    }

    private void OnRewardAdFullScreenContentFailed(AdError obj)
    {
        ShowMessage("Rewarded ad failed to open full screen content with error : " + obj);
    }

    #endregion
    
    #region Other Func
    
    private async void DelayAction(float delayTime, Action action)
    {
        await Task.Delay((int)(delayTime * 1000));
        action?.Invoke();
    }
    
    public void ShowMessage(string msg)
    {
        #if !UNITY_EDITOR && UNITY_ANDROID && USE_SHOWMESSAGE
        AndroidJavaObject @static = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject androidJavaObject = new AndroidJavaClass("android.widget.Toast");
        androidJavaObject.CallStatic<AndroidJavaObject>("makeText", new object[]
        {
                @static,
                msg,
                androidJavaObject.GetStatic<int>("LENGTH_SHORT")
        }).Call("show", Array.Empty<object>());
        #endif
        Debug.Log(msg);
    }

    #endregion
    
    
    #endif
}
