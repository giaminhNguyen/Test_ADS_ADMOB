#if USE_FIREBASE_REMOTE
using System;
using System.Collections.Generic;
using Firebase.RemoteConfig;
using UnityEngine;
#endif
public class RemoteConfigProperties : Singleton<RemoteConfigProperties>
{
    #if USE_FIREBASE_REMOTE

    #region Properties

    public float ad_inter_interval { get;private set; }
    public bool  is_resume_ad      { get;private set; }
    public bool  is_ads            { get;private set; }

    #endregion

    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad();
        ad_inter_interval = 25;
        is_resume_ad      = false;
        is_ads            = false;
    }

    public Dictionary<string,object> GetDefaultValues()
    {
        return new()
        {
                { "ad_inter_interval", ad_inter_interval },
                { "is_resume_ad", is_resume_ad ? 1 : 0 },
                { "is_ads", is_ads ? 1 : 0 }
        };
    }

    public void RefreshValues()
    {
        var data = FirebaseRemoteConfig.DefaultInstance;
        ad_inter_interval = (float) data.GetValue("ad_inter_interval").DoubleValue;
        is_resume_ad      = data.GetValue("is_resume_ad").BooleanValue;
        is_ads            = data.GetValue("is_ads").BooleanValue;
    }
    
    #endif
}
