using UnityEngine;
using System;
using UnityEngine.Serialization;
using VInspector;
#if USE_ADS_ADMOB
using System.Threading.Tasks;
using GoogleMobileAds.Api;
#endif


namespace UnityHelper
{
    public class AdsManager_ADMOB : MonoBehaviour
    {
        #region Properties

        public static AdsManager_ADMOB Instance;

        public bool HasRewardedVideo
        {
            get
            {
                var check = false;
                #if USE_ADS_ADMOB
                
                #if UNITY_EDITOR
                check = true;
                #else
                check = _rewardedAd?.CanShowAd() ?? false;
                #endif
                
                #endif
                return check;
            }
        }

        public bool HasInterstitial
        {
            get
            {
                var check = false;
                #if USE_ADS_ADMOB
                
                #if UNITY_EDITOR
                check = true;
                #else
                check = _interstitialAd?.CanShowAd() ?? false;
                #endif
                
                #endif
                return check;
            }
        }

        public bool HasBanner { get; set; }

        public bool HasOpenAds
        {
            get
            {
                var check = false;
                #if USE_ADS_ADMOB
                
                #if UNITY_EDITOR
                check = true;
                #else
                check = (_appOpenAd?.CanShowAd() ?? false) && DateTime.Now < _expireTime ;
                #endif
                
                #endif
                return check && _useOpenAds;
            }
        }

        [SerializeField]
        private bool _useOpenAds = true;

        [SerializeField]
        private bool _useBannerAds = true;
        #if UNITY_ANDROID
        [SerializeField]
        private string _rewardAdId = "ca-app-pub-3940256099942544/5224354917";

        [SerializeField]
        private string _interAdId = "ca-app-pub-3940256099942544/1033173712";

        [SerializeField, ShowIf("_useBannerAds", true)]
        private string _bannerAdId = "ca-app-pub-3940256099942544/6300978111";

        [SerializeField, ShowIf("_useOpenAds", true)]
        private string _openAdId = "ca-app-pub-3940256099942544/9257395921";

        [EndIf]
        #elif UNITY_IPHONE
        [SerializeField]
        private  string _rewardAdId = "ca-app-pub-3940256099942544/1712485313";
        [SerializeField]
        private  string _interAdId = "ca-app-pub-3940256099942544/4411468910";
        [SerializeField,ShowIf("_useBannerAds",true)]
        private  string _bannerAdId = "ca-app-pub-3940256099942544/2934735716";
        [SerializeField,ShowIf("_useOpenAds",true)]
        private  string _openAdId = "ca-app-pub-3940256099942544/5575463023";
        [EndIf]
        #else
        [SerializeField]
        private  string _rewardAdId = "unused";
        [SerializeField]
        private  string _interAdId = "unused";
        [SerializeField,ShowIf("_useBannerAds",true)]
        private  string _bannerAdId = "unused";
        [SerializeField,ShowIf("_useOpenAds",true)]
        private  string _openAdId = "unused";
        [EndIf]
        #endif

        #if USE_ADS_ADMOB
        [Header("Banner"), ShowIf("_useBannerAds", true)]
        public bool autoLoadBanner = true;

        public AdSize     bannerSize     = AdSize.Banner;
        public AdPosition bannerPosition = AdPosition.Bottom;

        [EndIf]
        private InterstitialAd _interstitialAd;

        private RewardedAd _rewardedAd;
        private BannerView _bannerView;

        private readonly TimeSpan  TIMEOUT = TimeSpan.FromHours(4);
        private          DateTime  _expireTime;
        private          AppOpenAd _appOpenAd;
        #endif

        private Action _actionDoneRewarded;
        private Action _actionDoneInterstitial;
        private Action _actionDoneOpenAd;

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
            #if UNITY_EDITOR
            HasBanner = true;
            #endif
        }

        #if USE_ADS_ADMOB

        void Start()
        {
            #if UNITY_EDITOR
            return;
            #endif

            // Initialize the Google Mobile Ads SDK.
            MobileAds.RaiseAdEventsOnUnityMainThread = true;
            MobileAds.Initialize(initStatus =>
            {
                if (_useBannerAds && autoLoadBanner)
                {
                    LoadBannerAd();
                }

                LoadInterstitialAd();
                LoadRewardedAd();
            });

            if (_useOpenAds)
            {
                LoadOpenAd();
            }
        }


        #region Public Func

        public void DelayShowInter(float time)
        {
            DelayAction(time, () => { ShowInterstitialAd(); });
        }

        public void DelayShowReward(float time)
        {
            DelayAction(time, () => { ShowRewardedAd(); });
        }

