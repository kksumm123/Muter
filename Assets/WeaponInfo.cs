using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInfo : MonoBehaviour
{
    public enum WeaponType
    {
        None,
        Gun,
        Melee,
        Throw,
    }
    public WeaponType weaponType;
    public float power = 20;
    public float addedRandomPowerValue = 4;
    public float delay = 0.2f;
    public float kncoBackForce = 1;

    [Header("√—")]
    public GameObject bullet;
    public Transform bulletSpawnPosition;
    public int currentClipBulletCount = 10;
    public int maxClipBulletCount = 20;
    public int allBulletCount = 500;
    public int maxBulletCount = 500;
    public float reloadTime = 2;

    [Header("±Ÿ¡¢")]
    public float attackStartTime = 0.3f;
    public float attackTime = 0.15f;
    Collider attackCollider;

    [Header("≈ı√¥")]
    public GameObject throwGo;

    public void Init()
    {
        if (weaponType == WeaponType.Gun)
            bulletSpawnPosition = transform.Find("BulletSpawnPosition");
        else
            attackCollider = transform.Find("AttackCollider").GetComponent<Collider>();
    }
}
