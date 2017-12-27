using UnityEngine;

/// <summary>
/// Exit dialog showed on game pause menu.
/// </summary>
public class ExitDialog : MonoBehaviour {
    public void ShowExitDialog() {
        CanvasController.I.TogglePauseMenu();
        gameObject.SetActive(true);
    }

    public void CloseExitDialog() {
        CanvasController.I.TogglePauseMenu();
        gameObject.SetActive(false);
    }

    public void ConfirmExit() {
        gameObject.SetActive(false);
        GameManager.I.LoadScene("MainMenu");
    }
}