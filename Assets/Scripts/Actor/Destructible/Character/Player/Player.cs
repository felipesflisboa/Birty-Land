﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {
	[Header("Player")]
	[Tooltip("Content who will be rotate with the target"), SerializeField] Transform rotateContainer;
	public Transform cameraContainer;
	[SerializeField] AudioSource walkingBGM;
	[SerializeField] AudioSource damageSFX;

	internal PlayerLevelController levelController;
    float sqrMinClickDistanceByMovement;

    Timer aimDetectionTimer;
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

    // AutoAim
    float sqrAutoAimMaxDistance;
    Timer autoAimTimer;
    Destructible target;

    const float CURSOR_DETECTION_INTERVAL = 0.1f;
	const float REFRESH_WALKING_BGM_INTERVAL = 0.1f;
    const float AUTO_AIM_INTERVAL = 0.2f;
    const float AUTO_AIM_MAX_DISTANCE = 15f;
    const float MIN_CLICK_DISTANCE_BY_MOVEMENT = 0.5f;

    protected override bool AutoClampPos {
		get {
			return true;
		}
	}

	public override float Speed{
		set{
			if(!Mathf.Approximately(Speed, value)){
				if(walkingBGM!=null)
					walkingBGM.pitch = Mathf.Clamp(0.6f+value/30f, 0.6f, 1.2f);
				base.Speed = value;
			}
		}
    }

    Vector2 _lastInput;
    Vector2 _lastInputAdjusted;
    /// <summary>
    /// Input cached, multiplied by normal.
    /// </summary>
    protected Vector2 InputAdjusted {
        get {
            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (input != _lastInput) {
                _lastInput = input;
                _lastInputAdjusted = input.Multiply(MathUtil.Abs(input.normalized));
            }
            return _lastInputAdjusted;
        }
    }

    protected override void Awake(){
		base.Awake();
		team = Team.ALLY;

        sqrAutoAimMaxDistance = AUTO_AIM_MAX_DISTANCE * AUTO_AIM_MAX_DISTANCE;
        sqrMinClickDistanceByMovement = MIN_CLICK_DISTANCE_BY_MOVEMENT * MIN_CLICK_DISTANCE_BY_MOVEMENT;

        levelController = new PlayerLevelController(this);

		bulletTimer = new Timer(bulletCooldown);
		aimDetectionTimer = new Timer(CURSOR_DETECTION_INTERVAL);
        if(GameManager.I.UseTouchControls)
            autoAimTimer = new Timer(AUTO_AIM_INTERVAL);
        // refreshDestructibleArrayTimer = new Timer(REFRESH_DESTRUCTIBLE_ARRAY); //remove

        if (walkingBGM!=null)
			refreshWalkingBGMTimer = new Timer(REFRESH_WALKING_BGM_INTERVAL);
	}

    protected override void Update() {
		base.Update();

		if(!CanAct)
			return;

        //remove
        // bool autoAim = true;
        // if (autoAim) {
            //remove
            // if (refreshDestructibleArrayTimer.CheckAndUpdate()) {
            //    gameDestructibleArray = Scenario;
            // }
        // }

        if (autoAimTimer!= null && autoAimTimer.CheckAndUpdate()) {
            Destructible lastTarget = target;
            target = null;
            float sqrLowerDistance = sqrAutoAimMaxDistance;
            // Checks all destructible to find out the nearst one
            foreach (Destructible destructible in Scenario.I.destructibleList) {
                if (destructible==null || !destructible.Alive || destructible.team==team)
                    continue;
                float sqrDistance = (destructible.transform.position - transform.position).XZToV2().sqrMagnitude;
                if (sqrLowerDistance > sqrDistance) {
                    sqrLowerDistance = sqrDistance;
                    target = destructible;
                }
            }
            bool forceRotateRefresh = target != lastTarget && target != null;
            if (forceRotateRefresh)
                aimDetectionTimer.MarkDone();
        }

        bool updateRotateContainer = aimDetectionTimer.CheckAndUpdate();
        if (updateRotateContainer) {
            Vector2 difference = Vector2.zero;
            if (GameManager.I.UseTouchControls) {
                if(target!=null && target.Alive)
                    difference = (target.transform.position - transform.position).XZToV2();
            } else {
                GameManager.I.RefreshClickPosition();
                difference = GameManager.I.lastClickWorldPos - transform.position.XZToV2();
            }

            if(difference != Vector2.zero)
                rotateContainer.localEulerAngles = MathUtil.GetAngle(difference) * Vector3.up;
        }

        bool fire = GameManager.I.UseTouchControls ? (GameManager.I.autoFire && target != null) : Input.GetButton("Fire1");
        fire = fire && !GameManager.I.Paused && bulletTimer.Check();
        if (fire) {
            if (bulletShootSFX != null)
                bulletShootSFX.Play();
            CreateBullet(bulletShootPoint);
            Scenario.I.NotifyCreepsNearNeedRefresh();
            bulletTimer.Reset();
        }

        if (refreshWalkingBGMTimer!=null && (refreshWalkingBGMTimer.CheckAndUpdate() || GameManager.I.Paused)){
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

        if (GameManager.I.UseTouchControls) {
            GameManager.I.RefreshClickPosition();
            Vector2 clickDifference = GameManager.I.lastClickWorldPos - transform.position.XZToV2();
            if(sqrMinClickDistanceByMovement < clickDifference.sqrMagnitude)
                Move(clickDifference.normalized.YToZ());
        } else {
            Move(InputAdjusted.YToZ());
        }
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