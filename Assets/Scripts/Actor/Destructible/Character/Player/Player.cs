using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {
	[Header("Player")]
	[Tooltip("Content who will be rotate with the target"), SerializeField] Transform rotateContainer;
	public Transform cameraContainer;
	[SerializeField] AudioSource walkingBGM;
	[SerializeField] AudioSource damageSFX;

	internal PlayerLevelController levelController;
	Plane groundPlane;

	Timer cursorDetectionTimer;
	Timer refreshWalkingBGMTimer;

	[Header("Bullet")]
	[SerializeField] GameObject bulletPrefab;
	[SerializeField] Transform bulletShootPoint;
	[SerializeField] AudioSource bulletShootSFX;
	public int bulletDamage;
	[SerializeField] float bulletCooldown;
	[SerializeField] float bulletSpeed;
	[SerializeField] float bulletDuration;
	Timer bulletTimer;

	const float CURSOR_DETECTION_INTERVAL = 0.1f;
	const float REFRESH_WALKING_BGM_INTERVAL = 0.1f;

	protected override bool AutoClampPos {
		get {
			return true;
		}
	}

	public override float Speed{
		set{
			if(!Mathf.Approximately(Speed, value)){
				if(walkingBGM!=null)
					walkingBGM.pitch = Mathf.Clamp(0.6f+value/22f, 0.6f, 1.2f);
				base.Speed = value;
			}
		}
	}

	protected override void Awake(){
		base.Awake();
		team = Team.ALLY;
		groundPlane = new Plane(Vector3.down, Vector3.zero);
		levelController = new PlayerLevelController(this);
		bulletTimer = new Timer(bulletCooldown);
		cursorDetectionTimer = new Timer(CURSOR_DETECTION_INTERVAL);
		if(walkingBGM!=null)
			refreshWalkingBGMTimer = new Timer(REFRESH_WALKING_BGM_INTERVAL);
	}

	protected override void Update() {
		base.Update();

		if(!CanAct)
			return;
		
		if(Input.GetButton("Fire1") && !GameManager.I.Paused && bulletTimer.Check()){
			if(bulletShootSFX!=null)
				bulletShootSFX.Play();
			CreateBullet(bulletShootPoint);
			Scenario.I.NotifyCreepsNearNeedRefresh();
			bulletTimer.Reset();
		}

		bool updateRotateContainer = cursorDetectionTimer==null || cursorDetectionTimer.CheckAndUpdate();
		if(updateRotateContainer){
			Ray clickRay = Camera.main.ScreenPointToRay(Input.mousePosition); // Only checks first
			float rayDistance;
			if (groundPlane.Raycast(clickRay, out rayDistance)){
				Vector3 intersectPos = clickRay.GetPoint(rayDistance);
				Vector3 difference = intersectPos- transform.position;
				rotateContainer.localEulerAngles = MathUtil.GetAngle(difference.x,difference.z)*Vector3.up;
			}
		}

		if(refreshWalkingBGMTimer!=null && (refreshWalkingBGMTimer.CheckAndUpdate() || GameManager.I.Paused)){
			bool playWalkingBGM = isMoving && !GameManager.I.Paused;
			if(playWalkingBGM){
				if(!walkingBGM.isPlaying)
					walkingBGM.Play();
			}else{
				if(walkingBGM.isPlaying)
					walkingBGM.Pause();
			}
		}
	}

	protected override void FixedUpdate() {
		base.FixedUpdate();

		if(!CanAct)
			return;

		Move(new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")));
	}

	protected Bullet CreateBullet(Transform transformReference){
		Bullet bullet = Instantiate(bulletPrefab, transformReference.position, transformReference.rotation).GetComponent<Bullet>();
		bullet.transform.SetParent(Scenario.I.actorArea);
		bullet.InitializeBullet(this, bulletDamage, bulletSpeed, bulletDuration);
		return bullet;
	}

	public override void TakeDamage(int damage){
		base.TakeDamage(damage);
		if(damage>0 && Alive && damageSFX!=null){
			if(damageSFX.isPlaying)
				damageSFX.Stop();
			damageSFX.Play();
		}
	}

	public override void Finish(){
		cameraContainer.SetParent(Scenario.I.actorArea);
		gameObject.SetActive(false);
		GameManager.I.EndGame();
	}
}