using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Draw a Score List. Put on an empty object as pivot.
/// Version 4.1
/// </summary>
public abstract class ScoreListDrawer<TScoreList,TScoreListGeneric> : MonoBehaviour
	where TScoreListGeneric : struct, System.IComparable
	where TScoreList : ScoreList<TScoreListGeneric>, new()
{
	public GameObject textScorePrefab; // Y start and Y gain are pick on prefab Y position and height values 
	public float yGainValue;
	public float yGainRatio; // Based on total screen size
	public Text labelCurrentScore; // Current Score label to be show if exist
	public Text textCurrentScore; // Current Score text value to be show if exist
	public string newRecordFormatMessage = "{0} NEW RECORD!";
	public bool clearLastScoreAfterShowing;

	// Override this property with a static variable
	protected abstract TScoreListGeneric? LastScore{get;set;}

	protected virtual void Start (){
		Draw();
    }

	public void Restart(){
		Clear();
		Draw();
	}

	/// <summary>
	/// Draw the prefabs.
	/// </summary>
	public virtual void Draw(){
		// The Y start is the prefab Y position
		bool showedLastScore = false;
		TScoreList scoreList = new TScoreList();
		scoreList.Load();
		for(int i = 0;i<scoreList.Size;i++){
			Text textScore = Instantiate(textScorePrefab).GetComponent<Text>();
			textScore.transform.SetParent(transform, false);
			textScore.transform.localPosition = CalculateLocalPosition(i);
			textScore.text = string.Format("{0:00}. {1}", i+1, scoreList.GetString(i));
			// NEW RECORD Label		
			if (LastScore!=null && !showedLastScore){
				// Cast values to compare with Mathf.Approximately;
				float value = (float)System.Convert.ChangeType(scoreList.values[i], typeof(float));
				float lastScore = (float)System.Convert.ChangeType(LastScore, typeof(float));
				if(Mathf.Approximately(value, lastScore)){
					textScore.text = string.Format(newRecordFormatMessage, textScore.text);
					showedLastScore = true;
				}
			}
		}
		if(labelCurrentScore!=null && LastScore==null){
			labelCurrentScore.gameObject.SetActive(false);
		}
		if(textCurrentScore!=null){
			if(LastScore == null){
				textCurrentScore.gameObject.SetActive(false);
			}else{
				textCurrentScore.text = scoreList.GetStringAsValue((TScoreListGeneric)LastScore);
			}
		}
		if(clearLastScoreAfterShowing)
			LastScore = null;
	}

	/// <summary>
	/// Calculates the local position of each item.
	/// </summary>
	protected virtual Vector3 CalculateLocalPosition(int index){
		return Vector3.up*(yGainValue*index+Screen.height*index*yGainRatio);
	}

	/// <summary>
	/// Remove all childs.
	/// </summary>
	public virtual void Clear(){
		foreach(Transform child in transform){
			Destroy(child.gameObject);
		}
	}
}