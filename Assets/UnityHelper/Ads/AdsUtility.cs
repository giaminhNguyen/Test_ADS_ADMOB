namespace UnityHelper
{
    public static class AdsUtility
    {
        //#if USE_IRON_SOURCE
        public static float lastTimeShowReward  = 0;
        public static float lastTimeShowInter = 0;
        public static bool  hasReward         = ADSManager_IS.Instance.HasRewardedVideo;
        public static bool  hasInter          = ADSManager_IS.Instance.HasInterstitial;
        public static bool  hasBanner         = ADSManager_IS.Instance.HasBanner;
        public static int   interCount        = 0;
        public static int   rewardCount       = 0;

        // #if USE_ADS_ADMOB
        public static float lastTimeShowAdmobInter   = 0;
        public static float lastTimeShowAdmobReward  = 0;
        public static float lastTimeShowAdmobOpenAds = 0;
        public static bool  hasAdmobReward           = AdsManager_ADMOB_1.instance.IsRewardReady();
        public static bool  hasAdmobInter            = AdsManager_ADMOB_1.instance.IsInterstitialReady();
        public static bool  hasAdmobBanner           = AdsManager_ADMOB_1.instance.IsBannerReady();
        public static bool  hasOpenAd               = AdsManager_ADMOB_1.instance.IsOpenAdReady();
        public static int   admobInterCount          = 0;
        public static int   admobRewardCount         = 0;
    }
}