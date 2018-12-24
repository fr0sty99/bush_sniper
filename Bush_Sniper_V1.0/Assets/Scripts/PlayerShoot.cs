using UnityEngine;
using UnityEngine.Networking;
using System;

public class PlayerShoot : MonoBehaviour
{
    private const string PLAYER_TAG = "Player";

    [SerializeField]
    public PlayerWeapon weapon;
    private float timeToFire = 0;

    private void Start()
    {

    }

    private void Update()
    {
        // if firerate is zero
        if (Math.Abs(weapon.fireRate) < float.Epsilon)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time > timeToFire)
            {
                timeToFire = Time.time + 1 / weapon.fireRate;
                Shoot();
            }
        }

    }

    private void Shoot()
    {
        // transform.right means forward in our case. the red axis is the axis which our player is facing


        Vector2 _startPos = weapon.firePoint.position;
        Vector2 _start = transform.position;
        Vector2 _destPos = transform.right * weapon.range;

        // Raycast from _startPos to _destPos with the length of weapon.range
        RaycastHit2D _hit = Physics2D.Raycast(_startPos, _destPos, weapon.range);

        GetComponentInParent<Player>().SpawnShootEffect(weapon.firePoint.position, weapon.firePoint.rotation);

        Debug.DrawRay(_startPos, _destPos, Color.cyan);

        // if we hit a player
        try
        {
            if (_hit.collider.tag == PLAYER_TAG)
            {
                // tell server that we hit that player with its netID in its n  ame
                Shoot(_hit.collider.name, weapon.damage);
            }
        }
        catch (Exception e)
        {
            // didnt hit any player.
            // TODO: rewrite this hacked part
            return;
        }

    }

    void Shoot(string _damagedID, int damage)
    {
        // TODO: make objects take damage
        // GetComponentInParent<Player>().TakeDamage(damage);
    }

}
