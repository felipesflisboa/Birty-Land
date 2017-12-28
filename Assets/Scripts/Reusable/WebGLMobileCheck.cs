using UnityEngine;
using System.Runtime.InteropServices;

/// <summary>
/// Return if WebGL is running on mobile browser.
/// 
/// Needs WebGLMobileCheck.jslib on Plugins folder.
/// </summary>
public static class WebGLMobileCheck{
    static bool _initialized;
    static bool _isMobile;
    public static bool IsMobile {
        get {
            if (!_initialized) {
                _initialized = true;
                _isMobile = IsWebGLMobile();
            }
            return _isMobile;
        }
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    static extern bool IsWebGLMobile();
#else
    static bool IsWebGLMobile() { return false; }
#endif
}