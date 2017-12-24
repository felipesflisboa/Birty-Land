using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Paged text.
/// </summary>
public class InfoPaged : MonoBehaviour {
	public RectTransform[] panelArray;
	int index;
	Timer clickCooldownTimer;
	const float CLICK_INTERVAL = 0.25f;

	void Awake () {
		clickCooldownTimer = new Timer(CLICK_INTERVAL);
	}

	void OnEnable () {
		index = 0;
		RefreshPanels();
		clickCooldownTimer.Reset();
	}

	void Update(){
		bool goFoward = Input.GetButtonDown("Fire1") && clickCooldownTimer.CheckAndUpdate();
		if(goFoward){
			index++;
			bool goToNextScene = index == panelArray.Length;
			if(goToNextScene)
				MainMenuManager.I.EnablePanel(MainMenuPanelType.TITLE);
			else
				RefreshPanels();
		}
	}

	void RefreshPanels(){
		for(int i=0;i<panelArray.Length;i++){
			panelArray[i].gameObject.SetActive(i==index);
		}
	}
}