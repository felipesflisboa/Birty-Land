using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Scenario : SingletonMonoBehaviour<Scenario> {
	[SerializeField] Vector2 size;
	[SerializeField] float border; // Counting size
	public Transform actorArea;

	//TODO cache creeps instead of searching every method call

	bool refreshNotifyCreeps;
	Timer notifyCreepsRefreshTimer;

	const float NOTIFY_CREEPS_REFRESH_INTERVAL = 1f;

	Rect _rect;
	public Rect Rect {
		get {
			if (_rect.width==0f)
				_rect = new Rect(-size/2f, size);
			return _rect;
		}
	}

	Rect _rectWithoutBorder;
	public Rect RectWithoutBorder {
		get {
			if (_rectWithoutBorder.width==0f)
				_rectWithoutBorder = new Rect(Scenario.I.Rect.min + border*Vector2.one, Scenario.I.Rect.size - 2*border*Vector2.one);;
			return _rectWithoutBorder;
		}
	}

	AudioSource _bgmAudioSource;
	public AudioSource BGMAudioSource {
		get {
			if (_bgmAudioSource == null)
				_bgmAudioSource = GetComponent<AudioSource>();
			return _bgmAudioSource;
		}
	}

	void Start(){
		notifyCreepsRefreshTimer = new Timer(NOTIFY_CREEPS_REFRESH_INTERVAL);
	}

	void Update(){
		if(refreshNotifyCreeps && notifyCreepsRefreshTimer.Check()){
			NotifyCreepsNear(GameManager.I.player.transform.position);
		}
	}

	public void NotifyCreepsNearNeedRefresh(){
		refreshNotifyCreeps = true;
	}

	/// <summary>
	/// Notifies the creeps near for hunting state.
	/// Shouldn't be called in a small interval, prefer NotifyCreepsNearNeedRefresh.
	/// </summary>
	/// <param name="pos">Position.</param>
	public void NotifyCreepsNear(Vector3 pos){
		foreach(Creep creep in actorArea.GetComponentsInChildren<Creep>()){
			if(creep.hunting)
				continue;

			Vector3 difference = creep.transform.position - pos;
			bool onRange = new Vector2(difference.x,difference.z).sqrMagnitude < creep.sqrTriggerHuntMaxDistance;
			creep.hunting = onRange;
		}

		notifyCreepsRefreshTimer.Reset();
		refreshNotifyCreeps = false;
	}

	/// <param name="hunting">Hunting.</param>
	/// <summary>
	/// Return all creeps in a range.
	/// </summary>
	/// <returns>The creeps on the range.</returns>
	/// <param name="pos">Center position.</param>
	/// <param name="radius">Radius of sphere search.</param>
	/// <param name="hunting">If isn't null, return only creeps with the same hunting value.</param>
	public List<Creep> GetCreepsOnPosition(Vector3 pos, float radius, bool? hunting){
		float sqrRadius = radius*radius;
		List<Creep> creepList = new List<Creep>();
		foreach(Creep creep in actorArea.GetComponentsInChildren<Creep>()){
			if(hunting != null && creep.hunting != hunting)
				continue;

			Vector3 difference = creep.transform.position - pos;
			bool onRange = new Vector2(difference.x,difference.z).sqrMagnitude < sqrRadius;
			//TODO add creep radius
			if(onRange)
				creepList.Add(creep);
		}
		return creepList;
	}

	void OnDrawGizmos() {
		Gizmos.color = new Color(0.2f,0.6f, 0.6f, 0.75F);
		Gizmos.DrawWireCube (transform.position, new Vector3(size.x-2*border,1f,size.y-2*border));
		Gizmos.color = new Color(0.2f,1F, 0.2f, 0.75F);
		Gizmos.DrawWireCube (transform.position, new Vector3(size.x,1f,size.y));
	}
}