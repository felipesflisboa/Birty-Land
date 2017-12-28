using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

/// <summary>
/// Print a log of WebGL memory usage on an interval.
///
/// Needs WebGLMemoryStats on plugin folder. Only works when debug flag is on.
/// </summary>
public class WebGLMemoryLog : MonoBehaviour {
    public float interval;

    float nextPrintTime;

    #if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    public static extern uint GetTotalMemorySize();

    [DllImport("__Internal")]
    public static extern uint GetTotalStackSize();

    [DllImport("__Internal")]
    public static extern uint GetStaticMemorySize();

    [DllImport("__Internal")]
    public static extern uint GetDynamicMemorySize();

	void Update () {
		if(Time.time > nextPrintTime) {
            nextPrintTime = Time.time + interval;
        }
	}
    #endif
}
