using UnityEngine;

/// <summary>
/// Score list float implementation.
/// </summary>
public abstract class ScoreListFloat : ScoreList<float>{
	protected override float DefaultValue{
		get{
			return Asc ? 999999999 : 0;
		}
	}

	protected override void SaveSingleValue(string key, float val){
		PlayerPrefs.SetFloat(key,val);
	}

	protected override float LoadSingleValue(string key){
		return PlayerPrefs.GetFloat(key,DefaultValue);
	}
}