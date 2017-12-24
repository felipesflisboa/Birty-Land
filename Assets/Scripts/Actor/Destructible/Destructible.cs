using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for anything that has HP.
/// </summary>
public class Destructible : Actor {
	public int scoreValue;
	[Tooltip("For calculate when spawning")] public float sphereCastRadius;
	[SerializeField] int defaultMaxHP = 1;
	[SerializeField] float regenPerSecondRatio;

	internal int maxHP;
	internal int hp;
	float regenCount;

	public virtual bool Alive{
		get{
			return hp > 0;
		}
	}

	public virtual float HPPercentage{
		get{
			return hp*100f / maxHP;
		}
	}

	protected override void Awake(){
		base.Awake();
		if(defaultMaxHP != 0){
			maxHP = defaultMaxHP;
		}
		if(maxHP != 0){
			hp = maxHP;
		}
		regenCount = 0f;
	}

	protected override void Update(){
		base.Update();
		if(Alive && regenPerSecondRatio>0){
			regenCount+=regenPerSecondRatio*Time.deltaTime*maxHP;
			while(regenCount>1f){
				regenCount-=1f;
				AddHP(1);
			}
		}
	}

	public virtual void TakeDamage(int damage){
		AddHP(-damage);
	}

	public virtual void AddHP(int damage){
		hp = Mathf.Clamp(hp + damage, 0, maxHP);
		if (hp == 0)
			Explode();
	}

	public override void Explode(){
		GameManager.I.AddScore(scoreValue);
		base.Explode();
	}
}