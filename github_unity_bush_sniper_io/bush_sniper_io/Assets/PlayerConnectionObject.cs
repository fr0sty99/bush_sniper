using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnectionObject : NetworkBehaviour
{

    // Use this for initialization
    void Start()
    {
        // is this actually my own local PlayerConnectionObject?
        if (isLocalPlayer == false)
        {
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

    public GameObject PlayerUnitPrefab;

    // SyncVars are variables where if their value changes on the SERVER, then all clients are 
    // automatically informed of the new value;

    // hooked SyncVar
    [SyncVar(hook = "OnPlayerNameChanged")]
    public string PlayerName = "Anonymous";

    /* without hooked function it would look like this:
     * 
        [SyncVar]
        public string PlayerName = "Anonymous";
    */

    // Update is called once per frame
    void Update()
    {
        // Remeber: Update runs on EVERYONE's computer,
        // whether or not they own this particular PlayerConnectionObject

        // if this is not our PlayerObject, we bail out 
        if (isLocalPlayer == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            CmdSpawnMyUnit();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            string n = "Player " + Random.Range(1, 100);

            Debug.Log("Sending the server a request to change our name to " + n);
            CmdChangePlayerName(n);
        }

    }

    void OnPlayerNameChanged(string newName)
    {
        Debug.Log("OnPlayerNameChanged: Old Name: " + PlayerName + "   NewName: " + newName);

        // WARNING: If you use a hook on a SyncVar, then our local value
        // does NOT get automatically updated
        PlayerName = newName;

        gameObject.name = "PlayerConnectionObject ["+newName+"]";
    }


    ///////////////////////////////////////////////// COMMANDS
    // Commands are special functions that only get executed on the server.
    // We can define commands with a 'Command' annotation

    [Command]
    void CmdSpawnMyUnit()
    {
        // We are guaranteed to be on the server right now
        GameObject go = Instantiate(PlayerUnitPrefab);

        // next line is not needed, because we spawn with networkauthority (NetworkServer.SpawnWithClientAuthority(go, connectionToClient);)
        // go.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);    

        // Now that the object exists on the server, propagate it to all
        // the clients (and also wire up the NetworkIdentity) 
        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    }

    [Command]
    void CmdChangePlayerName(string n)
    {
        Debug.Log("CmdChangePlayerName: " + n);

        // Maybe we should check that the name doesn't have any blacklisted words in it?
        // If there is a bad word in the name, do we just ignore this request and do nothing?
        // ... or do we still call the Rpc but with the original name?

        PlayerName = n;

        // Tell all the client what this player's name now is
        // RpcChangePlayerName(PlayerName);
    }


    ///////////////////////////////////////////////// RPC
    // RPCs are special functions that ONLY get executed on the client

    /*    Not needed anymore because playerName is a SyncVar now
     * 
     * [ClientRpc]
        void RpcChangePlayerName(string n) {
            Debug.Log("RpcChangePlayerName: We were asked to change the player name on a particular PlayerConnectionObject: " + n);
            PlayerName = n;
        }
        */
}
