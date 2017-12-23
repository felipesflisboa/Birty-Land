using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach this to a Text to make a frames/second indicator.
///
/// It calculates frames/second over each updateInterval, so the display does not keep changing wildly.
///
/// It is also fairly accurate at very low FPS counts (<10).
/// We do this not by simply counting frames per interval, but by accumulating FPS for each frame. This way we end up with
/// correct overall FPS even if the interval renders something like 5.5 frames.
/// 
/// Updated to new Unity UI.
/// </summary>
[RequireComponent( typeof( Text ) )]
public class FPSHUD : MonoBehaviour{
	public  float updateInterval = 0.5f;

	private Text  text;
	private float accum   = 0; // FPS accumulated over the interval
	private int   frames  = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval

	void Awake(){
		Application.targetFrameRate = 60;
		useGUILayout = false;
	}

	void Start(){
		text = GetComponent<Text>();
		if( !text ){
	        Debug.Log("UtilityFramesPerSecond needs a Text component!");
	        enabled = false;
	        return;
	    }
	    timeleft = updateInterval;
	}

	void Update(){
	    timeleft -= Time.deltaTime;
	    accum += Time.timeScale/Time.deltaTime;
	    ++frames;

	    // Interval ended - update GUI text and start new interval
	    if( timeleft <= 0.0 ){
	        // display two fractional digits (f2 format)
		    float fps = accum/frames;
		    string format = System.String.Format("{0:F2} FPS",fps);
			text.text = format;

		    if( fps < 20 ){
				text.color = Color.yellow;
			}else{
		        if( fps < 10 )
					text.color = Color.red;
		        else
					text.color = Color.green;
	
		        timeleft = updateInterval;
		        accum = 0.0F;
		        frames = 0;
		    }
		}
	}
}