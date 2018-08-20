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
