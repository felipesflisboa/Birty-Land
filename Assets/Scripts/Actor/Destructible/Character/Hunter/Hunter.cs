using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy spawned every X seconds who hunts the player.
/// </summary>
public class Hunter : Character {
	[Header("Bullet")]
	[SerializeField] GameObject bulletPrefab;
	[SerializeField, Tooltip("When empty, uses self")] Transform[] bulletShootPointArray;
	[SerializeField] AudioSource bulletShootSFX;
	public int bulletDamage;
	[SerializeField] protected float bulletCooldown;
	[SerializeField] protected float bulletSpeed;
	[SerializeField] protected float bulletDuration;
	Timer bulletTimer;

	Timer refreshTargetTimer;

	protected override bool AutoClampPos {
		get {
			return true;
		}
	}

	protected override void Awake(){
		base.Awake();
		team = Team.ENEMY;
		if(bulletShootPointArray.Length==0)
			bulletShootPointArray = new []{transform};
		refreshTargetTimer = new Timer(0.08f);
	}

	public override void Initialize(){
		base.Initialize();
		bulletTimer = new Timer(bulletCooldown);
	}

	protected override void Update() {
		base.Update();

		if(!CanAct)
			return;

		if(bulletTimer.Check()){
			if(bulletShootSFX!=null)
				bulletShootSFX.Play();
			foreach(Transform shootPoint in bulletShootPointArray)
				CreateBullet(shootPoint);
			Scenario.I.NotifyCreepsNearNeedRefresh();
			bulletTimer.Reset();
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

	protected Bullet CreateBullet(Transform transformReference){
		Bullet bullet = Instantiate(bulletPrefab, transformReference.position, transformReference.rotation).GetComponent<Bullet>();
		bullet.transform.SetParent(Scenario.I.actorArea);
		bullet.InitializeBullet(this, bulletDamage, bulletSpeed, bulletDuration);
		return bullet;
	}
}