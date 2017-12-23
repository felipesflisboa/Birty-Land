using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic enemies.
/// </summary>
public class Creep : Character {
	[Header("Creep")]
	[SerializeField] int collisionDamage;
	[Tooltip("Stops Collision damage after this time"), SerializeField] float collisionCooldown;
	Timer collisionTimer;

	internal bool hunting;
	[SerializeField] float triggerHuntMaxDistance;
	internal float sqrTriggerHuntMaxDistance;

	Timer refreshTargetTimer;

	protected override bool AutoClampPos {
		get {
			return true;
		}
	}

	protected override bool CanAct{
		get{
			return base.CanAct && (collisionTimer==null || collisionTimer.Check());
		}
	}

	protected override void Awake(){
		base.Awake();
		team = Team.ENEMY;
		refreshTargetTimer = new Timer(0.3f);
		sqrTriggerHuntMaxDistance = triggerHuntMaxDistance*triggerHuntMaxDistance;
	}

	public override void Initialize(){
		base.Initialize();
		if(collisionCooldown>0f && collisionDamage>0 )
			collisionTimer = new Timer(collisionCooldown);
	}

	protected override void Update() {
		base.Update();

		if(!hunting || !CanAct)
			return;

		if(refreshTargetTimer.CheckAndUpdate()){
			Vector3 difference = GameManager.I.player.transform.position - transform.position;
			transform.localEulerAngles = MathUtil.GetAngle(difference.x,difference.z)*Vector3.up;
		}
	}

	protected override void FixedUpdate() {
		base.FixedUpdate();
		if(!CanAct)
			return;
		if(hunting)
			Move(transform.forward);
	}

	/// <summary>
	/// Do the damage at touch.
	/// 
	/// OnCollisionEnter isn't used, since on a continuous movement, this is ignored.
	/// </summary>
	void OnCollisionStay(Collision collision){
		if(!CanAct)
			return;

		Destructible destructible = collision.collider.GetComponentInParent<Destructible>();
		if(destructible != null && destructible.team != team){
			destructible.TakeDamage(collisionDamage);
			hunting = true; // Starts hunting, if isn't hunting.
			collisionTimer.Reset();
		}
	}
}