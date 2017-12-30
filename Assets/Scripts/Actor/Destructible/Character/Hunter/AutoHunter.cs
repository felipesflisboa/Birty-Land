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

        bool firstAuto = differenceMinute == 0;
        if (firstAuto) {
            maxHP = 1500;
            Speed = 14f;
        } else {
            maxHP = 1800 + 600 * differenceMinute;
            Speed = 16.5f + 0.8f * differenceMinute;
        }
        int damage = 120 + 25 * differenceMinute;
        scoreValue = 30 + 10 * differenceMinute;

		if(SpriteRenderer!=null && spriteMaterialArray.Length>0)
			SpriteRenderer.material = spriteMaterialArray[differenceMinute%spriteMaterialArray.Length];

		base.Awake();

        weapon.damage = damage;
    }
}