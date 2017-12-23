using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {
	[SerializeField] Text hpLabel;	
	[SerializeField] Text attackLabel;	
	[SerializeField] Text speedLabel;

	PlayerLevelSystem levelSystem;

	void OnEnable(){
		if(levelSystem == null && GameManager.I.player != null)
			levelSystem = GameManager.I.player.levelSystem;
		if(levelSystem != null)
			Refresh();
	}

	void Refresh(){
		hpLabel.text = string.Format(
			"HP Lv{0}\nPay {1} Pts to increase", levelSystem.hpLevel, levelSystem.CostForNextLevel(levelSystem.hpLevel)
		);
		attackLabel.text = string.Format(
			"Attack Lv{0}\nPay {1} Pts to increase", levelSystem.attackLevel, levelSystem.CostForNextLevel(levelSystem.attackLevel)
		);
		speedLabel.text = string.Format(
			"Speed Lv{0}\nPay {1} Pts to increase", levelSystem.speedLevel, levelSystem.CostForNextLevel(levelSystem.speedLevel)
		);
		CanvasManager.I.hud.Refresh();
	}

	public void RaiseHPLevel(){
		if(levelSystem.RaiseHPLevel())
			OnLevelRaised();
		else
			OnLevelFailedRaised();
	}

	public void RaiseAttackLevel(){
		if(levelSystem.RaiseAttackLevel())
			OnLevelRaised();
		else
			OnLevelFailedRaised();
	}

	public void RaiseSpeedLevel(){
		if(levelSystem.RaiseSpeedLevel())
			OnLevelRaised();
		else
			OnLevelFailedRaised();
	}

	void OnLevelRaised(){
		//TODO good SFX
		// Remove selection to won't press again the button when unpausing
		UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
		Refresh();
	}

	void OnLevelFailedRaised(){
		//TODO bad SFX
		// Remove selection to won't press again the button when unpausing
		UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
	}
}