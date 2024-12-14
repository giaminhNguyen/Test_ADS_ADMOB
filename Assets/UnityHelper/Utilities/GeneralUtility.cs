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
        
    }
}