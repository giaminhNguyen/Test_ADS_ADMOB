using System;
using UnityEngine;
using UnityEngine.UI;
using UnityHelper;

namespace DefaultNamespace
{
    public class TestAdsManager : MonoBehaviour
    {
        [SerializeField]
        private Text _rewardStatus;

        [SerializeField]
        private Text _interStatus;

        [SerializeField]
        private Text _bannerStatus;
        
        [SerializeField]
        private Text _nativeSatus;
        
        [SerializeField]
        private Text _openAdSatus;

        [SerializeField]
        private Text _isAds;
        
        [SerializeField]
        private Text _isResumeAd;
        
        [SerializeField]
        private Text _adInterInterval;
        

        private void OnEnable()
        {
            this.RegisterListener(EventID.FetchRemoteConfigComplete, OnFetchRemoteConfigCompleteHandle);
        }
        
        private void OnDisable()
        {
            EventDispatcher.Instance.RemoveListener(EventID.FetchRemoteConfigComplete, OnFetchRemoteConfigCompleteHandle);
        }

        private void OnFetchRemoteConfigCompleteHandle(object obj)
        {
            ShowMessage("Fetch Remote Config Complete");
            #if USE_FIREBASE_REMOTE
            _isAds.text           = RemoteConfigProperties.Instance.is_ads ? "True" : "False";
            _isResumeAd.text      = RemoteConfigProperties.Instance.is_resume_ad ? "True" : "False";
            _adInterInterval.text = RemoteConfigProperties.Instance.ad_inter_interval.ToString();
            #endif
            
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

        private void Update()
        {
            _rewardStatus.text = AdsManager_ADMOB.Instance.HasRewardedVideo ? "Ready" : "Not Ready";
            _interStatus.text  = AdsManager_ADMOB.Instance.HasInterstitial ? "Ready" : "Not Ready";
            _bannerStatus.text = AdsManager_ADMOB.Instance.HasBanner ? "Ready" : "Not Ready";
            _openAdSatus.text  = AdsManager_ADMOB.Instance.HasOpenAds ? "Ready" : "Not Ready";
        }

        public void ShowInterstitial()
        {
            #if USE_ADS_ADMOB
            AdsManager_ADMOB.Instance.ShowInterstitialAd();
            #endif
            
        }
        
        public void ShowReward()
        {
            #if USE_ADS_ADMOB
            AdsManager_ADMOB.Instance.ShowRewardedAd();
            #endif
            
        }
        
        public void LoadBanner()
        {
            #if USE_ADS_ADMOB
            AdsManager_ADMOB.Instance.LoadBannerAd();
            #endif
            
        }

        public void ShowBanner()
        {
            #if USE_ADS_ADMOB
            AdsManager_ADMOB.Instance.ShowBannerAd();
            #endif
            
        }

        public void HideBanner()
        {
            #if USE_ADS_ADMOB
            AdsManager_ADMOB.Instance.HideBannerAd();
            #endif
            
        }

        public void DestroyBanner()
        {
            #if USE_ADS_ADMOB
            AdsManager_ADMOB.Instance.DestroyBannerView();
            #endif
            
        }

        public void LoadNative()
        {
            
        }
        
        public void ShowNative()
        {
            
        }
        
        public void HideNative(){}
        
        public void DestroyNative()
        {
            
        }
        
        public void LoadOpenAd()
        {
            #if USE_ADS_ADMOB
            AdsManager_ADMOB.Instance.LoadOpenAd();
            #endif
            
        }
        
        public void ShowOpenAd()
        {
            #if USE_ADS_ADMOB
            AdsManager_ADMOB.Instance.ShowOpenAd();
            #endif
            
        }

        public void DestroyOpenAd()
        {
            #if USE_ADS_ADMOB
            AdsManager_ADMOB.Instance.DestroyOpenAd();
            #endif
            
        }
        
    }
}