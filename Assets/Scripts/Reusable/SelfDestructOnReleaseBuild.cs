using UnityEngine;

/// <summary>
/// Selftdestruct itself when wasn't on debug mode.
/// </summary>
public class SelfDestructOnReleaseBuild : MonoBehaviour {
	void Awake () {
        if (!Debug.isDebugBuild)
            Destroy(gameObject);
	}
}