        public void LoadRewardedAd()
        {
            #if UNITY_EDITOR
            return;
            #endif
            ShowMessage("Loading Rewarded Ad");

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
            RewardedAd.Load(_rewardAdId, adRequest, (RewardedAd ad, LoadAdError error) =>
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

        public void ShowRewardedAd(Action actionDone = null, Action actionFailed = null)
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

            _rewardedAd.Show((Reward reward) => { });
        }

        public void LoadInterstitialAd()
        {
            #if UNITY_EDITOR
            return;
            #endif
            ShowMessage("Loading Interstitial Ad");

            // Clean up the old ad before loading a new one.
            if (_interstitialAd != null)
            {
                _interstitialAd.Destroy();
                _interstitialAd = null;
            }

            // create our request used to load the ad.
            var adRequest = new AdRequest();

            // send the request to load the ad.
            InterstitialAd.Load(_interAdId, adRequest, (InterstitialAd ad, LoadAdError error) =>
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

        public void ShowInterstitialAd(Action actionDone = null, Action actionFailed = null)
        {
            #if UNITY_EDITOR
            actionDone?.Invoke();

            return;
            #endif

            if (!HasInterstitial)
            {
                actionFailed?.Invoke();

                return;
            }

            _actionDoneInterstitial = actionDone;
            _interstitialAd.Show();
        }

        public void LoadBannerAd()
        {
            if (!_useBannerAds)
                return;
            #if UNITY_EDITOR
            return;
            #endif

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
            if (!_useBannerAds)
                return;
            #if UNITY_EDITOR
            return;
            #endif
            HasBanner = false;

            if (_bannerView == null)
            {
                return;
            }

            _bannerView.Destroy();
            _bannerView = null;
        }

        public void ShowBannerAd()
        {
            if (!_useBannerAds)
                return;
            #if UNITY_EDITOR
            return;
            #endif
            _bannerView?.Show();
        }

        public void HideBannerAd()
        {
            #if UNITY_EDITOR
            return;
            #endif
            if (!_useBannerAds)
                return;
            _bannerView?.Hide();
        }

        public void LoadOpenAd()
        {
            if(!_useOpenAds) return;
            #if UNITY_EDITOR
            return;
            #endif

            if (_appOpenAd != null)
            {
                DestroyOpenAd();
            }

            var adRequest = new AdRequest();

            AppOpenAd.Load(_openAdId, adRequest, (AppOpenAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    DelayAction(0.15f,LoadOpenAd);
                    return;
                }
                

                _appOpenAd = ad;
                _expireTime = DateTime.Now + TIMEOUT;
                RegisterEventHandlers(ad);
            });
        }

        public void ShowOpenAd(Action actionDone = null, Action actionFailed = null)
        {
            if(!_useOpenAds) return;
            #if UNITY_EDITOR
            actionDone?.Invoke();
            return;
            #endif
            // App open ads can be preloaded for up to 4 hours.
            if (_appOpenAd != null && _appOpenAd.CanShowAd() && DateTime.Now < _expireTime)
            {
                _actionDoneOpenAd = actionDone;
                _appOpenAd.Show();
            }
            else
            {
                actionFailed?.Invoke();
            }

        }

        public void DestroyOpenAd()
        {
            if(!_useOpenAds) return;
            #if UNITY_EDITOR
            return;
            #endif
            if (_appOpenAd == null)
            {
                return;
            }
            
            _appOpenAd.Destroy();
            _appOpenAd = null;

        }

        #endregion

        #region OpenAds

        private void RegisterEventHandlers(AppOpenAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += OnOpenAdPaid;
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += OnOpenAdImpressionRecorded;
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += OnOpenAdClicked;
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += OnOpenAdFullScreenContentOpened;
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += OnOpenAdFullScreenContentClosed;
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += OnOpenAdFullScreenContentFailed;
        }

        private void OnOpenAdPaid(AdValue obj)
        {
        }

        private void OnOpenAdImpressionRecorded()
        {
        }

        private void OnOpenAdClicked()
        {
        }

        private void OnOpenAdFullScreenContentOpened()
        {
        }

        private void OnOpenAdFullScreenContentClosed()
        {
            Action action = () =>
            {
                _actionDoneOpenAd?.Invoke();
                LoadOpenAd();
            };
            DelayAction(0.15f, action);
        }

        private void OnOpenAdFullScreenContentFailed(AdError obj)
        {
            DelayAction(0.15f, LoadOpenAd);
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
            _bannerView = new BannerView(_bannerAdId, AdSize.Banner, AdPosition.Bottom);
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
            HasBanner = true;
        }

        private void OnBannerAdLoadFailed(LoadAdError obj)
        {
            DelayAction(0.1f, LoadBannerAd);
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
            ShowMessage("Banner ad opened full screen content.");
        }

        private void OnBannerAdFullScreenContentClosed()
        {
            ShowMessage("Banner ad closed full screen content.");
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
        }

        private void OnInterAdClicked()
        {
        }

        private void OnInterAdFullScreenContentOpened()
        {
        }

        private void OnInterAdFullScreenContentClosed()
        {
            Action action = () =>
            {
                ShowMessage("Interstitial ad closed full screen content.");
                _actionDoneInterstitial?.Invoke();
                LoadInterstitialAd();
            };
            DelayAction(0.15f, action);
        }

        private void OnInterAdFullScreenContentFailed(AdError obj)
        {
            Action action = () =>
            {
                ShowMessage("Interstitial ad failed to open full screen content.");
                LoadInterstitialAd();
            };
            DelayAction(0.15f, action);
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
        }

        private void OnRewardAdClicked()
        {
        }

        private void OnRewardAdFullScreenContentOpened()
        {
        }

        private void OnRewardAdFullScreenContentClosed()
        {
            Action action = () =>
            {
                ShowMessage("Rewarded ad closed full screen content.");
                _actionDoneRewarded?.Invoke();
                LoadRewardedAd();
            };
            DelayAction(0.15f, action);
        }

        private void OnRewardAdFullScreenContentFailed(AdError obj)
        {
            Action action = () =>
            {
                ShowMessage("Rewarded ad failed to open full screen content.");
                LoadRewardedAd();
            };
            DelayAction(0.15f, action);
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


        #endif
    }
}