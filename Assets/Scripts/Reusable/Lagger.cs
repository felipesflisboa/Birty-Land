using UnityEngine;
using System.Collections;

/// <summary>
/// Lags the game. Used to test low FPS
/// </summary>
public class Lagger : MonoBehaviour {
	public float lagPerFrame = 0.01f;

#if UNITY_EDITOR // Make sure that this script won't run outside of Editor
	System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

	void Update () {
		stopWatch.Reset();
		stopWatch.Start();
		while(stopWatch.ElapsedMilliseconds<lagPerFrame*1000){
			Mathf.Sqrt(8192); // Do a Random operation
		}
	}
#endif
}