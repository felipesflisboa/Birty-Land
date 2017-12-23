using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : Actor {
	public int scoreValue;
	[SerializeField] int defaultMaxHP = 1;
	[SerializeField] float regenPerSecondPercent01;
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

	public override void Initialize(){
		base.Initialize();
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
		if(Alive && regenPerSecondPercent01>0){
			regenCount+=regenPerSecondPercent01*Time.deltaTime*maxHP;
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