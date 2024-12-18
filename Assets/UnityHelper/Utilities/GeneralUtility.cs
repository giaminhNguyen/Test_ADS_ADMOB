using UnityEngine;

namespace UnityHelper
{
    public static class GeneralUtility
    {
        public static bool IsConnectedInternet (this MonoBehaviour mono)
        {
            return Application.internetReachability switch
            {
                    NetworkReachability.ReachableViaLocalAreaNetwork   => true,
                    NetworkReachability.ReachableViaCarrierDataNetwork => true,
                    _                                                  => false
            };
        }
        
        /// <summary>
        /// Kiểm tra xem ứng dụng đang chạy trên UnityEditor hay không.
        /// </summary>
        public static bool IsUnityEditor()
        {
            var check = false;
            #if UNITY_EDITOR
            check = true;
            #endif
            return check;
        }
        
        /// <summary>
        /// Kiểm tra xem ứng dụng đang chạy trên UnityEditor hay không.
        /// </summary>
        public static bool IsUnityEditor(this MonoBehaviour mono)
        {
            var check = false;
            #if UNITY_EDITOR
            check = true;
            #endif
            return check;
        }

        
    }
}