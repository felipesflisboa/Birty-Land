using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

/// <summary>
/// ADS util class.
/// </summary>
public class AdsUtil : MonoBehaviour {
    /// <summary>
    /// ADS Result.
    /// Override other frameworks enums to have total package independency.
    /// </summary>
    public enum Result {
        None = 0, Failed, Skipped, Finished
    }

    public static bool Supported {
        get {
#if UNITY_ADS
            return true;
#else
		    return false;
#endif
        }
    }

    public static bool IsReady {
        get {
#if UNITY_ADS
            return Advertisement.isSupported && Advertisement.IsReady();
#else
		    return false;
#endif
        }
    }

    public static void Show(System.Action callback) {
        System.Action<Result> newCallback = (result) => callback();
        Show(newCallback);
    }

    /// <summary>
    /// Show a video ADS.
    /// </summary>
    /// <param name="callback">Callback. True if was viewed until the end, false if was skipped, or null if wasn't showed.</param>
    public static void Show(System.Action<Result> callback) {
#if UNITY_ADS
        ShowOptions options = new ShowOptions();
        options.resultCallback = (result) => {
            switch (result) {
                case ShowResult.Finished:   callback(Result.Finished);  break;
                case ShowResult.Skipped:    callback(Result.Skipped);   break;
                case ShowResult.Failed:     callback(Result.Failed);    break;
            }
        };
        Advertisement.Show(options);
#else
        Debug.LogError("Ads not supported on this platform!");
#endif
    }
}