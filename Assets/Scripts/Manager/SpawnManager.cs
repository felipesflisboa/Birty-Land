using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class who spawns the Hives and the Hunters.
/// </summary>
public class SpawnManager : SingletonMonoBehaviour<SpawnManager> {
	[SerializeField] GameObject hivePrefab;
	[SerializeField] GameObject[] hunterPrefabArray;
	[SerializeField] AudioSource hunterSpawnSFX;

	int lastHiveId;
	int lastHunterIndex=-1;

	Timer spawnHiveCheckTimer;
	Timer spawnHunterCheckTimer;

	const int HIVE_QUANTITY = 23;
	const float SPAWN_HIVE_CHECK_INTERVAL = 5;

	const float MIN_DISTANCE_BETWEEN_HIVES = 12;
	const float INITIAL_MIN_DISTANCE_BETWEEN_HIVE_PLAYER = 4;
	const float MIN_DISTANCE_BETWEEN_HIVE_PLAYER = 15; // After game starts, hives only will spawn outscreen. 

	public const float SPAWN_HUNTER_INTERVAL = 60;

	public void Begin(){
		const float border = 7f; // So won't spawn hives on borders 

		List<Hive> hiveList = new List<Hive>();
		for (int i = 0; i < HIVE_QUANTITY; i++)
			hiveList.Add(SpawnHive(hiveList, true));

		spawnHiveCheckTimer = new Timer(SPAWN_HIVE_CHECK_INTERVAL, false);
		spawnHunterCheckTimer = new Timer(SPAWN_HUNTER_INTERVAL, false);
	}

	void Update(){
		if(GameManager.I.state != GameState.OCURRING)
			return;

		if(spawnHiveCheckTimer!=null && spawnHiveCheckTimer.CheckAndUpdate()){
			List<Hive> hiveList = Scenario.I.actorArea.GetComponentsInChildren<Hive>(true).ToList();
			while(hiveList.Count<HIVE_QUANTITY)
				hiveList.Add(SpawnHive(hiveList, false));
		}
		if(spawnHunterCheckTimer != null && spawnHunterCheckTimer.CheckAndUpdate()){
			if(hunterSpawnSFX!=null)
				hunterSpawnSFX.Play();
			SpawnHunter();
		}
	}

	Hive SpawnHive(List<Hive> hiveList, bool initialSpawn){
		float sqrMinDistanceBetweenHives = Mathf.Pow(MIN_DISTANCE_BETWEEN_HIVES, 2);
		float sqrMinDistanceBetweenHivePlayer = Mathf.Pow(
			initialSpawn ? INITIAL_MIN_DISTANCE_BETWEEN_HIVE_PLAYER : MIN_DISTANCE_BETWEEN_HIVE_PLAYER, 2
		);
		for(int overflowCount = 0; overflowCount<100; overflowCount++){
			Vector3 randomPos = Vector3.zero;
			randomPos.x = Random.Range(Scenario.I.RectWithoutBorder.xMin, Scenario.I.RectWithoutBorder.xMax);
			randomPos.z = Random.Range(Scenario.I.RectWithoutBorder.yMin, Scenario.I.RectWithoutBorder.yMax);

			bool canSpawn = true;

			// Check Player
			Vector3 differenceToPlayer = GameManager.I.player.transform.position - randomPos;
			bool onPlayerRange = new Vector2(differenceToPlayer.x,differenceToPlayer.z).sqrMagnitude < sqrMinDistanceBetweenHivePlayer;
			canSpawn = !onPlayerRange;
			if(!canSpawn)
				continue;

			// Check other Hives
			for (int i = 0; i < hiveList.Count && canSpawn; i++) {
				Vector3 difference = hiveList[i].transform.position - randomPos;
				bool onRange = new Vector2(difference.x,difference.z).sqrMagnitude < sqrMinDistanceBetweenHives;
				canSpawn = !onRange;
			}
			if(!canSpawn)
				continue;

			// Do a simple OverlapSphere to make sure than isn't anything on the spot.
			const float hiveRadius = 1.2f;
			canSpawn = Physics.OverlapSphere(randomPos, hiveRadius).Length == 0;
			if(!canSpawn)
				continue;

			// After successfully passed by all checks, initialize hive.
			Hive hive = Instantiate(hivePrefab).GetComponent<Hive>();
			hive.transform.position = randomPos;
			hive.transform.SetParent(Scenario.I.actorArea);
			hive.hiveId = ++lastHiveId;
			hive.Initialize();
			return hive;
		}
		Debug.LogErrorFormat("[SpawnManager.SpawnHive] Can't spawn in {0}. {1} hive(s) in game.", name, hiveList.Count);
		return null;
	}


	Hunter SpawnHunter(){
		lastHunterIndex = Mathf.Min(lastHunterIndex+1, hunterPrefabArray.Length-1);
		GameObject hunterPrefab = hunterPrefabArray[lastHunterIndex];

		Vector3 randomPos = Vector3.zero;
		for(int overflowCount = 0; overflowCount<100; overflowCount++){
			randomPos += Scenario.I.transform.position;

			// Do a simple OverlapSphere to make sure than isn't anything on the spot.
			const float hunterRadius = 1.2f;
			bool canSpawn = Physics.OverlapSphere(randomPos, hunterRadius).Length == 0;
			if(canSpawn){
				Hunter hunter = Instantiate(hunterPrefab).GetComponent<Hunter>();
				hunter.transform.position = randomPos;
				hunter.transform.SetParent(Scenario.I.actorArea);
				hunter.Initialize();
				Debug.LogFormat("[SpawnManager.SpawnHunter] {0} spawned!.", hunter.name);
				return hunter;
			}

			const float spawnAreaRadius = 7f;
			Vector2 circleRandomPos = Random.insideUnitCircle*spawnAreaRadius;
			randomPos = new Vector3(circleRandomPos.x, 0f, circleRandomPos.y);
		}
		Debug.LogErrorFormat("[SpawnManager.SpawnHunter] Can't spawn in {0}.", name);
		return null;
	}
}