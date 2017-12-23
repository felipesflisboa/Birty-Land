﻿/// <summary>
/// Timed racing game example for ScoreList. Values are stored as seconds.
/// </summary>
public class ScoreListTimed : ScoreListInt{
	protected override string KeyLabel{
		get{
			return "BirtyLand4";
		}
	}
	public override string GetStringAsValue(int value){
		return string.Format("{0}:{1:00}",value / 60, value % 60);
	}
}