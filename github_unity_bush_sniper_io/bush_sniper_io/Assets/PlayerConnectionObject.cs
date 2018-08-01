using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnectionObject : NetworkBehaviour {
    public GameObject PlayerUnitPrefab;


	// Use this for initialization
	void Start () {
        // is this actually my own local PlayerConnectionObject?
        if( isLocalPlayer == false ) {
            // This object belongs to another player
            return;
        } 
        
        // sine the PLayerObject is invisible and not part  of the world, give me something physical to move around
        Debug.Log("PlayerConnectionObject::Start -- Spawning my own personal unity");

        // Instante() only creates an object on the local computer
        // Even if it has a NetworkIdentity it still will NOT exist on
        // the network ( and therefore not on any other client )
        // unless NetworkServer.Spawn() is called on this object

        // NetworkServer.Spawn() is the only way to make a player appear on all clients

        // Instantiate is used for things like f.e. a firework, there's no need for network-synchronization
        // Instantiate(PlayerUnitPrefab);

        // Command (politely) the server to SPAWN our unit
        CmdSpawnMyUnit();

    }

	 
	// Update is called once per frame
	void Update () {
        // Remeber: Update runs on EVERYONE's computer,
        // whether or not they own this particular PlayerConnectionObject
        if (isLocalPlayer == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.S)) {
            CmdSpawnMyUnit();
        }
	}


    ///////////////////////////////////////////////// COMMANDS
    // Commands are special functions that only get executed on the server.
    // We can define commands with a 'Command' annotation

    [Command]
    void CmdSpawnMyUnit() {
        // We are guaranteed to be on the server right now
        GameObject go = Instantiate(PlayerUnitPrefab);

        // next line is not needed, because we spawn with networkauthority (NetworkServer.SpawnWithClientAuthority(go, connectionToClient);)
        // go.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);    


        // Now that the object exists on the server, propagate it to all
        // the clients (and also wire up the NetworkIdentity) 
        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    }

}
