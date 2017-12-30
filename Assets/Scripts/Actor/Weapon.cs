using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Weapon that can be used by a Character. Call try to Shoot
/// </summary>
public class Weapon : MonoBehaviour {
    [Header("Bullet")]
    [SerializeField] AudioSource sfx;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField, Tooltip("When empty, uses self")] Transform[] bulletShootPointArray = new Transform[0];
    public int damage;
    [SerializeField] float cooldown;
    public float bulletSpeed;
    public float bulletDuration;

    Character owner;

    Timer timer;

    public bool CanFire {
        get {
            return !IsOnCooldown;
        }
    }

    public bool IsOnCooldown {
        get {
            return timer != null && !timer.Check();
        }
    }

    public void Initialize(Character pOwner) {
        owner = pOwner;
        if (bulletShootPointArray.Length == 0)
            bulletShootPointArray = new[] { transform };
        timer = new Timer(cooldown);
    }

    /// <summary>
    /// Try to Fire
    /// </summary>
    public bool FirePress() {
        if (CanFire) {
            PutOnCooldown();
            Fire();
            return true;
        }
        return false;
    }

    void Fire() {
        if (sfx != null)
            sfx.Play();
        foreach (Transform shootPoint in bulletShootPointArray) {
            Bullet bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation).GetComponent<Bullet>();
            bullet.transform.SetParent(Scenario.I.actorArea);
            bullet.InitializeBullet(owner, damage, bulletSpeed, bulletDuration);
        }
    }

    public void PutOnCooldown() {
        if (timer != null && timer.interval == cooldown) {
            timer.Reset();
        } else {
            timer = null; //TODO Maybe change it into a property
            if (cooldown != 0f)
                timer = new Timer(cooldown, false);
        }
    }
}