using UnityEngine;

/// <summary>
/// Change a transform color over the time. Only works if the material has the property _Color, otherside ignores. 
/// If time==0f, change at start.
/// Version 4.
/// </summary>
public class SetColor : MonoBehaviour {
    public Color definedColor;
    public bool setInChildren = true;
    /// Not exact time! Use 0 for instant change.
    public float totalTime; 

	// Use this for initialization
	void Start () {
		if(totalTime==0f){
			Paint(transform, definedColor, setInChildren, 1f);
		    Destroy(this);
	    }
	}

	void Update () {
		float percentage01 = Time.deltaTime/totalTime;
		if(Paint(transform,definedColor, setInChildren, percentage01)){
			Destroy(this);
		}
	}

    /// <summary>
    /// Paint the transform and all of it children using color. Return if some color is changed.
    /// </summary>
	public bool Paint(Transform transformToPaint, Color color, bool setInChildren, float percentage01){
        bool finished = true;

		// Ignore objects and chidren with SetColor.
		SetColor transformSetColor = null;
		bool hasOtherSetColor = false;
		if(transformToPaint != transform){ // Ignores self component
			transformSetColor = transformToPaint.GetComponent<SetColor>();
			hasOtherSetColor = transformSetColor != null;
		}

		if(!hasOtherSetColor){
			Renderer transformRenderer = transformToPaint.GetComponent<Renderer>();
			if(transformRenderer != null){
				Material transformMaterial = transformRenderer.material;
				if(transformMaterial!= null){ 
					if(transformMaterial.HasProperty("_Color") && transformMaterial.color != color){
						transformMaterial.color = Color.Lerp(transformMaterial.color,color,percentage01);
						if(transformMaterial.color != color)
							finished = false;
					}
				}
			}
		}

		if(setInChildren && (!hasOtherSetColor || !transformSetColor.setInChildren)){
			foreach(Transform child in transformToPaint){
				if(!Paint(child,color,setInChildren, percentage01))
					finished = false;
			}
		}

	    return finished;
	}
}