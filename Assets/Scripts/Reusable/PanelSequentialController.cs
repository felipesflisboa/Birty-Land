using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Sample script for navigating into sequential subpanels by pressing a game button.
/// Just put this script into a panel and it automatic detect and organize the subpanels. 
/// </summary>
public class PanelSequentialController : MonoBehaviour {
	[SerializeField] string sceneToLoadWhenEndName; 
	[SerializeField] string buttonForwardName = "Fire1"; 
	[SerializeField] string buttonBackwardName; 
	[SerializeField] float clickCooldownMin = 0.25f; // Cooldown min between clicks.

	RectTransform[] panelArray;
	int index;
	Timer clickCooldownTimer;

	void Start () {
		clickCooldownTimer = new Timer(clickCooldownMin);

		// Try to get all childs as panels
		List<RectTransform> panelList = new List<RectTransform>();
		foreach(Transform child in transform){
			RectTransform subpanel = child as RectTransform;
			if(subpanel!=null)
				panelList.Add(subpanel);
		}
		panelArray = panelList.ToArray();

		RefreshPanels();
	}

	void Update(){
		bool goFoward = buttonForwardName!="" && Input.GetButtonDown(buttonForwardName) && clickCooldownTimer.CheckAndUpdate();
		if(goFoward){
			index++;
			bool goToNextScene = index == panelArray.Length;
			if(goToNextScene)
				UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoadWhenEndName);
			else
				RefreshPanels();
		}
		bool goBackward = buttonBackwardName!="" && Input.GetButtonDown(buttonBackwardName) && clickCooldownTimer.CheckAndUpdate();
		if(goBackward){
			if(index>0){
				index--;
				RefreshPanels();
			}
		}
	}

	void RefreshPanels(){
		for(int i=0;i<panelArray.Length;i++){
			panelArray[i].gameObject.SetActive(i==index);
		}
	}
}