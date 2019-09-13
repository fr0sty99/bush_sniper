using UnityEngine;

[RequireComponent(typeof(Transform))]
[System.Serializable]
public class Shotgun : PlayerWeapon
{
    private void Start() {
        name = "Shotgun";
        fireRate = 5;
        damage = 30;
        range = 10f;
    }
}
