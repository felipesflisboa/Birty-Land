using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

/// <summary>
/// ADS util class.
/// </summary>
public class ADSUtil : MonoBehaviour {
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
        System.Action<bool?> newCallback = (result) => callback();
        Show(newCallback);
    }

    /// <summary>
    /// Show a video ADS.
    /// </summary>
    /// <param name="callback">Callback. True if was viewed until the end, false if was skipped, or null if wasn't showed.</param>
    public static void Show(System.Action<bool?> callback) {
#if UNITY_ADS
        ShowOptions options = new ShowOptions();
        options.resultCallback = (result) => {
            switch (result) {
                case ShowResult.Finished:   callback(true);     break;
                case ShowResult.Skipped:    callback(false);    break;
                case ShowResult.Failed:     callback(null);     break;
            }
        };
        Advertisement.Show(options);
#else
        Debug.LogError("Ads not supported on this platform!");
#endif
    }
}