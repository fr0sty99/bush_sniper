using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    [SerializeField]
    private int maxHealth = 100;

    [SyncVar] // this prefix means, when the value of this variable changes, it will be "pushed out" to all other clients
    private int currentHealt;

	private void Awake()
	{
        SetDefaults();

	}

    public void TakeDamage(int _amount) 
    {
        currentHealt -= _amount;

        // only on server because this is the host, we deactivate all remotePlayer-Components
        Debug.Log(transform.name + " now has " + currentHealt + " Health.");
    }

    public void SetDefaults() 
    {
        currentHealt = maxHealth;
    }
}
