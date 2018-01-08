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
    float sqrMinClickDistanceByMovement;
    float sqrMinClickDistanceToMove;

    Timer aimDetectionTimer;
	Timer refreshWalkingBGMTimer;

    // AutoAim
    float sqrAutoAimMaxDistance;
    Timer autoAimTimer;
    Destructible target;

    const float CURSOR_DETECTION_INTERVAL = 0.1f;
	const float REFRESH_WALKING_BGM_INTERVAL = 0.1f;
    const float AUTO_AIM_INTERVAL = 0.2f;
    const float AUTO_AIM_MAX_DISTANCE = 15f;
    const float MIN_CLICK_DISTANCE_BY_MOVEMENT = 1f;

    protected override bool AutoClampPos {
		get {
			return true;
		}
	}

    public int Damage {
        get {
            return weapon.damage;
        }
        set {
            weapon.damage = value;
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
        
		aimDetectionTimer = new Timer(CURSOR_DETECTION_INTERVAL);
        if(GameManager.I.UseTouchControls)
            autoAimTimer = new Timer(AUTO_AIM_INTERVAL);

        if (walkingBGM!=null)
			refreshWalkingBGMTimer = new Timer(REFRESH_WALKING_BGM_INTERVAL);
	}

    protected override void Update() {
		base.Update();

		if(!CanAct)
			return;

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
                GameManager.I.RefreshCursorPosition();
                difference = GameManager.I.lastClickWorldPos - transform.position.XZToV2();
            }

            if(difference != Vector2.zero)
                rotateContainer.localEulerAngles = MathUtil.GetAngle(difference) * Vector3.up;
        }

        bool fire = GameManager.I.UseTouchControls ? (GameManager.I.autoFire && target != null) : Input.GetButton("Fire1");
        fire = fire && !GameManager.I.Paused && weapon.CanFire;
        if (fire) {
            weapon.FirePress();
            Scenario.I.NotifyCreepsNearNeedRefresh();
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
            bool clicked = Input.GetMouseButton(0);
#if UNITY_EDITOR
            clicked = true; // Don't force a click on editor.
#endif
            if(clicked)
                GameManager.I.RefreshCursorPosition();
            Vector2 clickDifference = GameManager.I.lastClickWorldPos - transform.position.XZToV2();
            Debug.LogFormat(
                "lastClickWorldPos={0} transform.position={1} clickDifference={2} clickDifference.sqrMagnitude={3}",
                GameManager.I.lastClickWorldPos,
                transform.position,
                clickDifference,
                clickDifference.sqrMagnitude
            ); //remove
            if(sqrMinClickDistanceByMovement < clickDifference.sqrMagnitude)
                Move(clickDifference.normalized.YToZ());
        } else {
            Move(InputAdjusted.YToZ());
        }
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