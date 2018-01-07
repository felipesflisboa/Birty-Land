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
        System.Action<ShowResult> newCallback = (result) => callback();
        Show(newCallback);
    }

    public static void Show(System.Action<ShowResult> callback) {
#if UNITY_ADS
        ShowOptions options = new ShowOptions();
        options.resultCallback = callback;
        Advertisement.Show(options);
#else
		Debug.LogError("Ads not supported on this platform!");
#endif
    }
}