using UnityEngine;

[System.Serializable] // with this class prefix, the object referenced from this class will have accesible field in the inspector.
public class PlayerWeapon {
    public string name = "Glock";

    public float damage = 10f;
    public float range = 100f;


}
