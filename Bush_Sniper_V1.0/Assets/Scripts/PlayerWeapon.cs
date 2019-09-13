using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

[RequireComponent(typeof(Transform))]
[System.Serializable] // with this class prefix, the object referenced from this class will have accesible field in the inspector.
public class PlayerWeapon : MonoBehaviour {
    public string name = "default";
    public float fireRate = 0;
    public int damage = 10;
    public float range = 20f;
    public Transform firePoint;
}
