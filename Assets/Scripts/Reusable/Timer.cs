using UnityEngine;
using System.Collections;

/// <summary>
/// Class for using Cooldowns.
/// To use, just initialize the Timer with the expected interval and check in CheckAndUpdate that also updates the timer.
/// Version 1.2
/// </summary>
public class Timer {
    public bool useTimeSinceLevelLoad;
    public float interval;
	public System.Func<float> currentTimeFunc;
    float timeDestiny;

    public bool UseTimeSinceLevelLoad{
        set{ // Resets the timer on set.
            useTimeSinceLevelLoad = value;
            Reset();
        }
        get{
            return useTimeSinceLevelLoad;
        }
    }

    /// <summary>
    /// Returns if timer is running.
    /// </summary>
    public bool IsTimerRunning{
        get{
            return timeDestiny>0f;
        }
    }

    float Time{
        get{
			return currentTimeFunc==null ? UnityEngine.Time.timeSinceLevelLoad : currentTimeFunc();
        }
    }

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="pInterval">Time inteval expect between the cooldown.</param>
    /// <param name="pReturnFirstCheckAsTrue">If <c>true</c>, in first CheckTime() returns true. </param>
	/// <param name="pCurrentTimeFunc">A closure to return the time. If isn't informed, uses UnityEngine.Time.timeSinceLevelLoad.</param>
	public Timer (float pInterval, bool pReturnFirstCheckAsTrue=true, System.Func<float>pCurrentTimeFunc=null) {
        interval = pInterval;
		currentTimeFunc = pCurrentTimeFunc;
        if(!pReturnFirstCheckAsTrue)
            Reset();
	}

    /// <summary>
    /// Clear Timer marking the check as true.
    /// </summary>
    public void MarkDone(){
        timeDestiny=0f;
    }

    /// <summary>
    /// Resets timer.
    /// </summary>
    public void Reset(){
        timeDestiny=Time+interval;
    }

    /// <summary>
	/// Returns the remaining time for reaching at interval.
    /// </summary>
    public int RemainingSecondsAsInt(bool useNegatives=false){
		return Mathf.FloorToInt(RemainingSecondsAsFloat(useNegatives));
    }

	/// <summary>
	/// Returns the remaining time for reaching at interval.
    /// </summary>
	/// <param name="useNegatives">If <c>false</c>, returns 0f if suprass the mark. If <c>true</c>, returns the negatives of surpassed time.
    public float RemainingSecondsAsFloat(bool useNegatives=false){
        float ret = timeDestiny - Time;
        if(!useNegatives)
            ret = Mathf.Max(ret,0f);
        return ret;
    }

    /// <summary>
	/// Checks if time exceed the interval, and, if exceeds, resets the timer.
    /// </summary>
    public bool CheckAndUpdate(){
        bool ret = Check();
        if(ret)
            Reset(); 
        return ret;
    }

	/// <summary>
	/// Checks if time exceed the interval.
    /// </summary>
    public bool Check(){
        return timeDestiny <= Time;
    }
}