using UnityEngine;

/// <summary>
/// Fix an object global rotation on Late Update.
/// Version 1.1
/// </summary>
public class FreezeRotation : MonoBehaviour {
	public bool retainOriginalPosition = true;
	[Tooltip("If !retainOriginalPosition")] public Vector3 globalEuler;
	Quaternion globalRotation;

	void Awake () {
		globalRotation = retainOriginalPosition ? transform.rotation : Quaternion.Euler(globalEuler);
	}

	void LateUpdate () {
		// Check to ignore small changes, since small Quaternion change are ignored in this equality operator
		if(transform.rotation != globalRotation)
			transform.rotation = globalRotation;
	}
}
