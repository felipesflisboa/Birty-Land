using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// System for handling player levels.
/// </summary>
[System.Serializable]
public class PlayerLevelSystem {
	Player player;

	internal int hpLevel;
	internal int attackLevel;
	internal int speedLevel;

	int hpBase;
	int attackBase;
	float speedBase;

	public PlayerLevelSystem(Player pPlayer){
		player = pPlayer;

		hpLevel = 1;
		attackLevel = 1;
		speedLevel = 1;

		hpBase = player.maxHP;
		attackBase = player.bulletDamage;
		speedBase = player.Speed;
	}

	/// <summary>
	/// Apply level gain on each attribute.
	/// </summary>
	void RefreshValues(){
		player.maxHP = hpBase*hpLevel;
		player.bulletDamage = attackBase*attackLevel;
		player.Speed = speedBase*speedLevel;
	}

	public int CostForNextLevel(int currentLevel){
		return currentLevel==1 ? 2 : Mathf.RoundToInt(5*Mathf.Pow(2,currentLevel-2));
	}
		
	public bool RaiseHPLevel(){
		return RaiseLevel(ref hpLevel);
	}
	public bool RaiseAttackLevel(){
		return RaiseLevel(ref attackLevel);
	}
	public bool RaiseSpeedLevel(){
		return RaiseLevel(ref speedLevel);
	}

	/// <summary>
	/// Try to raise an attribute level paying the cost.
	/// </summary>
	/// <returns>If cost was payed and level increased.</returns>
	/// <param name="currentLevel">Current level of the attribute who will raise if this method succeed.</param>
	bool RaiseLevel(ref int currentLevel){
		int cost = CostForNextLevel(currentLevel);
		if(cost<=GameManager.I.points){
			GameManager.I.SpendPoint(cost);
			++currentLevel;
			RefreshValues();
			return true;
		}else{
			return false;
		}
	}
}