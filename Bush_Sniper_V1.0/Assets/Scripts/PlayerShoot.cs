using UnityEngine;
using UnityEngine.Networking;
using System;

public class PlayerShoot : MonoBehaviour
{
    private const string PLAYER_TAG = "Player";
    private const string CRATE_TAG = "Crate";
    private const string RIFLE_TAG = "Rifle";
    private const string SHOTGUN_TAG = "Shotgun";
    private const string PISTOL_TAG = "Pistol";
    private const string FIST_TAG = "Fist";

    private GameObject pistol;
    private GameObject shotgun;
    private GameObject rifle;
    private GameObject fist;

    [SerializeField]
    public PlayerWeapon currentWeapon;
    private float timeToFire = 0;

    private void Start()
    {
        pistol = transform.Find("WeaponHolder").Find("Pistol").gameObject;
        shotgun = transform.Find("WeaponHolder").Find("Shotgun").gameObject;
        rifle = transform.Find("WeaponHolder").Find("Rifle").gameObject;
        fist = transform.Find("WeaponHolder").Find("Fist").gameObject;
    }

    private void Update()
    {
        // if firerate is zero
        if (Math.Abs(currentWeapon.fireRate) < float.Epsilon)
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
                timeToFire = Time.time + 1 / currentWeapon.fireRate;
                Shoot();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == PISTOL_TAG)
        {
            pistol.SetActive(true);
            shotgun.SetActive(false);
            rifle.SetActive(false);
            fist.SetActive(false);
            currentWeapon = pistol.GetComponent<Pistol>();
        }
        else if (collision.name == RIFLE_TAG)
        {
            pistol.SetActive(false);
            shotgun.SetActive(false);
            rifle.SetActive(true);
            fist.SetActive(false);
            currentWeapon = rifle.GetComponent<Rifle>();
        }
        else if (collision.name == SHOTGUN_TAG)
        {
            pistol.SetActive(false);
            shotgun.SetActive(true);
            rifle.SetActive(false);
            fist.SetActive(false);
            currentWeapon = shotgun.GetComponent<Shotgun>();
        }
        Destroy(collision.transform.parent.gameObject);
    }

    private void Shoot()
    {
        // transform.right means forward in our case. the red axis is the axis which our player is facing
        if (currentWeapon.name != FIST_TAG)
        {
            Vector2 _startPos = currentWeapon.firePoint.position;
            Vector2 _destPos = transform.right * currentWeapon.range;

            // Raycast from _startPos to _destPos with the length of weapon.range
            RaycastHit2D _hit = Physics2D.Raycast(_startPos, _destPos, currentWeapon.range);

            GetComponentInParent<Player>().SpawnShootEffect(currentWeapon.firePoint.position, currentWeapon.firePoint.rotation, currentWeapon.name);
            Debug.DrawRay(_startPos, _destPos, Color.cyan);

            // if we hit a player
            try
            {
                if (_hit.collider.tag == CRATE_TAG)
                {
                    _hit.collider.gameObject.GetComponent<Crate>().takeDamange(currentWeapon.damage);
                    Debug.Log("crate hit with: " + currentWeapon.damage + ". crate health: " + _hit.collider.gameObject.GetComponent<Crate>().getHealth());
                    if (_hit.collider.gameObject.GetComponent<Crate>().getHealth() <= 0)
                    {
                        Destroy(_hit.collider.gameObject); // will spawn weapon and show exploding animation
                    }
                }
            }
            catch (Exception e)
            {
                // didnt hit any player.
                // TODO: rewrite this hacked part
                return;
            }
        }
        else
        {
            // TODO: meele attack
            Vector2 _startPos = currentWeapon.firePoint.position;
            Vector2 _destPos = transform.right * 0.3f; // TODO: remove hardcoded meeele range

            // Raycast from _startPos to _destPos with the length of weapon.range
            RaycastHit2D _hit = Physics2D.Raycast(_startPos, _destPos, 0.3f);

            // show attack animation
            Debug.DrawRay(_startPos, _destPos, Color.cyan);
            if (_hit.collider.tag == CRATE_TAG)
            {
                _hit.collider.gameObject.GetComponent<Crate>().takeDamange(currentWeapon.damage);
                Debug.Log("crate hit with: " + currentWeapon.damage + ". crate health: " + _hit.collider.gameObject.GetComponent<Crate>().getHealth());
                if (_hit.collider.gameObject.GetComponent<Crate>().getHealth() <= 0)
                {
                    Destroy(_hit.collider.gameObject); // will spawn weapon and show exploding animation
                }
            }

        }
    }

    void Shoot(string _damagedID, int damage)
    {
        // TODO: make objects take damage
        // GetComponentInParent<Player>().TakeDamage(damage);
    }

}
