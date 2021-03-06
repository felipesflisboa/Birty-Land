﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// System for handling player levels gains.
/// </summary>
[System.Serializable]
public class PlayerLevelController {
	Player player;

	internal int hpLevel;
	internal int attackLevel;
	internal int speedLevel;

	int hpBase;
	int attackBase;
	float speedBase;

	public PlayerLevelController(Player pPlayer){
		player = pPlayer;

		hpLevel = 1;
		attackLevel = 1;
		speedLevel = 1;

		hpBase = player.maxHP;
		attackBase = player.Damage;
		speedBase = player.Speed;
	}

	/// <summary>
	/// Apply level gain on each attribute.
	/// </summary>
	void RefreshValues(){
		player.maxHP = hpBase*hpLevel;
		player.Damage = attackBase*attackLevel;
		player.Speed = speedBase*speedLevel;
	}

	public int CostForNextLevel(int currentLevel){
		return currentLevel==1 ? 2 : Mathf.RoundToInt(5*Mathf.Pow(2,currentLevel-2));
	}
		
	public bool RaiseHPLevel(bool force=false){
		return RaiseLevel(ref hpLevel, force);
	}
	public bool RaiseAttackLevel(bool force=false) {
		return RaiseLevel(ref attackLevel, force);
	}
	public bool RaiseSpeedLevel(bool force=false) {
		return RaiseLevel(ref speedLevel, force);
	}

	/// <summary>
	/// Try to raise an attribute level paying the cost.
	/// </summary>
	/// <returns>If cost was payed and level increased.</returns>
	/// <param name="currentLevel">Current level of the attribute who will raise if this method succeed.</param>
	/// <param name="force">When true, don't apply the cost limitation.</param>
	bool RaiseLevel(ref int currentLevel, bool force){
		int cost = CostForNextLevel(currentLevel);
		if(force || cost <= GameManager.I.points){
			if(!force)
				GameManager.I.SpendPoint(cost);
			++currentLevel;
			RefreshValues();
			return true;
		}else{
			return false;
		}
	}
}
