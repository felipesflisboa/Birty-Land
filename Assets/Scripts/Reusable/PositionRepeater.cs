using UnityEngine;

/// <summary>
/// Automatic repeat the position if it surpass the repeatValue informed.
/// Good for parallax backgrounds.
/// </summary>
public class PositionRepeater : MonoBehaviour {
    public bool useLocalPosition;
    public bool useLateUpdate = true;
    public Vector3 repeatValue; // Uses absolute value, so only put positive numbers.
    public Vector3 bonusBeforeCalculation; // Value applied and unapplied before and after the calculation.

    void Awake() {
        // Safe check for negative values.
        bool valid = true;
        for (int i = 0; i < 3; i++) {
            if (repeatValue[i]<0f) {
                valid = false;
                break;
            }
        }
        if(!valid)
            Debug.LogErrorFormat("PositionRepeater.repeatValue was invalid({0}).", repeatValue);
    }

    void Update () {
        if (!useLateUpdate)
            CheckAndApply();
    }
	
	void LateUpdate () {
        if (useLateUpdate)
            CheckAndApply();
    }

    /// <summary>
    /// Apply the repeat. Returns true if any axis was applied.
    /// </summary>
    public bool CheckAndApply() {
        bool somethingChanged = false;
        Vector3 position = useLocalPosition ? transform.localPosition : transform.position;
        position += bonusBeforeCalculation;
        for (int i = 0; i < 3; i++) {
            if (repeatValue[i] != 0f && Mathf.Abs(position[i]) > repeatValue[i]) {
                position[i] -= Mathf.Sign(position[i]) * repeatValue[i];
                somethingChanged = true;
            }
        }

        if (!somethingChanged)
            return false;

        position -= bonusBeforeCalculation;
        if (useLocalPosition)
            transform.localPosition = position;
        else
            transform.position = position;

        return true;
    }
}