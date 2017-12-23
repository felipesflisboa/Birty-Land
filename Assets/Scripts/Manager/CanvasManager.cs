using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : SingletonMonoBehaviour<CanvasManager> {
	[SerializeField] RectTransform gameOverRect;
	[SerializeField] RectTransform pauseRect;
	[SerializeField] RectTransform pauseAlertRect;
	internal HUD hud;

	internal bool pauseAlertInitialized{get; private set;}
	bool pauseAlertActive;
	Timer pauseAlertTwinkleTimer;

	Canvas _canvas;
	public Canvas Canvas {
		get {
			if (_canvas == null)
				_canvas = GetComponent<Canvas> ();
			return _canvas;
		}
	}

	void Awake(){
		hud = GetComponentInChildren<HUD>();
	}

	void Start(){
		if(gameOverRect!=null)
			gameOverRect.gameObject.SetActive(false);
		if(pauseRect!=null)
			pauseRect.gameObject.SetActive(false);
		if(pauseAlertRect!=null)
			pauseAlertRect.gameObject.SetActive(false);
		pauseAlertTwinkleTimer = new Timer(1f);
	}

	void Update(){
		if(pauseAlertActive && pauseAlertTwinkleTimer.CheckAndUpdate()){
			pauseAlertRect.gameObject.SetActive(!pauseAlertRect.gameObject.activeSelf);
		}
	}

	public void DisplayGameOverMenu (){
		gameOverRect.gameObject.SetActive(true);
	}

	public void TogglePauseMenu (){
		pauseRect.gameObject.SetActive(!pauseRect.gameObject.activeSelf);
		if(pauseAlertActive){
			pauseAlertActive = false;
			pauseAlertRect.gameObject.SetActive(false);
		}
	}

	public void InitializePauseAlert(){
		pauseAlertInitialized = true;
		pauseAlertActive = true;
	}
}