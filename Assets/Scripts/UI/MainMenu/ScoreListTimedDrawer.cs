using UnityEngine;

public class ScoreListTimedDrawer : ScoreListDrawer<ScoreListTimed, int>{
	public float xGainAfterHalfValue;
	public float xGainAfterHalfRatio;

	public static int? lastScore;

	protected override int? LastScore{
		get{
			return lastScore;
		}
		set{
			lastScore = value;
		}
	}

	protected override Vector3 CalculateLocalPosition(int index){
		Vector3 ret = Vector3.zero;
		int yIndex = index;
		bool useTwoColumns = (xGainAfterHalfValue!=0f || xGainAfterHalfRatio!=0f) && index>=5;

		if(useTwoColumns){
			ret.x = xGainAfterHalfValue+Screen.height*xGainAfterHalfRatio;
			yIndex%=5;
		}
		ret.y = yGainValue*yIndex+Screen.height*yIndex*yGainRatio;
		return ret;
	}
}