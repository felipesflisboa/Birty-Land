using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Movable destructibles, like enemies and Player.
/// </summary>
public class Character : Destructible {
	[SerializeField] float baseSpeed;
	[SerializeField] Animation moveAnimation;

	protected bool isMoving{get; private set;}

	Timer refreshAnimationTimer;

	float _speed;
	public virtual float Speed{
		get{
			return _speed;
		}
		set{
			if(!Mathf.Approximately(Speed, value)){
				_speed = value;
				if(moveAnimation!=null)
					foreach (AnimationState state in moveAnimation)
						state.speed = Speed;
			}
		}
	}

	protected virtual bool CanAct{
		get{
			return GameManager.I.state == GameState.OCURRING && Alive;
		}
	}

	/* //remove
	Animation _animation;
	public Animation Animation {
		get {
			if (_animation == null)
				_animation = GetComponentInChildren<Animation> ();
			return _animation;
		}
	}
	*/

	public override void Initialize(){
		base.Initialize();
		isMoving = false;
		Speed = baseSpeed;
		if(moveAnimation!=null)
			refreshAnimationTimer = new Timer(0.25f);
	}

	protected override void Update() {
		base.Update();

		if(refreshAnimationTimer!=null && refreshAnimationTimer.CheckAndUpdate()){
			if(isMoving){
				if(!moveAnimation.isPlaying)
					moveAnimation.Play();
			}else{
				if(moveAnimation.isPlaying)
					moveAnimation.Stop();
			}
		}
	}

	protected override void FixedUpdate() {
		isMoving = false;
		base.FixedUpdate();
	}

	protected virtual void Move(Vector3 movement){
		if(movement != Vector3.zero){
			// Fixed delta time is used to standardize speed value with bullets and other stuff who also use speed.
			Rigidbody.MovePosition(transform.position + movement * Speed*Time.fixedDeltaTime);
			isMoving = true;
		}
	}
}