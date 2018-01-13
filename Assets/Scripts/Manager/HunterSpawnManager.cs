using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class who spawns the Hunters.
/// </summary>
public class HunterSpawnManager : SingletonMonoBehaviour<HunterSpawnManager> {
	[SerializeField] GameObject[] prefabArray;
	[SerializeField] AudioSource spawnSFX;

	int lastIndex=-1;
	Timer spawnCheckTimer;

	public const float SPAWN_INTERVAL = 60;

	public void Begin(){
		spawnCheckTimer = new Timer(SPAWN_INTERVAL, false);
	}

	void Update(){
		if(GameManager.I.state != GameState.Ocurring)
			return;

		if(spawnCheckTimer != null && spawnCheckTimer.CheckAndUpdate()){
			GameManager.I.PrintDebugData();
			if(spawnSFX!=null)
				spawnSFX.Play();
			SpawnHunter();
		}
	}

	Hunter SpawnHunter(){
		lastIndex = Mathf.Min(lastIndex+1, prefabArray.Length-1);
		GameObject hunterPrefab = prefabArray[lastIndex];

		Vector3 randomPos = Vector3.zero;
		for(int overflowCount = 0; overflowCount<100; overflowCount++){
			randomPos += Scenario.I.transform.position;

			// Do a simple OverlapSphere to make sure than isn't anything on the spot.
			float hunterRadius = hunterPrefab.GetComponent<Destructible>().sphereCastRadius;
			bool canSpawn = Physics.OverlapSphere(randomPos, hunterRadius).Length == 0;
			if(canSpawn){
				Hunter hunter = Instantiate(hunterPrefab).GetComponent<Hunter>();
				hunter.transform.position = randomPos;
				hunter.transform.SetParent(Scenario.I.actorArea);
				return hunter;
			}

			const float spawnAreaRadius = 7f;
			randomPos = (Random.insideUnitCircle*spawnAreaRadius).YToZ();
		}
		Debug.LogErrorFormat("[HunterSpawnManager.Spawn] Can't spawn in {0}.", name);
		return null;
	}
}