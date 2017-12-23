using UnityEngine;

/// <summary>
/// Score list int implementation.
/// </summary>
public abstract class ScoreListInt : ScoreList<int>{
	protected override int DefaultValue{
		get{
			return Asc ? 999999999 : 0;
		}
	}

	protected override void SaveSingleValue(string key, int val){
		PlayerPrefs.SetInt(key,val);
	}

	protected override int LoadSingleValue(string key){
		return PlayerPrefs.GetInt(key,DefaultValue);
	}
}