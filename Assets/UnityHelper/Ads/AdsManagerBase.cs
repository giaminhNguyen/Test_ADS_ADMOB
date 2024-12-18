using System;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityHelper
{
    public abstract class AdsManagerBase : MonoBehaviour
    {
        protected bool _isAdsStatus;
        
        public abstract bool IsRewardReady();
        public abstract bool IsInterstitialReady();
        public abstract bool IsBannerReady();
        
        protected abstract void LoadRewardedAd();
        protected abstract void LoadInterstitialAd();
        protected abstract void LoadBannerAd();
        
        public abstract void ShowRewardedAd(Action actionDone = null, Action actionFailed = null);
        public void DelayShowRewardedAd(float delayTime,Action actionDone = null, Action actionFailed = null)
        {
            DelayAction(delayTime, (() => ShowRewardedAd(actionDone, actionFailed)));
        }
        
        public abstract void ShowInterstitialAd(Action actionDone = null, Action actionFailed = null);
        public void DelayShowInterstitialAd(float delayTime,Action actionDone = null, Action actionFailed = null)
        {
            DelayAction(delayTime, () => ShowInterstitialAd(actionDone, actionFailed));
        }
        
        public abstract void ShowBannerAd();
        public abstract void HideBannerAd();
        
        public void SetStatus(bool status)
        {
            _isAdsStatus = status;

            if (_isAdsStatus)
            {
                Ready();
            }
            else
            {
                NotReady();
            }
        }
        
        protected abstract void Ready();
        protected abstract void NotReady();
        
        #region Other Func

        protected async void DelayAction(float delayTime, Action action)
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
        
    }
}