using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Allows resolution change.
/// </summary>
public class ResolutionChanger : MonoBehaviour {
    Vector2Int referenceResolution;

	void Start () {
        CanvasScaler canvasScaler = FindObjectOfType<CanvasScaler>();
        referenceResolution = Vector2Int.RoundToInt(canvasScaler.referenceResolution);
    }

    /// <summary>
    /// Change windows resolution intto the multiplier chosen.
    /// </summary>
    /// <param name="multiplier"></param>
    public void SetResolution(int multiplier) {
#if !UNITY_EDITOR
        Screen.SetResolution(referenceResolution.x * multiplier, referenceResolution.y * multiplier, false);
#endif
    }

    public void SetFullScreenResolution() {
#if !UNITY_EDITOR
        Resolution lastResolution = Screen.resolutions[Screen.resolutions.Length - 1]; // Last is the max size
        Screen.SetResolution(lastResolution.width, lastResolution.height, true);
#endif
    }
}