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

        private void Start()
        {
            AdsManager_ADMOB_1.instance.SetStatus(true);
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
            _rewardStatus.text = AdsUtility.hasAdmobReward ? "Ready" : "Not Ready";
            _interStatus.text  = AdsUtility.hasAdmobInter ? "Ready" : "Not Ready";
            _bannerStatus.text = AdsUtility.hasAdmobBanner ? "Ready" : "Not Ready";
            _openAdSatus.text  = AdsUtility.hasOpenAd ? "Ready" : "Not Ready";
        }

        public void ShowInterstitial()
        {
            AdsManager_ADMOB_1.instance.ShowInterstitialAd();
            
        }
        
        public void ShowReward()
        {
            AdsManager_ADMOB_1.instance.ShowRewardedAd();
            
        }
        
        public void LoadBanner()
        {
            
        }

        public void ShowBanner()
        {
            AdsManager_ADMOB_1.instance.ShowBannerAd();
            
        }

        public void HideBanner()
        {
            AdsManager_ADMOB_1.instance.HideBannerAd();
            
        }

        public void DestroyBanner()
        {
            AdsManager_ADMOB_1.instance.DestroyBannerView();
            
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
            AdsManager_ADMOB_1.instance.LoadOpenAd();
            
        }
        
        public void ShowOpenAd()
        {
           AdsManager_ADMOB_1.instance.ShowOpenAd();
            
        }

        public void DestroyOpenAd()
        {
            AdsManager_ADMOB_1.instance.DestroyOpenAd();
        }
        
    }
}