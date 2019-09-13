using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player : MonoBehaviour {

    private bool _isDead = false;
    public bool isDead {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;

    [SerializeField]
    private GameObject bulletTrailPrefab;

    [SerializeField]
    private GameObject muzzleFlashPrefab;

    // TODO: refactor this hack
    private float muzzleRotationOffset = 180f; // needed to flip the image

    [SerializeField]
    private float muzzleLifeTime = 0.02f;
    // TODO: implement player skin

    private int currentHealt;


    public void Setup()
	{
        SetDefaults();



	}


    public void SpawnShootEffect(Vector2 _pos, Quaternion _rot, string WeaponName) {
        
        showBulletTrail(_pos, _rot);
        showMuzzle(_pos, _rot);
    }

    private void showMuzzle(Vector2 _pos, Quaternion _rot) {
        Vector3 muzzlePos = new Vector3(_pos.x, _pos.y, -1f);
        GameObject muzzleFlashObject = Instantiate(muzzleFlashPrefab, muzzlePos, _rot);
        muzzleFlashObject.transform.Rotate(0, 0, muzzleRotationOffset);
        //muzzleFlashObject.transform.parent = GetComponent<PlayerShoot>().weapon.firePoint;
        //float size = Random.Range(0.03f, 0.07f);
        //muzzleFlashObject.transform.localScale = new Vector2(size, size);
        Debug.Log("muzzle: " + muzzleFlashObject);
        Destroy(muzzleFlashObject, muzzleLifeTime);
    }

    private void showBulletTrail(Vector2 _pos, Quaternion _rot) {
        // spawn a bulletTrail and Destroy it after 1 second
        Destroy(Instantiate(bulletTrailPrefab, _pos, _rot), 1); 
    }

    public void TakeDamage(int _amount) 
    {
        if (isDead) {
            return;
        }
        currentHealt -= _amount;

        // only on server because this is the host, we deactivate all remotePlayer-Components
        Debug.Log(transform.name + " now has " + currentHealt + " Health.");

        if (currentHealt <= 0) {
            Die();
        }
    }

    private void Die() {
// TODO: die animation ///////////////////////////////
        isDead = true;

    }

    public void SetDefaults() 
    {
        isDead = false;

        currentHealt = maxHealth;

    }
}
