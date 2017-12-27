using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for movable destructibles, like enemies and Player.
/// </summary>
public class Character : Destructible {
	[SerializeField] float baseSpeed;
	[Tooltip("Animator with a single move animation as default"), SerializeField] Animator moveAnimation;
    [SerializeField] float moveAnimationMultiplier = 1f;

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
                if (moveAnimation != null)
                    moveAnimation.speed = Speed* moveAnimationMultiplier;
			}
		}
	}

	protected virtual bool CanAct{
		get{
			return GameManager.I.state == GameState.OCURRING && Alive;
		}
	}

	protected override void Awake(){
		base.Awake();
		isMoving = false;
        if(baseSpeed!=0f)
		    Speed = baseSpeed;
		if(moveAnimation!=null)
			refreshAnimationTimer = new Timer(0.25f);
	}

	protected override void Update() {
		base.Update();

		if(refreshAnimationTimer!=null && refreshAnimationTimer.CheckAndUpdate()){
            bool changed = moveAnimation.enabled != isMoving;
            if(changed)
                moveAnimation.enabled = isMoving;
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