using System;
using GoogleMobileAds.Api;
using UnityEngine;
using VInspector;

namespace UnityHelper
{
    public class AdsManager_ADMOB_1 : AdsManagerBase
    {
        #region Properties

        public static AdsManager_ADMOB_1 instance;
        
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
        private bool   _hasBanner = false;
        private bool _initialized = false;
        #endregion

        #region Unity Func

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion
        
        #region public Func
        
        public bool IsOpenAdReady()
        {
            bool check;
            
            #if UNITY_EDITOR
                check = true;
            #elif USE_ADS_ADMOB
                check = (_appOpenAd?.CanShowAd() ?? false) && DateTime.Now < _expireTime ;
            #endif
            
            return check && _useOpenAds;
        }
        
        public override bool IsRewardReady()
        {
            var check = false;
            #if UNITY_EDITOR
            check = true;
            #elif USE_ADS_ADMOB
                check = _rewardedAd?.CanShowAd() ?? false;
            #endif
                
            return check;
        }

        public override bool IsInterstitialReady()
        {
            var check = false;
                
            #if UNITY_EDITOR
            check = true;
            #elif USE_ADS_ADMOB
                check = _interstitialAd?.CanShowAd() ?? false;
            #endif
                
            return check;
        }

        public override bool IsBannerReady()
        {
            var check = _hasBanner;
                
            #if UNITY_EDITOR
            check = true;
            #endif
                
            return check;
        }
        
        public void ShowOpenAd(Action actionDone = null, Action actionFailed = null)
        {
            if (this.IsUnityEditor())
            {
                actionDone?.Invoke();
                return;
            }
            if(!_useOpenAds) return;
            #if USE_ADS_ADMOB
            if (_appOpenAd != null && _appOpenAd.CanShowAd() && DateTime.Now < _expireTime)
            {
                _actionDoneOpenAd = actionDone;
                _appOpenAd.Show();
            }
            else
            {
                actionFailed?.Invoke();
            }
            #endif
        }
        
        public override void ShowRewardedAd(Action actionDone = null, Action actionFailed = null)
        {
            if (this.IsUnityEditor())
            {
                actionDone?.Invoke();
                UpdateLastRewardTime();
            }
            #if USE_ADS_ADMOB
            if (!IsRewardReady())
            {
                actionFailed?.Invoke();

                return;
            }

            _actionDoneRewarded = actionDone;

            _rewardedAd.Show(_ => { });
            #endif
            
        }

        public override void ShowInterstitialAd(Action actionDone = null, Action actionFailed = null)
        {
            if (this.IsUnityEditor())
            {
                actionDone?.Invoke();
                UpdateLastInterTime();
                return;
            }

            #if USE_ADS_ADMOB
                if (!IsInterstitialReady())
                {
                    actionFailed?.Invoke();

                    return;
                }

                _actionDoneInterstitial = actionDone;
                _interstitialAd.Show();
            #endif
            
        }

        public override void ShowBannerAd()
        {
            if(this.IsUnityEditor() || !_useBannerAds)
                return;
            #if USE_ADS_ADMOB
            _bannerView?.Show();
            #endif
            
        }

        public override void HideBannerAd()
        {
            if(this.IsUnityEditor() || !_useBannerAds)
                return;
            #if USE_ADS_ADMOB
            _bannerView?.Hide();
            #endif
        }
        
        public void LoadOpenAd()
        {
            #if USE_ADS_ADMOB
            if (this.IsUnityEditor())
            {
                UpdateLastOpenAdsTime();
                return;
            }
            
            DestroyOpenAd();

            var adRequest = new AdRequest();

            AppOpenAd.Load(_openAdId, adRequest, (AppOpenAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    DelayAction(0.15f,LoadOpenAd);
                    return;
                }
                

                _appOpenAd  = ad;
                _expireTime = DateTime.Now + TIMEOUT;
                RegisterEventHandlers(ad);
            });
            #endif
            
        }

