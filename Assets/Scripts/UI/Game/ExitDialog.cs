using UnityEngine;

/// <summary>
/// Exit dialog showed on game pause menu.
/// </summary>
public class ExitDialog : MonoBehaviour {
    bool reenablePauseButton;

    public void ShowExitDialog() {
        if (CanvasController.I.pauseButton.gameObject.activeSelf) {
            reenablePauseButton = true; // Only enable latter if was already enabled before dialog was showed.
            CanvasController.I.pauseButton.gameObject.SetActive(false);
        }
        CanvasController.I.TogglePauseMenu();
        gameObject.SetActive(true);
    }

    public void CloseExitDialog() {
        if (reenablePauseButton) {
            reenablePauseButton = false;
            CanvasController.I.pauseButton.gameObject.SetActive(true);
        }
        CanvasController.I.TogglePauseMenu();
        gameObject.SetActive(false);
    }

    public void ConfirmExit() {
        gameObject.SetActive(false);
        GameManager.I.BackToMainMenu();
    }
}