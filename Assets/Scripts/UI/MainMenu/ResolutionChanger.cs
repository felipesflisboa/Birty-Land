using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Allows resolution change.
/// </summary>
public class ResolutionChanger : MonoBehaviour {
    [Tooltip("Position 2 is 2 resolution button and so on. Position 0 is full resolution"), SerializeField]
    Button[] resolutionButtonByMultiplier = new Button[0];
    Vector2Int referenceResolution;

    Resolution FullResolution {
        get {
            return Screen.resolutions[Screen.resolutions.Length - 1]; // Last is the max size
        }
    }

	void Start () {
        CanvasScaler canvasScaler = FindObjectOfType<CanvasScaler>();
        referenceResolution = Vector2Int.RoundToInt(canvasScaler.referenceResolution);

        Vector2Int fullResolutionSize = new Vector2Int(Mathf.RoundToInt(FullResolution.height), Mathf.RoundToInt(FullResolution.width));
        // Check if the button resolution is valid. If wasn't, destroy it
        for (int i = 1; i < resolutionButtonByMultiplier.Length; i++) {
            if (resolutionButtonByMultiplier[i] == null)
                continue;
            Vector2Int resolutionSize = Vector2Int.RoundToInt(canvasScaler.referenceResolution*i);
            if (resolutionSize.x > fullResolutionSize.x || resolutionSize.y > fullResolutionSize.y) {
                Destroy(resolutionButtonByMultiplier[i].gameObject);
                resolutionButtonByMultiplier[i] = null;
            }
        }

        RefreshResolutionButtons();
    }

    /// <summary>
    /// Refresh current label for won't be interactable.
    /// </summary>
    void RefreshResolutionButtons() {
        int resolutionButtonByMultiplierIndex = 0;

#if !UNITY_EDITOR
        if (!Screen.fullScreen) 
            resolutionButtonByMultiplierIndex = Mathf.RoundToInt(Screen.height / referenceResolution.y);
#endif

        for (int i = 0; i < resolutionButtonByMultiplier.Length; i++) {
            if (resolutionButtonByMultiplier[i] != null)
                resolutionButtonByMultiplier[i].interactable = i != resolutionButtonByMultiplierIndex;
        }
    }

    /// <summary>
    /// Change windows resolution intto the multiplier chosen.
    /// </summary>
    /// <param name="multiplier"></param>
    public void SetResolution(int multiplier) {
#if !UNITY_EDITOR
        Screen.SetResolution(referenceResolution.x * multiplier, referenceResolution.y * multiplier, false);
        this.Invoke(new WaitForSeconds(0.05f), () => RefreshResolutionButtons()); // Wait for resolution refresh
#endif
    }

    public void SetFullScreenResolution() {
#if !UNITY_EDITOR
        Screen.SetResolution(FullResolution.width, FullResolution.height, true);
        this.Invoke(new WaitForSeconds(0.05f), () => RefreshResolutionButtons()); // Wait for resolution refresh
#endif
    }
}