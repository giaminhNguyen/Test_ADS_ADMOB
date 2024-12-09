using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

#if USE_IRON_SOURCE
using com.unity3d.mediation;
#endif

namespace UnityHelper
{
    public class ADSManager_IS : MonoBehaviour
    {

        #region Properties

        public static ADSManager_IS Instance;

        public bool HasRewardedVideo
        {
            get
            {
                var check = false;
                #if USE_IRON_SOURCE
            check = IronSource.Agent.isRewardedVideoAvailable();
                #endif
                return check;
            }
        }

        public bool HasInterstitial
        {
            get
            {
                var check = false;
                #if USE_IRON_SOURCE
             check = _interstitialAd?.IsAdLoaded()??false;
                #endif
                return check;
            }
        }

        public bool HasBanner { get; set; }

        #if UNITY_ANDROID
        [SerializeField]
        private string _appKey = "85460dcd";

        [SerializeField]
        private string _bannerAdUnitId = "thnfvcsog13bhn08";

        [SerializeField]
        private string _interstitialAdUnitId = "aeyqi3vqlv6o8sh9";
        #elif UNITY_IPHONE
    [SerializeField]
    private string _appKey = "8545d445";

    [SerializeField]
    private string _bannerAdUnitId = "iep3rxsyp9na3rw8";

    [SerializeField]
    private string _interstitialAdUnitId = "wmgt0712uuux8ju4";
        #else
    [SerializeField]
    private string _appKey = "unexpected_platform";
    [SerializeField]
    private string _bannerAdUnitId = "unexpected_platform";
    [SerializeField]
    private string _interstitialAdUnitId = "unexpected_platform";
        #endif
        #if USE_IRON_SOURCE
    [Header("Banner")]
    public bool autoLoadBanner = true;
    public LevelPlayAdSize bannerSize = LevelPlayAdSize.BANNER;
    public LevelPlayBannerPosition bannerPosition = LevelPlayBannerPosition.BottomCenter;
    
    private LevelPlayBannerAd       _bannerAd;
    private LevelPlayInterstitialAd _interstitialAd;
    private Action _actionDoneRewarded;
    private Action _actionDoneInterstitial;
        #endif

        #endregion


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
        #if USE_IRON_SOURCE

    public void Start()
    {
        #if UNITY_EDITOR
        return;
        #endif
        IronSource.Agent.validateIntegration();
        // SDK init
        Debug.Log("unity-script: LevelPlay SDK initialization");
        LevelPlay.Init(_appKey,adFormats:new []{LevelPlayAdFormat.REWARDED});
        
        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
        LevelPlay.OnInitFailed += SdkInitializationFailedEvent;
        
    }

    void InitializeAds()
    {
#if UNITY_EDITOR
return;
#endif
        //Add ImpressionSuccess Event
        IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;

        
        //Add AdInfo Rewarded Video Events
        IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;
        IronSourceRewardedVideoEvents.onAdLoadFailedEvent += RewardedVideoOnAdLoadFailedEvent; 
        
        _bannerAd = new LevelPlayBannerAd(_bannerAdUnitId,bannerSize,bannerPosition);

        // Register to Banner events
        _bannerAd.OnAdLoaded += BannerOnAdLoadedEvent;
        _bannerAd.OnAdLoadFailed += BannerOnAdLoadFailedEvent;
        _bannerAd.OnAdDisplayed += BannerOnAdDisplayedEvent;
        _bannerAd.OnAdDisplayFailed += BannerOnAdDisplayFailedEvent;
        _bannerAd.OnAdClicked += BannerOnAdClickedEvent;
        _bannerAd.OnAdCollapsed += BannerOnAdCollapsedEvent;
        _bannerAd.OnAdLeftApplication += BannerOnAdLeftApplicationEvent;
        _bannerAd.OnAdExpanded += BannerOnAdExpandedEvent;

        // Create Interstitial object
        _interstitialAd = new LevelPlayInterstitialAd(_interstitialAdUnitId);
        
        // Register to Interstitial events
        _interstitialAd.OnAdLoaded += InterstitialOnAdLoadedEvent;
        _interstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailedEvent;
        _interstitialAd.OnAdDisplayed += InterstitialOnAdDisplayedEvent;
        _interstitialAd.OnAdDisplayFailed += InterstitialOnAdDisplayFailedEvent;
        _interstitialAd.OnAdClicked += InterstitialOnAdClickedEvent;
        _interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
        _interstitialAd.OnAdInfoChanged += InterstitialOnAdInfoChangedEvent;
    }
    
    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }
    
    private void OnDisable()
    {
        _bannerAd?.DestroyAd();
        _interstitialAd?.DestroyAd();
    }
    

    #region Public Func
    
    public async void DelayShowRewardedVideo(int delayTimeMs,Action actionDone = null,Action actionFailed = null)
    {
        await Task.Delay(delayTimeMs);
        ShowRewardedVideo(actionDone,actionFailed);
    }

    public void ShowRewardedVideo(Action actionDone = null,Action actionFailed = null)
    {
        
        #if UNITY_EDITOR
        actionDone?.Invoke();
        return;
        #endif
        
        if (!HasRewardedVideo)
        {
            ShowMessage("No Reward Video Available");
            actionFailed?.Invoke();
            return;
        }
        
        _actionDoneRewarded = actionDone;
        IronSource.Agent.showRewardedVideo();
    }
    
    public async void DelayShowInterstitial(int delayTimeMs,Action actionDone = null,Action actionFailed = null)
    {
        await Task.Delay(delayTimeMs);
        ShowInterstitial(actionDone,actionFailed);
    }

