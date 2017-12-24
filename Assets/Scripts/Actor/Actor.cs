using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for game objects like bullets, hive and player.
/// </summary>
public class Actor : MonoBehaviour {
	[SerializeField] protected GameObject[] explosionPrefabArray;
	internal Team team;
	Timer clampPosTimer;

	Rigidbody _rigidbody;
	public Rigidbody Rigidbody {
		get {
			if (_rigidbody == null)
				_rigidbody = GetComponent<Rigidbody> ();
			return _rigidbody;
		}
	}

	/// <summary>
	/// After a set interval, automatic clamp the position;
	/// </summary>
	protected virtual bool AutoClampPos {
		get {
			return false;
		}
	}

#region MonoBehaviour
	protected virtual void Awake(){
		if(AutoClampPos)
			clampPosTimer = new Timer(0.1f);
	}

	protected virtual void Start(){}

	protected virtual void Update(){
		if(clampPosTimer != null && clampPosTimer.CheckAndUpdate())
			ClampPos();
	}

	protected virtual void FixedUpdate(){}
#endregion

#region Actor main
	public virtual void Explode(){
		foreach(GameObject explosionPrefab in explosionPrefabArray){
			Vector3 explosionPos = transform.position;
			if(Rigidbody != null)
				explosionPos += - Time.fixedDeltaTime*Rigidbody.velocity; // Small adjustment for one frame behind.
			Transform explosionTransform = Instantiate(explosionPrefab).transform;
			explosionTransform.position = explosionPos;
			explosionTransform.SetParent(Scenario.I.actorArea);
		}
		Finish();
	}

	public virtual void Finish(){
		gameObject.SetActive(false); // Since destroying isn't immediate.
		Destroy(gameObject);
	}
#endregion

	/// <summary>
	/// Clamp position to scenario bounds.
	/// </summary>
	protected void ClampPos(){
		if(!Scenario.I.Rect.Contains(transform.position.XZToV2())){
			Vector3 newPos = transform.position;
			newPos.x = Mathf.Clamp(newPos.x, Scenario.I.Rect.xMin, Scenario.I.Rect.xMax);
			newPos.z = Mathf.Clamp(newPos.z, Scenario.I.Rect.yMin, Scenario.I.Rect.yMax);
			transform.position = newPos; 
		}
	}
}
