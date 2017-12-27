using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
	[SerializeField] protected Text hpText;
	[SerializeField] protected Text scoreText;
	[SerializeField] protected Text timeText;
	protected Timer refreshTimer;

	const float REFRESH_INTERVAL = 0.25f;

	protected virtual void Start(){
		refreshTimer = new Timer(REFRESH_INTERVAL);
		Refresh();
	}

	protected virtual void Update(){
		if(refreshTimer.Check())
			Refresh();
	}

	public virtual void Refresh(){
		if(GameManager.I.state == GameState.BEFORE_START)
			return;

		refreshTimer.Reset();

		if(hpText != null){
			hpText.text = string.Format("{0}|{1}", GameManager.I.player.hp, GameManager.I.player.maxHP);
		}
		if(scoreText != null){
			scoreText.text = string.Format("{0}", GameManager.I.points);
		}
		if(timeText != null){
			int totalSeconds = GameManager.I.SecondsInt;
			timeText.text = string.Format("{0}:{1:00}",totalSeconds / 60, totalSeconds % 60);
		}
	}
}