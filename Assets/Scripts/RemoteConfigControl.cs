using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;
using UnityHelper;

public class RemoteConfigControl : Singleton<RemoteConfigControl>
{
    #region Properties
    public Action OnFetchDone;
    public bool   isDataFetched = false;

    public    float  ad_inter_interval     = 25;
    public    bool is_resume_ad          = false;
    public    bool is_ads                = false;
    protected bool isFirebaseInitialized = false;
    #endregion
    

    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

    // Start is called before the first frame update
    public override void Awake()
    {
        InitializeFirebase();
    }

    private void OnEnable()
    {
        this.RegisterListener(EventID.OnRetryCheckInternet, OnRetryCheckInternetHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance.RemoveListener(EventID.OnRetryCheckInternet, OnRetryCheckInternetHandle);
    }
    
    private void OnRetryCheckInternetHandle(object obj)
    {
        InitializeFirebase();
    }

    public void InitializeFirebase()
    {
        //LoadData();
        Dictionary<string, object> defaults =
            new Dictionary<string, object>
            {
                {"ad_inter_interval", ad_inter_interval},
                {"is_resume_ad", is_resume_ad ? 1 : 0},
                {"rating_popup", is_ads ? 1 : 0},
            };

        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);

        Debug.Log("RemoteConfig configured and ready!");

        isFirebaseInitialized = true;
        FetchDataAsync();
    }

    public void FetchDataAsync()
    {
        Debug.Log("Fetching data...");
        var fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
            TimeSpan.Zero);
        fetchTask.ContinueWithOnMainThread(FetchComplete);
    }

    private void FetchComplete(Task fetchTask)
    {
        if (fetchTask.IsCanceled)
        {
            Debug.Log("Fetch canceled.");
        }
        else if (fetchTask.IsFaulted)
        {
            Debug.Log("Fetch encountered an error.");
        }
        else if (fetchTask.IsCompleted)
        {
            Debug.Log("Fetch completed successfully!");
        }

        var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
        switch (info.LastFetchStatus)
        {
            case Firebase.RemoteConfig.LastFetchStatus.Success:
                Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                Debug.Log($"Remote data loaded and ready (last fetch time {info.FetchTime}).");
                Invoke(nameof(ReflectProperties),2);
                break;
            case Firebase.RemoteConfig.LastFetchStatus.Failure:
                switch (info.LastFetchFailureReason)
                {
                    case Firebase.RemoteConfig.FetchFailureReason.Error:
                        Debug.Log("Fetch failed for unknown reason");
                        break;
                    case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                        Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                        break;
                }

                break;
            case Firebase.RemoteConfig.LastFetchStatus.Pending:
                Debug.Log("Latest Fetch call still pending.");
                break;
        }
        OnFetchDone?.Invoke();
    }
    

    private void ReflectProperties()
    {
        var data = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance;
        ad_inter_interval = (float)data.GetValue("ads_interval").DoubleValue;
        is_ads            = (bool)data.GetValue("rating_popup").BooleanValue;
        is_resume_ad      = (bool)data.GetValue("is_resume_ad").BooleanValue;
        isDataFetched     = true;
        this.PostEvent(EventID.FetchRemoteConfigComplete);
    }
}