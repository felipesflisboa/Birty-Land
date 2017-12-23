using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Destructible who spawn creeps.
/// </summary>
public class Hive : Destructible {
	internal int hiveId;

	[Header("Hive")]
	[SerializeField] GameObject creepPrefab;
	[SerializeField] float spawnRadius;
	[SerializeField] float creepLimit;
	[SerializeField] int initialSpawnAtGameStart;
	[SerializeField] int initialSpawnAfterGameStart;
	[SerializeField] float spawnInterval;
	[SerializeField, Tooltip("For avoiding lag if each spawn point spawn at same intervals")] float initialIntervalBase;

	int stoppedCreepsNearCount;

	protected bool CanAct{
		get{
			return GameManager.I.state == GameState.OCURRING && Alive;
		}
	}

	protected override void Awake(){
		base.Awake();
		team = Team.ENEMY;
	}

	public override void Initialize(){
		base.Initialize();
		StartCoroutine(SpawnRoutine());
	}

	/// <summary>
	/// Refreshs the stoppedCreepsNearCount. This value is used to put a limit on spawned creeps. 
	/// </summary>
	void RefreshStoppedCreepsNearCount(){
		stoppedCreepsNearCount = Scenario.I.GetCreepsOnPosition(transform.position, spawnRadius*1.5f, false).Count;
	}

	IEnumerator SpawnRoutine(){
		bool atGameStart = GameManager.I.SecondsFloat<5f;
		if(atGameStart)
			yield return new WaitForSeconds(initialIntervalBase*hiveId);

		int initialSpawn = atGameStart ? initialSpawnAtGameStart : initialSpawnAfterGameStart;
		for (int i = 0; i < initialSpawn; i++)
			Spawn();

		yield return new WaitForSeconds(spawnInterval);
		while(CanAct){
			RefreshStoppedCreepsNearCount();
			if(stoppedCreepsNearCount < creepLimit)
				Spawn();
			yield return new WaitForSeconds(spawnInterval);
		}
	}
		
	Creep Spawn(){
		Vector2 posV2 = new Vector2(transform.position.x, transform.position.z);
		float usedSpawnRadius = spawnRadius;
		for(int overflowCount = 0; overflowCount<200; overflowCount++){
			if(overflowCount==100){
				Debug.LogWarningFormat(
					"[Hive.Spawn] overflowCount 100 in {0} that already have {1} creep(s)! Raising spawn radius!", name, stoppedCreepsNearCount
				);
				usedSpawnRadius*=1.5f;
			}

			Vector2 randomPos = Random.insideUnitCircle*usedSpawnRadius+posV2;
			Vector3 randomPosV3 = new Vector3(randomPos.x, 0f, randomPos.y);
			const float creepRadius = .3f;
			bool canSpawn = Physics.OverlapSphere(randomPosV3, creepRadius).Length == 0;
			if(canSpawn){
				Creep creep = Instantiate(creepPrefab).GetComponent<Creep>();
				creep.transform.position = randomPosV3;
				creep.transform.SetParent(Scenario.I.actorArea);
				creep.Initialize();
				return creep;
			}
		}
		Debug.LogErrorFormat("[Hive.Spawn] Can't spawn in {0} that already have {1} creep(s)", name, stoppedCreepsNearCount);
		return null;
	}

	void OnDrawGizmosSelected(){
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, spawnRadius);
	}
}