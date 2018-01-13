using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class who spawns the Hives.
/// </summary>
public class HiveSpawnManager : SingletonMonoBehaviour<HiveSpawnManager> {
	[SerializeField] GameObject prefab;

	int lastId;
	Timer spawnCheckTimer;

	const int QUANTITY = 23;
	const float SPAWN_CHECK_INTERVAL = 5;

	const float MIN_DISTANCE_BETWEEN_HIVES = 12;
	const float INITIAL_MIN_DISTANCE_BETWEEN_PLAYER = 5.5f;
	const float MIN_DISTANCE_BETWEEN_PLAYER = 15; // After game starts, hives only will spawn outscreen. 

	public void Begin(){
		List<Hive> hiveList = new List<Hive>();
		for (int i = 0; i < QUANTITY; i++)
			hiveList.Add(Spawn(hiveList, true));

		spawnCheckTimer = new Timer(SPAWN_CHECK_INTERVAL, false);
	}

	void Update(){
		if(GameManager.I.state != GameState.Ocurring)
			return;

		if(spawnCheckTimer!=null && spawnCheckTimer.CheckAndUpdate()){
			List<Hive> hiveList = Scenario.I.actorArea.GetComponentsInChildren<Hive>(true).ToList();
			while(hiveList.Count<QUANTITY)
				hiveList.Add(Spawn(hiveList, false));
		}
	}

	Hive Spawn(List<Hive> hiveList, bool initialSpawn){
		float sqrMinDistanceBetweenHives = Mathf.Pow(MIN_DISTANCE_BETWEEN_HIVES, 2);
		float sqrMinDistanceBetweenHivePlayer = Mathf.Pow(
			initialSpawn ? INITIAL_MIN_DISTANCE_BETWEEN_PLAYER : MIN_DISTANCE_BETWEEN_PLAYER, 2
		);
		for(int overflowCount = 0; overflowCount<100; overflowCount++){
			Vector3 randomPos = Vector3.zero;
			randomPos.x = Random.Range(Scenario.I.RectWithoutBorder.xMin, Scenario.I.RectWithoutBorder.xMax);
			randomPos.z = Random.Range(Scenario.I.RectWithoutBorder.yMin, Scenario.I.RectWithoutBorder.yMax);

			bool canSpawn = true;

			// Check Player
			Vector3 differenceToPlayer = GameManager.I.player.transform.position - randomPos;
			bool onPlayerRange = differenceToPlayer.XZToV2().sqrMagnitude < sqrMinDistanceBetweenHivePlayer;
			canSpawn = !onPlayerRange;
			if(!canSpawn)
				continue;

			// Check other Hives
			for (int i = 0; i < hiveList.Count && canSpawn; i++) {
				Vector3 difference = hiveList[i].transform.position - randomPos;
				bool onRange = difference.XZToV2().sqrMagnitude < sqrMinDistanceBetweenHives;
				canSpawn = !onRange;
			}
			if(!canSpawn)
				continue;

			// Do a simple OverlapSphere to make sure than isn't anything on the spot.
			float hiveRadius = prefab.GetComponent<Destructible>().sphereCastRadius;
			canSpawn = Physics.OverlapSphere(randomPos, hiveRadius).Length == 0;
			if(!canSpawn)
				continue;

			// After successfully passed by all checks, initialize hive.
			Hive hive = Instantiate(prefab).GetComponent<Hive>();
			hive.transform.position = randomPos;
			hive.transform.SetParent(Scenario.I.actorArea);
			hive.hiveId = ++lastId;
			return hive;
		}
		Debug.LogErrorFormat("[HiveSpawnManager.Spawn] Can't spawn in {0}. {1} hive(s) in game.", name, hiveList.Count);
		return null;
	}
}