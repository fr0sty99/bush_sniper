using UnityEngine;

[RequireComponent(typeof(Transform))]
[System.Serializable]
public class Pistol : PlayerWeapon
{
    private void Start()
    {
        name = "Pistol";
        fireRate = 3;
        damage = 10;
        range = 20f;

    }
}
