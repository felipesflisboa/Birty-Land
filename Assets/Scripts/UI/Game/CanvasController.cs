using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handle the Canvas on game.
/// </summary>
public class CanvasController : SingletonMonoBehaviour<CanvasController> {
	[SerializeField] RectTransform gameOverRect;
    [SerializeField] RectTransform pauseRect;
    [SerializeField] RectTransform exitDialogRect;
    [SerializeField] RectTransform initialAlertRect;
    public PauseAlert pauseAlert;
    public ExitDialog exitDialog;

    Canvas _canvas;
	public Canvas Canvas {
		get {
			if (_canvas == null)
				_canvas = GetComponent<Canvas> ();
			return _canvas;
		}
	}

	HUD _hud;
	public HUD HUD {
		get {
			if (_hud == null)
				_hud = GetComponentInChildren<HUD> ();
			return _hud;
		}
	}

	void Start(){
		if(gameOverRect!=null)
			gameOverRect.gameObject.SetActive(false);
		if(pauseRect!=null)
			pauseRect.gameObject.SetActive(false);
		if(pauseAlert!=null)
			pauseAlert.gameObject.SetActive(false);
        if (exitDialog != null)
            exitDialog.gameObject.SetActive(false);

        if (initialAlertRect != null) {
            initialAlertRect.gameObject.SetActive(true);
            this.Invoke(new WaitForSeconds(2.5f), () => initialAlertRect.gameObject.SetActive(false));
        }
    }

	public void DisplayGameOverMenu (){
		gameOverRect.gameObject.SetActive(true);
	}

	public void TogglePauseMenu (){
		pauseRect.gameObject.SetActive(!pauseRect.gameObject.activeSelf);
		pauseAlert.gameObject.SetActive(false);
	}
}