using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

[System.Serializable] // with this class prefix, the object referenced from this class will have accesible field in the inspector.
public class PlayerWeapon {
    public string name = "Glock";

    public float fireRate = 0;
    public int damage = 10;
    public float range = 10f;
    public GameObject firePoint;

    PlayerWeapon() {
        if (firePoint == null)
        {
            Debug.LogError("PlayerWeapon::Constructor -- Oops, Please give this weapon a firepoint.");
        }
    }

}
