using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerObject : NetworkBehaviour {

	void Start () {
        if(isLocalPlayer == false) {
            // this object belongs to another player.
            return;
        }

        // since the PlayerObject is invisible and not part of the world
        // give me something physical to move around

        Debug.Log("PLayerObject::Start -- Spawning my own personal unit.");
    //    Instantiate(PlayerUnitPrefab);
	}

   // public GameObject PlayerUnitPrefab,
	
	void Update () {
		
	}
}
