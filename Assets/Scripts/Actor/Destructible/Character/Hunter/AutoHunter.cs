using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This hunter automatic calculate its attributes based on game time.
/// </summary>
public class AutoHunter : Hunter {
	[SerializeField, Tooltip("Uses these array for dynamically defining a material for the sprite")] Material[] spriteMaterialArray;

	SpriteRenderer _spriteRenderer;
	public SpriteRenderer SpriteRenderer {
		get {
			if (_spriteRenderer == null)
				_spriteRenderer = GetComponentInChildren<SpriteRenderer> (true);
			return _spriteRenderer;
		}
	}

	protected override void Awake () {
		const int initialMinute = 4;
		int currentMinute = Mathf.RoundToInt(GameManager.I.SecondsFloat/HunterSpawnManager.SPAWN_INTERVAL);
		int differenceMinute =  currentMinute - initialMinute;

		scoreValue = 30+10*differenceMinute;
		maxHP = 1400 + 600*differenceMinute;
        Speed = 10.5f + 2.5f*differenceMinute;
		bulletDamage = 60 + 30*differenceMinute;

		if(SpriteRenderer!=null && spriteMaterialArray.Length>0)
			SpriteRenderer.material = spriteMaterialArray[differenceMinute%spriteMaterialArray.Length];

		base.Awake();
	}
}