using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseAlert : MonoBehaviour {
	internal bool initialized{get; private set;}
	bool active;
	Timer twinkleTimer;

	CanvasGroup _canvasGroup;
	public CanvasGroup CanvasGroup {
		get {
			if (_canvasGroup == null)
				_canvasGroup = GetComponent<CanvasGroup> ();
			return _canvasGroup;
		}
	}

	void Start(){
		twinkleTimer = new Timer(1f);
	}

	public void Initialize(){
		initialized = true;
		active = true;
		gameObject.SetActive(true);
	}

	void Update(){
		if(active && twinkleTimer.CheckAndUpdate()){
			CanvasGroup.alpha = CanvasGroup.alpha==0f ? 1f : 0f;
		}
	}

	void OnDisable(){
		active = false;
	}
}