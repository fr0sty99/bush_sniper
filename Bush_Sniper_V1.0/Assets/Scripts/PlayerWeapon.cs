using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;


[System.Serializable] // with this class prefix, the object referenced from this class will have accesible field in the inspector.
public class PlayerWeapon {
    public string name = "Glock";

    public float fireRate = 0;
    public int damage = 10;
    public float range = 10f;
    public Transform firePoint;
    public Transform bulletTrailPrefab;

    // TODO: find a way to require the firePoint and bulletTrailPrefab

}