    public void ShowInterstitial(Action actionDone = null,Action actionFailed = null)
    {
        #if UNITY_EDITOR
        actionDone?.Invoke();
        return;
        #endif

        if (!HasInterstitial)
        {
            ShowMessage("No Interstitial Available");
            actionFailed?.Invoke();
            return;
        }
        _actionDoneInterstitial = actionDone;
        _interstitialAd.ShowAd();
    }
    
    public void LoadBanner()
    {
        ShowMessage("Load Banner");
        _bannerAd?.LoadAd();
    }

    public void HideBanner()
    {
        ShowMessage("Hide Banner");
        _bannerAd?.HideAd();
    }
    
    #endregion

    #region Other Func
    
    public void ShowMessage(string msg)
    {
        #if !UNITY_EDITOR && UNITY_ANDROID && USE_SHOWMESSAGE
        AndroidJavaObject @static =
 new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
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

    #region Init callback handlers

    void SdkInitializationCompletedEvent(LevelPlayConfiguration config)
    {
        Debug.Log("unity-script: I got SdkInitializationCompletedEvent with config: "+ config);
        InitializeAds();
        _interstitialAd.LoadAd();
        IronSource.Agent.loadRewardedVideo();

        if (autoLoadBanner)
        {
            _bannerAd.LoadAd();
        }
    }
    
    void SdkInitializationFailedEvent(LevelPlayInitError error)
    {
        Debug.Log("unity-script: I got SdkInitializationFailedEvent with error: "+ error);
    }

    #endregion

    #region AdInfo Rewarded Video
    
    private void RewardedVideoOnAdLoadFailedEvent(IronSourceError obj)
    {
        ShowMessage("RewardedVideoOnAdLoadFailedEvent");
        ShowMessage("Load Interstitial");
        IronSource.Agent.loadRewardedVideo();
    }
    
    void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got RewardedVideoOnAdOpenedEvent With AdInfo " + adInfo);
    }

    async void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got RewardedVideoOnAdClosedEvent With AdInfo " + adInfo);
        _actionDoneRewarded?.Invoke();
        await Task.Delay(100);
        ShowMessage("Load Interstitial");
        IronSource.Agent.loadRewardedVideo();
    }

    void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
    {
        ShowMessage("unity-script: I got RewardedVideoOnAdAvailable With AdInfo " + adInfo);
    }

    void RewardedVideoOnAdUnavailable()
    {
        ShowMessage("unity-script: I got RewardedVideoOnAdUnavailable");
    }

    void RewardedVideoOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got RewardedVideoOnAdShowFailedEvent With Error" + ironSourceError + "And AdInfo " + adInfo);
    }

    void RewardedVideoOnAdRewardedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got RewardedVideoOnAdRewardedEvent With Placement" + ironSourcePlacement + "And AdInfo " + adInfo);
    }

    void RewardedVideoOnAdClickedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got RewardedVideoOnAdClickedEvent With Placement" + ironSourcePlacement + "And AdInfo " + adInfo);
    }

    #endregion
    
    #region AdInfo Interstitial

    void InterstitialOnAdLoadedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdLoadedEvent With AdInfo " + adInfo);
    }

    void InterstitialOnAdLoadFailedEvent(LevelPlayAdError error)
    {
        ShowMessage("InterstitialOnAdLoadFailedEvent");
        ShowMessage("Load Interstitial");
        _interstitialAd.LoadAd();
    }
	
    void InterstitialOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdDisplayedEvent With AdInfo " + adInfo);
    }
	
    void InterstitialOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError infoError)
    {
        Debug.Log("unity-script: I got InterstitialOnAdDisplayFailedEvent With InfoError " + infoError);
    }
	
    void InterstitialOnAdClickedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdClickedEvent With AdInfo " + adInfo);
    }

    async void InterstitialOnAdClosedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdClosedEvent With AdInfo " + adInfo);
        _actionDoneInterstitial?.Invoke();
        await Task.Delay(100);
        ShowMessage("Load Interstitial");
        _interstitialAd.LoadAd();
    }
	
    void InterstitialOnAdInfoChangedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdInfoChangedEvent With AdInfo " + adInfo);
    }

    #endregion

    #region Banner AdInfo

    void BannerOnAdLoadedEvent(LevelPlayAdInfo adInfo)
    {
        HasBanner = true;
        Debug.Log("unity-script: I got BannerOnAdLoadedEvent With AdInfo " + adInfo);
    }

    async void BannerOnAdLoadFailedEvent(LevelPlayAdError ironSourceError)
    {
        HasBanner = false;
        Debug.Log("unity-script: I got BannerOnAdLoadFailedEvent With Error " + ironSourceError);
        await Task.Delay(100);
        _bannerAd.LoadAd();
    }

    void BannerOnAdClickedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdClickedEvent With AdInfo " + adInfo);
    }

    void BannerOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdDisplayedEvent With AdInfo " + adInfo);
    }
	
    void BannerOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError adInfoError)
    {
        Debug.Log("unity-script: I got BannerOnAdDisplayFailedEvent With AdInfoError " + adInfoError);
    }

    void BannerOnAdCollapsedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdCollapsedEvent With AdInfo " + adInfo);
    }

    void BannerOnAdLeftApplicationEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdLeftApplicationEvent With AdInfo " + adInfo);
    }

    void BannerOnAdExpandedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdExpandedEvent With AdInfo " + adInfo);
    }

    #endregion

    #region ImpressionSuccess callback handler

    void ImpressionDataReadyEvent(IronSourceImpressionData impressionData)
    {
        Debug.Log("unity - script: I got ImpressionDataReadyEvent ToString(): " + impressionData.ToString());
        Debug.Log("unity - script: I got ImpressionDataReadyEvent allData: " + impressionData.allData);
    }

    #endregion
        #endif
    }
}