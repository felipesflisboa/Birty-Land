using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Actor {
	internal int damage{get; private set;}
	
	public void InitializeBullet(Actor pOwner, int pDamage, float pSpeed, float pDuration){
		damage = pDamage;
		team = pOwner.team;
		Rigidbody.velocity = transform.forward * pSpeed;
		this.Invoke(new WaitForSeconds(pDuration), Finish);
	}

	void OnTriggerEnter (Collider other){
		Destructible destructible = other.GetComponentInParent<Destructible>();
		if(ValidDamageTarget(destructible)){
			destructible.TakeDamage(damage);
			Explode();
		}
	}

	protected bool ValidDamageTarget(Destructible target){
		return target != null && target.team != team;
	}
}