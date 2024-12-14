#if USE_FIREBASE_REMOTE
using UnityEngine;
using System;
using System.Threading.Tasks;
using Firebase.RemoteConfig;
using UnityHelper;
using Firebase;
using Firebase.Extensions;
#endif

public class RemoteConfigControl : Singleton<RemoteConfigControl>
{
    #if USE_FIREBASE_REMOTE

    #region Properties

    public    Action OnFetchDone;
    public    bool   isDataFetched = false;
    protected bool             isFirebaseInitialized = false;
    private   DependencyStatus _dependencyStatus     = DependencyStatus.UnavailableOther;
    
    #endregion


    

    // Start is called before the first frame update
    public override void Awake()
    {
        DontDestroyOnLoad();
        FirebaseRemoteConfig.DefaultInstance.OnConfigUpdateListener
                += ConfigUpdateListenerEventHandler;
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

    private void OnDestroy()
    {
        FirebaseRemoteConfig.DefaultInstance.OnConfigUpdateListener
                -= ConfigUpdateListenerEventHandler;
    }

    private void ConfigUpdateListenerEventHandler(object sender, ConfigUpdateEventArgs args)
    {
        if (args.Error != RemoteConfigError.None) {
            Debug.Log($"Error occurred while listening: {args.Error}");
            return;
        }
        Debug.Log("Updated keys: " + string.Join(", ", args.UpdatedKeys));
        FetchDataAsync();
    }

    private void OnRetryCheckInternetHandle(object obj)
    {
        InitializeFirebase();
    }

    public void InitializeFirebase()
    {
        FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(RemoteConfigProperties.Instance.GetDefaultValues());

        isFirebaseInitialized = true;
        FetchDataAsync();
    }

    public void FetchDataAsync()
    {
        FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero)
                            .ContinueWithOnMainThread(FetchComplete);
    }

    private void FetchComplete(Task fetchTask)
    {
        var info = FirebaseRemoteConfig.DefaultInstance.Info;

        switch (info.LastFetchStatus)
        {
            case LastFetchStatus.Success:
                FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                Debug.Log($"Remote data loaded and ready (last fetch time {info.FetchTime}).");
                Invoke(nameof(ReflectProperties), 2);

                break;
            case LastFetchStatus.Failure:
                switch (info.LastFetchFailureReason)
                {
                    case FetchFailureReason.Error:
                        Debug.Log("Fetch failed for unknown reason");

                        break;
                    case FetchFailureReason.Throttled:
                        Debug.Log("Fetch throttled until " + info.ThrottledEndTime);

                        break;
                }

                break;
            case LastFetchStatus.Pending:
                Debug.Log("Latest Fetch call still pending.");

                break;
        }

        OnFetchDone?.Invoke();
    }


    private void ReflectProperties()
    {
        RemoteConfigProperties.Instance.RefreshValues();
        isDataFetched     = true;
        this.PostEvent(EventID.FetchRemoteConfigComplete);
    }
    #endif
}