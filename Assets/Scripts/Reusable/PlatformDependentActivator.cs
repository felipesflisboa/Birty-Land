using UnityEngine;

/// <summary>
/// Make GameObjects disabled if is unavailable in the platform.
/// </summary>
public class PlatformDependentActivator : MonoBehaviour {
	public enum Call{
		None=0, Awake, Start, Update, LateUpdate
	}

	public Call call = Call.Awake;
    public bool windows;
    public bool macOS;
    public bool linux;
	public bool android;
    public bool iOS;
    public bool webDesktop;
    public bool webMobile;

    void Awake () {
		if(call == Call.Awake && !Available())
			gameObject.SetActive(false);
	}

	void Start () {
		if(call == Call.Start && !Available())
			gameObject.SetActive(false);
	}

	void Update () {
		if(call == Call.Update && !Available())
            gameObject.SetActive(false);
	}

	void LateUpdate () {
		if(call == Call.LateUpdate && !Available())
			gameObject.SetActive(false);
	}

    bool Available() {
#if UNITY_STANDALONE_WIN
        return windows;
#elif UNITY_STANDALONE_OSX
        return macOS;
#elif UNITY_STANDALONE_LINUX
        return linux;
#elif UNITY_ANDROID
		return android;
#elif UNITY_IOS
        return iOS;
#elif UNITY_WEBGL
        return (webDesktop && !WebGLMobileCheck.IsMobile) || (webMobile && WebGLMobileCheck.IsMobile);
#else
		return false;
#endif
    }
}