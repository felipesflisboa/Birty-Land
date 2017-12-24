using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Title animation who control sprite animations. Just toggle it on.
/// </summary>
public class TitleAnimation : MonoBehaviour {
	[SerializeField] Transform animationContainer;
	[SerializeField] Transform[] animationTransformArray; 
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
		foreach(Transform animationTransform in animationTransformArray)
			animationTransform.gameObject.SetActive(false);
	}

	void OnDisable(){
		if(animationContainer!=null) // Checked need because of quit game call.
			animationContainer.gameObject.SetActive(false);
	}

	void Update(){
		if(spriteTimer.CheckAndUpdate()){
			if(index==animationTransformArray.Length){
				foreach(Transform animationTransform in animationTransformArray)
					animationTransform.gameObject.SetActive(false);
			}else{
				animationTransformArray[index].gameObject.SetActive(true);
			}
			index = MathUtil.Repeat(index+1, animationTransformArray.Length+1);
		}
	}
}