        public void DestroyOpenAd()
        {
            if (this.IsUnityEditor()) return;
            
            #if USE_ADS_ADMOB
            if(_appOpenAd == null) return;
            _appOpenAd.Destroy();
            _appOpenAd = null;
            #endif
            
        }
        
        public void DestroyBannerView()
        {
            if ( this.IsUnityEditor() || !_useBannerAds)
                return;

            _hasBanner = false;

            #if USE_ADS_ADMOB
            if (_bannerView == null)
            {
                return;
            }

            _bannerView.Destroy();
            _bannerView = null;
            #endif
        }

        #endregion
        
        private void UpdateLastRewardTime()
        {
            AdsUtility.lastTimeShowAdmobReward = Time.time;
        }
        
        private void UpdateLastInterTime()
        {
            AdsUtility.lastTimeShowAdmobInter = Time.time;
        }
        
        private void UpdateLastOpenAdsTime()
        {
            AdsUtility.lastTimeShowAdmobOpenAds = Time.time;
        }
        
        private void IncreaseRewardCount()
        {
            AdsUtility.admobRewardCount++;
        }
        
        private void IncreaseInterCount()
        {
            AdsUtility.admobInterCount++;
        }
        
        
        protected override void Ready()
        {
            #if USE_ADS_ADMOB
            if (this.IsUnityEditor())
            {
                return;
            }
            if (!_initialized)
            {
                MobileAds.RaiseAdEventsOnUnityMainThread = true;
                MobileAds.Initialize(initStatus =>
                {
                    _initialized                             = true;
                    if (_useBannerAds && autoLoadBanner)
                    {
                        LoadBannerAd();
                    }

                    LoadInterstitialAd();
                    LoadRewardedAd();
                    
                    if (_useOpenAds)
                    {
                        LoadOpenAd();
                    }
                });
            }else if (_useBannerAds)
            {
                ShowBannerAd();
            }
            #endif
            
        }
        
        protected override void NotReady()
        {
            #if USE_ADS_ADMOB
            if (this.IsUnityEditor())
            {
                return;
            }
            if (_useBannerAds)
            {
                HideBannerAd();
            }
            #endif
            
        }
        
        protected override void LoadRewardedAd()
        {
            #if USE_ADS_ADMOB
            if(this.IsUnityEditor()) return;
            
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
            #endif
        }
        

        protected override void LoadInterstitialAd()
        {
            #if USE_ADS_ADMOB
            if(this.IsUnityEditor()) return;
            ShowMessage("Loading Interstitial Ad");

            if (_interstitialAd != null)
            {
                _interstitialAd.Destroy();
                _interstitialAd = null;
            }

            var adRequest = new AdRequest();

            InterstitialAd.Load(_interAdId, adRequest, (ad, error) =>
            {
                if (error != null || ad == null)
                {
                    DelayAction(0.1f, LoadInterstitialAd);

                    return;
                }

                _interstitialAd = ad;
                InterstitialEvent(_interstitialAd);
            });
            #endif
        }

        

        protected override void LoadBannerAd()
        {
            #if USE_ADS_ADMOB
            if(this.IsUnityEditor()) return;
            if (_bannerView == null)
            {
                _bannerView = new BannerView(_bannerAdId, bannerSize,bannerPosition);
            }

            BannerAdEvents();

            var adRequest = new AdRequest();
            adRequest.Keywords.Add("unity-admob-sample");
            _bannerView.LoadAd(adRequest);
            #endif
            
        }
        #if USE_ADS_ADMOB
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
            UpdateLastOpenAdsTime();
        }

        private void OnOpenAdFullScreenContentClosed()
        {
            UpdateLastOpenAdsTime();
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
            UpdateLastRewardTime();
            IncreaseRewardCount();
        }

        private void OnRewardAdFullScreenContentClosed()
        {
            UpdateLastRewardTime();
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
            UpdateLastInterTime();
            IncreaseInterCount();
        }

        private void OnInterAdFullScreenContentClosed()
        {
            UpdateLastInterTime();
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
            _hasBanner = true;
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
        #endif
    }
}