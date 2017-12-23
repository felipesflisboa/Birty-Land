using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Title panel who control sprite animations. Just toggle it on.
/// </summary>
public class TitlePanel : MonoBehaviour {
	[System.Serializable] 
	class AnimationGroup{
		[SerializeField] Transform [] animationTransformArray; 

		public bool Active{
			set{
				foreach(Transform animationTransform in animationTransformArray)
					animationTransform.gameObject.SetActive(value);
			}
		}
	}

	[SerializeField] Transform animationContainer;
	[SerializeField] AnimationGroup[] animationGroupArray;
	int index;
	Timer spriteTimer;

	const float SPRITE_INTERVAL = 1.6f;

	void Awake () {
		spriteTimer = new Timer(SPRITE_INTERVAL);

		// Speed up the sprites animation
		const float animationSpeed = 2;
		Animation[] animationArray = animationContainer.GetComponentsInChildren<Animation>(true);
		foreach(Animation anim in animationArray)
			foreach (AnimationState state in anim)
				state.speed = animationSpeed;
	}

	void OnEnable () {
		index = 0;
		spriteTimer.Reset();
		animationContainer.gameObject.SetActive(true);
		foreach(AnimationGroup animationGroup in animationGroupArray)
			animationGroup.Active = false;
	}

	void OnDisable(){
		if(animationContainer!=null) // Checked need because of quit game call.
			animationContainer.gameObject.SetActive(false);
	}

	void Update(){
		if(spriteTimer.CheckAndUpdate()){
			if(index==animationGroupArray.Length){
				foreach(AnimationGroup animationGroup in animationGroupArray)
					animationGroup.Active = false;
			}else{
				animationGroupArray[index].Active = true;
			}
			index = MathUtil.Repeat(index+1, animationGroupArray.Length+1);
		}
	}
}