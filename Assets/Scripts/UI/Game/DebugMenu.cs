using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour {
	[SerializeField] Text hpLabel;	
	[SerializeField] Text attackLabel;	
	[SerializeField] Text speedLabel;

    PlayerLevelController levelSystem;
	HunterSpawnManager hunterSpawnManager;

	public bool Openend {
		get {
			return gameObject.activeSelf;
		}
	}

    void OnEnable(){
		if(GameManager.I.player != null) {
			if (levelSystem == null)
				levelSystem = GameManager.I.player.levelController;
			if (hunterSpawnManager==null)
				hunterSpawnManager = FindObjectOfType<HunterSpawnManager>();
		}
		if(levelSystem != null && hunterSpawnManager != null)
			Refresh();
	}

	void Update() {
		if (Time.timeScale != 0 && Openend)
			Close();
	}

	public void Show() {
		gameObject.SetActive(true);
	}

	public void Close() {
		gameObject.SetActive(false);
	}

	public void Toggle() {
		if (Openend)
			Close();
		else
			Show();
	}

	void Refresh(){
		hpLabel.text = string.Format("HP Lv{0} - {1}", levelSystem.hpLevel, levelSystem.CostForNextLevel(levelSystem.hpLevel));
		attackLabel.text = string.Format("Atk Lv{0} - {1}", levelSystem.attackLevel, levelSystem.CostForNextLevel(levelSystem.attackLevel));
		speedLabel.text = string.Format("Speed Lv{0} - {1}", levelSystem.speedLevel, levelSystem.CostForNextLevel(levelSystem.speedLevel));
        CanvasController.I.HUD.Refresh();
    }

    public void RaiseHPLevel() {
		levelSystem.RaiseHPLevel(true);
		OnLevelRaised();
	}

	public void RaiseAttackLevel() {
		levelSystem.RaiseAttackLevel(true);
		OnLevelRaised();
	}

	public void RaiseSpeedLevel(){
		levelSystem.RaiseSpeedLevel(true);
		OnLevelRaised();
	}

	public void DestroyDestructiblesInRange() {
		UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
		GameManager.I.DestroyDestructiblesInRange();
	}

	public void AddOneMinute() {
		AddTime(60, false);
	}

	public void AddOneMinuteSkippingHunter() {
		AddTime(60, true);
	}

	void AddTime(float time, bool skipHunter) {
		GameManager.I.AddExtraTime(time);
		if (skipHunter)
			hunterSpawnManager.MoveIndexToNext();
		else
			hunterSpawnManager.SpawnHunter();
		UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
		GameManager.I.debugUsed = true;
		Refresh();
	}

	void OnLevelRaised() {
		UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
		GameManager.I.debugUsed = true;
		Refresh();
	}
}
