using UnityEngine;

[System.Serializable] // with this class prefix, the object referenced from this class will have accesible field in the inspector.
public class PlayerWeapon {
    public string name = "Glock";

    public int damage = 10;
    public float range = 10f;


}
