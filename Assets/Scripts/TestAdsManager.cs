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

        private void Update()
        {
            _rewardStatus.text = AdsManager_ADMOB.Instance.HasRewardedVideo ? "Ready" : "Not Ready";
            _interStatus.text = AdsManager_ADMOB.Instance.HasInterstitial ? "Ready" : "Not Ready";
            _bannerStatus.text = AdsManager_ADMOB.Instance.HasBanner ? "Ready" : "Not Ready";
        }

        public void ShowInterstitial()
        {
            AdsManager_ADMOB.Instance.ShowInterstitialAd();
        }
        
        public void ShowReward()
        {
            AdsManager_ADMOB.Instance.ShowRewardedAd();
        }
        
        public void LoadBanner()
        {
            AdsManager_ADMOB.Instance.LoadBannerAd();
        }

        public void DestroyBanner()
        {
            AdsManager_ADMOB.Instance.DestroyBannerView();
        }

        public void LoadNative()
        {
            
        }
        
        public void DestroyNative()
        {
            
        }
        
    }
}