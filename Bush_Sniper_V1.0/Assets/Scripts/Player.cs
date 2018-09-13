using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player : NetworkBehaviour {

    [SyncVar] // this prefix means, when the value of this variable changes, it will be "pushed out" to all other clients
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


    [SyncVar] // this prefix means, when the value of this variable changes, it will be "pushed out" to all other clients
    private int currentHealt;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    public void Setup()
	{
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        } 

        SetDefaults();



	}

	//void Update()
	//{
 //       if(!isLocalPlayer) {
 //           return;
 //       }

 //       if(Input.GetKeyDown(KeyCode.K)) {
 //           RpcTakeDamage(999);
 //       }
	//}

    [ClientRpc] // this method gets executed on every client if its called
    public void RpcSpawnShootEffect(Vector2 _pos, Quaternion _rot) {
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

    [ClientRpc] // this method gets executed on every client if its called
    public void RpcTakeDamage(int _amount) 
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

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        Collider2D _col = GetComponent<Collider2D>();
        if (_col != null)
        {
            _col.enabled = true;
        }

        Debug.Log(transform.name + " is DEAD!");

        StartCoroutine(Respawn());
         
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        SetDefaults();
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        Debug.Log(transform.name + " respawned.");
    }

    public void SetDefaults() 
    {
        isDead = false;

        currentHealt = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        Collider2D _col = GetComponent<Collider2D>();
        if(_col != null) {
            _col.enabled = true;
        }
    }
}
