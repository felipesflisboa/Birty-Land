using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy spawned every X seconds who hunts the player.
/// </summary>
public class Hunter : Character {
    float sqrMaxDistanceToShoot;

	Timer refreshTargetTimer;

	protected override bool AutoClampPos {
		get {
			return true;
		}
	}

	protected override void Awake(){
		base.Awake();
		team = Team.Enemy;
		refreshTargetTimer = new Timer(0.08f);;
    }

    protected override void Start() {
        base.Start();
        sqrMaxDistanceToShoot = Mathf.Pow(weapon.bulletSpeed * weapon.bulletDuration + 10f, 2);
    }

    protected override void Update() {
		base.Update();

		if(!CanAct)
			return;
        
		if(weapon.CanFire) {
            Vector2 difference = (GameManager.I.player.transform.position - transform.position).XZToV2();
            if (sqrMaxDistanceToShoot > difference.sqrMagnitude) {
                weapon.FirePress();
                Scenario.I.NotifyCreepsNearNeedRefresh();
            }
		}

		if(refreshTargetTimer.CheckAndUpdate()){
			Vector3 difference = GameManager.I.player.transform.position - transform.position;
			transform.localEulerAngles = (MathUtil.GetAngle(difference.x,difference.z))*Vector3.up;
		}
	}

	protected override void FixedUpdate() {
		base.FixedUpdate();
		if(!CanAct)
			return;
		Move(transform.forward);
	}
}