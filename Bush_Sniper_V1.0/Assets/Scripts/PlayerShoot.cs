using UnityEngine;
using UnityEngine.Networking;
using System;

public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";

    [SerializeField]
    public PlayerWeapon weapon;
    private float timeToFire = 0;
    [SerializeField]
    private LayerMask mask;

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

    [Client]
    private void Shoot()
    {
        // transform.right means forward in our case. the red axis is the axis which our player is facing


        Vector2 _startPos = weapon.firePoint.position;
        Vector2 _start = transform.position;
        Vector2 _destPos = transform.right * weapon.range;

        // Raycast from _startPos to _destPos with the length of weapon.range, we only hit objects in the Layermask "mask"
        RaycastHit2D _hit = Physics2D.Raycast(_startPos, _destPos, weapon.range, mask);

        CmdSpawnShootEffect(transform.name, weapon.firePoint.position, weapon.firePoint.rotation);

        Debug.DrawRay(_startPos, _destPos, Color.cyan);

        // if we hit a player
        try
        {
            if (_hit.collider.tag == PLAYER_TAG)
            {
                // tell server that we hit that player with its netID in its n  ame
                CmdPlayerShoot(_hit.collider.name, transform.name, weapon.damage);
            }
        }
        catch (Exception e)
        {
            // didnt hit any player.
            // TODO: rewrite this hacked part
            return;
        }

    }

    [Command] // this gets called on every client
    void CmdSpawnShootEffect(string _playerId, Vector2 pos, Quaternion rot)
    {
        Player _player = GameManager.GetPlayer(_playerId);
        _player.RpcSpawnShootEffect(pos, rot);
    }

    [Command]
    void CmdPlayerShoot(string _damagedPlayerID, string _playerId, int damage)
    {
        Debug.Log(_damagedPlayerID + " has been shot from " + _playerId + " with a damage of " + damage);

        Player _player = GameManager.GetPlayer(_damagedPlayerID);
        _player.RpcTakeDamage(damage);
    }

}
