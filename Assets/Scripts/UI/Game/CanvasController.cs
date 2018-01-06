using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handle the Canvas on game.
/// </summary>
public class CanvasController : SingletonMonoBehaviour<CanvasController> {
    [SerializeField] PauseMenu pauseMenu;
    [SerializeField] RectTransform gameOverRect;
    [SerializeField] RectTransform exitDialogRect;
    [SerializeField] RectTransform initialAlertRect;
    public Button pauseButton;
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

    void Start() {
        if (pauseMenu != null)
            pauseMenu.gameObject.SetActive(false);
        if (gameOverRect!=null)
			gameOverRect.gameObject.SetActive(false);
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

    public void TryToTogglePause() {
        if(!exitDialog.gameObject.activeSelf)
            GameManager.I.TryToTogglePause();
    }

    public void TogglePauseMenu (){
        pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeSelf);
		pauseAlert.gameObject.SetActive(false);
	}
}